using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static Unity.Mathematics.math;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// XR 환경에서 손으로 잡고 움직일 수 있는 조이스틱.  
    /// X/Y 방향으로 움직임을 감지하며, 움직임을 정해진 각도로 회전으로 변환하고 이벤트를 호출함.
    /// </summary>
    public class XRJoystick : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractable xrGrabInteractable = null; // 잡을 수 있게 하는 XR 인터랙터
        [SerializeField] private Transform rotationPoint = null;               // 조이스틱이 회전할 지점 (보통 조이스틱 헤드)
        [SerializeField] private float maxAngle = 60;                          // 최대 회전 각도
        [SerializeField] private float shaftLength = .2f;                      // 조이스틱 축의 길이 (움직임 범위)
        [SerializeField] private bool returnToStartOnRelease = true;          // 손을 놓았을 때 원위치로 돌아갈지 여부
        [SerializeField] private float returnSpeed = 5;                        // 원위치로 돌아가는 속도
        [SerializeField] private Vector2 startingPosition = Vector2.zero;     // 시작 시 위치 (X/Y -1 ~ 1 범위)
        [SerializeField] private Vector2 returnToPosition = Vector2.zero;     // 놓았을 때 돌아갈 위치
        [SerializeField] private bool xAxis = true, yAxis = true;             // X/Y 축 활성화 여부
        [SerializeField] private float remapValueMin = -1, remapValueMax = 1; // 출력 값 범위 (예: 0 ~ 1 로 바꿔줄 수 있음)
        [SerializeField] private bool InvokeEventsAtStart;                    // 시작할 때 값 변경 이벤트 호출 여부

        private Transform hand, originalPositionTracker;                      // 현재 잡고 있는 손, 초기 위치 기준 오브젝트
        public Vector2 CurrentValue { get; private set; }                     // 현재 조이스틱의 위치 값 (-1 ~ 1)
        public Vector2 RemapValue { get; private set; }                       // 리맵된 값 (remapValueMin ~ Max)

        public UnityEventVector2 ValueChanged;                                // 위치 변경 이벤트 (벡터)
        public UnityEventFloat SingleValueChanged;                            // 단일 축 이벤트 (X 또는 Y 하나만 쓸 때)
        private Vector3 handOffSetFromStartOfGrab;                            // 손이 잡았을 때 오프셋 (잡은 위치 보정용)

        private void Start()
        {
            OnValidate();

            // 기준 위치 오브젝트 생성 (조이스틱 중심 고정용)
            originalPositionTracker = new GameObject("originalPositionTracker").transform;
            originalPositionTracker.parent = transform.parent;
            originalPositionTracker.localPosition = transform.localPosition;
            originalPositionTracker.localRotation = transform.localRotation;

            // 잡기/놓기 이벤트 등록
            xrGrabInteractable.selectEntered.AddListener(OnGrab);
            xrGrabInteractable.selectExited.AddListener((x) => hand = null);
            xrGrabInteractable.selectExited.AddListener((x) => StartCoroutine(ReturnToZero()));

            // 시작 시 이벤트 호출할지
            if (InvokeEventsAtStart)
                InvokeUnityEvents();
        }

        // 에디터에서 값 변경 시 자동 참조 연결
        private void OnValidate()
        {
            if (!xrGrabInteractable)
                xrGrabInteractable = GetComponent<XRBaseInteractable>();

            if (rotationPoint)
                SetStartPosition();
        }

        // 시작 위치 설정
        private void SetStartPosition()
        {
            float x = remap(-1, 1, -shaftLength, shaftLength, startingPosition.x);
            float z = remap(-1, 1, -shaftLength, shaftLength, startingPosition.y);
            SetHandleRotation(new Vector3(x, 0, z));
        }

        // 조이스틱을 잡았을 때
        private void OnGrab(SelectEnterEventArgs x)
        {
            StopAllCoroutines();
            hand = x.interactorObject.transform;
            handOffSetFromStartOfGrab = x.interactorObject.transform.position - transform.position;
        }

        private void Update()
        {
            if (!hand) return;

            GetVectorProjectionFromHand(out var locRot);
            SetHandleRotation(locRot);
            InvokeUnityEvents();
        }

        // 손의 위치를 기준으로 조이스틱의 회전 방향 계산
        private void GetVectorProjectionFromHand(out Vector3 locRot)
        {
            Vector3 positionToProject = hand.position - handOffSetFromStartOfGrab;
            Vector3 v = positionToProject - transform.position;

            // 조이스틱 위쪽 방향 기준으로 평면 투영
            Vector3 projection = Vector3.ProjectOnPlane(v, originalPositionTracker.up);

            // 최대 이동 범위 내로 제한
            Vector3 projectedPoint;
            if (xAxis & yAxis)
                projectedPoint = transform.position + Vector3.ClampMagnitude(projection, 1);
            else
                projectedPoint = transform.position + new Vector3(Mathf.Clamp(projection.x, -1, 1), 0, Mathf.Clamp(projection.z, -1, 1));

            locRot = transform.InverseTransformPoint(projectedPoint);
        }

        // 조이스틱을 실제로 회전시킴
        private void SetHandleRotation(Vector3 locRot)
        {
            float x = remap(-shaftLength, shaftLength, -1, 1, locRot.x);
            float z = remap(-shaftLength, shaftLength, -1, 1, locRot.z);

            Vector3 newValue = Vector3.zero;

            if (xAxis & yAxis)
                newValue = Vector2.ClampMagnitude(new Vector2(x, z), 1);
            if (!xAxis)
                newValue = new Vector2(0, Mathf.Clamp(z, -1, 1));
            if (!yAxis)
                newValue = new Vector2(Mathf.Clamp(x, -1, 1), 0);

            // 실제 회전 적용
            rotationPoint.localEulerAngles = new Vector3(newValue.y * maxAngle, 0, -newValue.x * maxAngle);

            CurrentValue = newValue;
        }

        // 이벤트 호출 (값 변경)
        private void InvokeUnityEvents()
        {
            // 예: -1~1 → 0~1 로 변환
            RemapValue = remap(-1, 1, remapValueMin, remapValueMax, CurrentValue);
            ValueChanged.Invoke(RemapValue);

            // 단일 축만 쓸 경우 하나만 전달
            if (!xAxis)
                SingleValueChanged.Invoke(RemapValue.y);
            if (!yAxis)
                SingleValueChanged.Invoke(RemapValue.x);
        }

        // 손에서 놓았을 때 원래 위치로 돌아감
        private IEnumerator ReturnToZero()
        {
            if (!returnToStartOnRelease) yield break;

            while (CurrentValue.magnitude >= .01f)
            {
                CurrentValue = Vector2.Lerp(CurrentValue, returnToPosition, Time.deltaTime * returnSpeed);
                rotationPoint.localEulerAngles = new Vector3(CurrentValue.y * maxAngle, 0, -CurrentValue.x * maxAngle);
                InvokeUnityEvents();
                yield return null;
            }

            CurrentValue = returnToPosition;
            rotationPoint.localEulerAngles = returnToPosition;
            InvokeUnityEvents();
        }

        // 에디터에서 보기 편하게 시각화
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            if (xAxis && yAxis)
                Gizmos.DrawWireSphere(transform.position, shaftLength);
            if (!xAxis && yAxis)
                Gizmos.DrawLine(transform.position - transform.forward * shaftLength, transform.position + transform.forward * shaftLength);
            if (!yAxis && xAxis)
                Gizmos.DrawLine(transform.position - transform.right * shaftLength, transform.position + transform.right * shaftLength);

            Gizmos.DrawLine(transform.position, transform.position + transform.up * shaftLength);
        }
    }
}
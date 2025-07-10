using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 총의 슬라이드를 당기는 액션(탄 장전)을 구현한 컴포넌트입니다.
    /// 슬라이드를 특정 방향으로 당길 수 있고, 끝까지 당기면 효과음과 함께 "장전 완료" 이벤트가 발생합니다.
    /// 손을 놓으면 원래 위치로 슬라이드가 돌아옵니다.
    /// </summary>
    public class GunCocking : MonoBehaviour
    {
        // XR 인터랙션용 상호작용 가능한 오브젝트들
        [SerializeField] private XRBaseInteractable xrGrabInteractable = null; // 슬라이드 부분에 해당
        [SerializeField] private XRGrabInteractable mainGrabInteractable = null; // 총 몸통
        [SerializeField] private ProjectileWeapon projectileWeapon = null; // 발사 관련 스크립트 참조

        // 슬라이드 이동 방향 및 거리
        [SerializeField] private Vector3 LocalAxis = -Vector3.forward; // 당기는 방향
        [SerializeField] private float AxisLength = .1f; // 이동 거리
        [SerializeField] private float ReturnSpeed = 1; // 손을 놓았을 때 원위치로 돌아가는 속도

        // 사운드
        [SerializeField] private AudioRandomize pullBackAudio = null; // 당겼을 때 효과음
        [SerializeField] private AudioRandomize releaseAudio = null; // 놓았을 때 효과음

        // 내부 처리용 변수들
        private IXRSelectInteractor currentHand, grabbingInteractor; // 현재 슬라이드를 잡고 있는 손
        private XRInteractionManager interactionManager;
        private Transform originalParent;
        private Vector3 grabbedOffset, endPoint, startPoint;
        private float currentDistance;
        private bool hasReachedEnd, isSelected;
        private Rigidbody rb;

        // 장전 완료 시 발생하는 이벤트
        public UnityEvent GunCockedEvent;

        private void Start()
        {
            // XR 이벤트 연결
            xrGrabInteractable.selectEntered.AddListener(OnGrabbed);   // 슬라이드를 잡았을 때
            xrGrabInteractable.selectExited.AddListener(OnRelease);    // 슬라이드를 놓았을 때
            mainGrabInteractable.selectExited.AddListener(ReleaseIfMainHandReleased); // 총 몸통을 놓았을 때 슬라이드도 놓기

            originalParent = transform.parent;
            LocalAxis.Normalize(); // 이동 방향 정규화

            // 이동 방향이 음수이면 자동으로 방향 반전
            if (AxisLength < 0)
            {
                LocalAxis *= -1;
                AxisLength *= -1;
            }

            startPoint = transform.localPosition;
            endPoint = transform.localPosition + LocalAxis * AxisLength;
        }

        private void OnEnable()
        {
            OnValidate();

            // 인터랙션 매니저 갱신
            interactionManager.UnregisterInteractable(xrGrabInteractable as IXRInteractable);
            interactionManager.RegisterInteractable(xrGrabInteractable as IXRInteractable);
        }

        // 에디터에서 참조가 빠졌을 경우 자동으로 연결
        private void OnValidate()
        {
            if (!interactionManager)
                interactionManager = FindFirstObjectByType<XRInteractionManager>();
            if (!xrGrabInteractable)
                xrGrabInteractable = GetComponent<XRBaseInteractable>();
            if (!mainGrabInteractable)
                mainGrabInteractable = transform.parent.GetComponentInParent<XRGrabInteractable>();
            if (!projectileWeapon)
                projectileWeapon = GetComponentInParent<ProjectileWeapon>();
        }

        // 외부에서 시작점과 끝점을 가져오는 메서드
        public Vector3 GetEndPoint() => endPoint;
        public Vector3 GetStartPoint() => startPoint;

        public void FixedUpdate()
        {
            if (stopAnimation) return;

            if (isSelected)
                SlideFromHandPosition(); // 손으로 당기고 있는 경우 슬라이드 위치 조정
            else
                ReturnToOriginalPosition(); // 손을 놓으면 자동 복귀
        }

        // 총 몸통을 놓으면 슬라이드도 놓도록 처리
        private void ReleaseIfMainHandReleased(SelectExitEventArgs arg0)
        {
            if (currentHand?.transform && xrGrabInteractable)
                interactionManager.SelectExit(currentHand,
                    xrGrabInteractable.GetComponent<IXRSelectInteractable>());
        }

        // 손 위치를 기반으로 슬라이드를 당기는 동작 처리
        private void SlideFromHandPosition()
        {
            Vector3 worldAxis = transform.TransformDirection(LocalAxis); // 월드 방향으로 변환

            Vector3 distance = grabbingInteractor.transform.position - transform.position - grabbedOffset;
            float projected = Vector3.Dot(distance, worldAxis); // 축에 따라 투영된 거리

            Vector3 targetPoint;
            if (projected > 0)
                targetPoint = Vector3.MoveTowards(transform.localPosition, endPoint, projected);
            else
                targetPoint = Vector3.MoveTowards(transform.localPosition, startPoint, -projected);

            Vector3 move = targetPoint - transform.localPosition;
            transform.localPosition += move;

            // 끝에 도달했을 때 효과음 재생
            if (!hasReachedEnd && (transform.localPosition - endPoint).magnitude <= .001f)
            {
                hasReachedEnd = true;
                pullBackAudio.Play();
            }
        }

        // 원래 위치로 슬라이드를 복귀시키는 처리
        private void ReturnToOriginalPosition()
        {
            Vector3 targetPoint = Vector3.MoveTowards(transform.localPosition, startPoint, ReturnSpeed * Time.deltaTime);
            Vector3 move = targetPoint - transform.localPosition;

            transform.localPosition += move;

            // 복귀 완료 시 장전 이벤트 발생 및 사운드 재생
            if (hasReachedEnd && (transform.localPosition - startPoint).magnitude <= .001f)
            {
                hasReachedEnd = false;
                GunCockedEvent.Invoke();
                releaseAudio.Play();
                SetClosed();
            }
        }

        private bool stopAnimation;

        // 애니메이션 활성화 (복귀 허용)
        public void SetClosed()
        {
            stopAnimation = false;
        }

        // 슬라이드를 잡았을 때
        private void OnGrabbed(SelectEnterEventArgs arg0)
        {
            var interactor = arg0.interactorObject;
            stopAnimation = false;
            currentHand = interactor;
            isSelected = true;
            grabbedOffset = interactor.transform.position - transform.position;
            grabbingInteractor = interactor;
            transform.parent = originalParent; // 부모를 원래대로
            transform.localPosition = startPoint;
        }

        // 슬라이드를 놓았을 때
        private void OnRelease(SelectExitEventArgs arg0)
        {
            currentHand = null;
            isSelected = false;
            transform.localPosition = startPoint;
        }

        // Scene에서 선택 시 슬라이드 축을 시각적으로 보여줌
        private void OnDrawGizmosSelected()
        {
            Vector3 end = transform.position + transform.TransformDirection(LocalAxis.normalized) * AxisLength;
            Gizmos.DrawLine(transform.position, end);
            Gizmos.DrawSphere(end, 0.01f);
        }

        // 슬라이드 복귀 애니메이션을 일시정지
        public void Pause()
        {
            stopAnimation = true;
        }
    }
}
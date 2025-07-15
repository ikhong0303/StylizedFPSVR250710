// Author: MikeNspired

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// XRGrabInteractable이 잡혔을 때/놓였을 때,
    /// 지정한 자식 Transform들을 활성/비활성, 혹은 이동/비활성 처리해주는 스크립트.
    /// (예: 칼집에서 칼을 뽑으면 칼집이 비활성화되고, 칼이 활성화되는 식)
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class OnGrabEnableDisable : MonoBehaviour, IReturnMovedColliders
    {
        [Header("Main References")]
        [SerializeField]
        private XRGrabInteractable grabInteractable; // XR Grab 기능을 담당하는 컴포넌트 (자동 할당)

        [Tooltip("잡았을 때 꺼지고, 놓으면 켜질 Transform (ex. 칼집)")]
        [SerializeField]
        private Transform disableOnGrab;

        [Tooltip("잡았을 때 켜지고, 놓으면 꺼질 Transform (ex. 칼)")]
        [SerializeField]
        private Transform enableOnGrab;

        [Header("Settings")]
        [Tooltip("true면, Grab/Release 시 콜라이더를 멀리 보내서 비활성화 후 위치/콜라이더 원복")]
        [SerializeField]
        private bool moveAndDisableAfterFrameOnGrabColliders = true;

        // 잡고/놓았을 때 원래 위치 기억용
        private Vector3 disableOnGrabStartPosition;
        private Vector3 enableOnGrabStartPosition;

        private void Awake()
        {
            OnValidate();

            // 초기 위치 저장
            if (disableOnGrab)
                disableOnGrabStartPosition = disableOnGrab.localPosition;
            if (enableOnGrab)
                enableOnGrabStartPosition = enableOnGrab.localPosition;

            // Grab/Release 이벤트 연결
            grabInteractable.selectEntered.AddListener(_ => OnGrab());
            grabInteractable.selectExited.AddListener(_ => OnRelease());
        }

        // 에디터에서 참조 자동 할당
        private void OnValidate()
        {
            if (!grabInteractable)
                grabInteractable = GetComponent<XRGrabInteractable>();
        }

        // Start 시, 초깃값 세팅 (비활성/활성 구분)
        private void Start()
        {
            if (disableOnGrab)
                disableOnGrab.gameObject.SetActive(true);
            if (enableOnGrab)
                enableOnGrab.gameObject.SetActive(false);
        }

        // XR 오브젝트를 잡았을 때 호출됨
        private void OnGrab()
        {
            if (moveAndDisableAfterFrameOnGrabColliders)
            {
                StopAllCoroutines();

                // enableOnGrab 오브젝트 바로 활성화
                EnableTransform(enableOnGrab, enableOnGrabStartPosition);

                // disableOnGrab 오브젝트는, 콜라이더 상태 복구 후 멀리 보냈다가 비활성화
                if (disableOnGrab)
                {
                    var collidersTrigger = disableOnGrab.GetComponent<CollidersSetToTrigger>();
                    collidersTrigger?.ReturnToDefaultState();

                    StartCoroutine(MoveDisableAndReturn(disableOnGrab, disableOnGrabStartPosition));
                }
            }
            else
            {
                // 그냥 바로 켜고/끄는 모드 (코루틴 미사용)
                if (disableOnGrab)
                    disableOnGrab.gameObject.SetActive(false);
                EnableTransform(enableOnGrab, enableOnGrabStartPosition);
            }
        }

        // XR 오브젝트를 놓았을 때 호출됨
        private void OnRelease()
        {
            if (moveAndDisableAfterFrameOnGrabColliders)
            {
                StopAllCoroutines();

                // disableOnGrab 오브젝트 바로 활성화
                EnableTransform(disableOnGrab, disableOnGrabStartPosition);

                // enableOnGrab 오브젝트는 콜라이더 복구 후 멀리 보냈다가 비활성화
                if (enableOnGrab)
                {
                    var collidersTrigger = enableOnGrab.GetComponent<CollidersSetToTrigger>();
                    collidersTrigger?.ReturnToDefaultState();

                    StartCoroutine(MoveDisableAndReturn(enableOnGrab, enableOnGrabStartPosition));
                }
            }
            else
            {
                // 바로 켜고/끄기
                if (enableOnGrab)
                    enableOnGrab.gameObject.SetActive(false);
                if (disableOnGrab)
                    disableOnGrab.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 모든 오브젝트를 원래 상태로 복구 (위치/콜라이더 포함)
        /// </summary>
        public void EnableAll()
        {
            StopAllCoroutines();

            if (disableOnGrab)
            {
                disableOnGrab.gameObject.SetActive(true);
                ResetTransformLocal(disableOnGrab, disableOnGrabStartPosition);
                ResetCollidersToDefault(disableOnGrab);
            }
            if (enableOnGrab)
            {
                enableOnGrab.gameObject.SetActive(true);
                ResetTransformLocal(enableOnGrab, enableOnGrabStartPosition);
                ResetCollidersToDefault(enableOnGrab);
            }
        }

        /// <summary>
        /// 콜라이더 이동된거 복구 (IReturnMovedColliders용)
        /// </summary>
        public void ReturnMovedColliders()
        {
            StopAllCoroutines();
            if (enableOnGrab)
                enableOnGrab.localPosition = enableOnGrabStartPosition;
            if (disableOnGrab)
                disableOnGrab.localPosition = disableOnGrabStartPosition;
        }

        /// <summary>
        /// 코루틴: 오브젝트를 멀리 이동, 비활성, 위치/콜라이더 원복
        /// </summary>
        private IEnumerator MoveDisableAndReturn(Transform objectToMove, Vector3 originalLocalPosition)
        {
            if (!objectToMove) yield break;

            // 콜라이더 임시로 트리거로 바꿈 (충돌방지)
            var collidersTrigger = objectToMove.GetComponent<CollidersSetToTrigger>();
            collidersTrigger?.SetAllToTrigger();

            yield return PhysicsHelper.MoveAndDisable(objectToMove.gameObject);

            // 위치/콜라이더 복구
            ResetTransformLocal(objectToMove, originalLocalPosition);
            collidersTrigger?.ReturnToDefaultState();
        }

        #region Helper Methods

        // 위치 원복
        private void ResetTransformLocal(Transform target, Vector3 localPos)
        {
            if (!target) return;
            target.localPosition = localPos;
        }

        // 오브젝트 활성화 및 위치/콜라이더 초기화
        private void EnableTransform(Transform target, Vector3 startPos)
        {
            if (!target) return;
            target.gameObject.SetActive(true);
            target.localPosition = startPos;
            ResetCollidersToDefault(target);
        }

        // 콜라이더 상태 원복
        private void ResetCollidersToDefault(Transform target)
        {
            if (!target) return;
            var collidersTrigger = target.GetComponent<CollidersSetToTrigger>();
            collidersTrigger?.ReturnToDefaultState();
        }

        #endregion
    }
}

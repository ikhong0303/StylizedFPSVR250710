// Author MikeNspired. 

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 손 애니메이션을 설정하는 메인 스크립트.
    /// 아이템별 손 포즈를 간단히 설정한 뒤, 아이템을 잡았을 때 해당 포즈를 손에 적용하는 것이 목적.
    /// Unity XR 시스템에서 XRGrabInteractable과 함께 작동하며, onSelectEnter / onSelectExit 이벤트를 사용함.
    /// </summary>
    public class XRHandPoser : HandPoser
    {
        public XRBaseInteractable interactable; // XR 인터랙션 대상 (예: 총기, 손잡이 등)
        public bool MaintainHandOnObject = true; // 손이 물체 위에서 유지되도록 할지 여부
        public bool WaitTillEaseInTimeToMaintainPosition = true; // ease 시간 동안 기다릴지 여부
        public bool overrideEaseTime = false; // easeInTime 값을 override할지 여부
        public float easeInTimeOverride = 0; // override할 경우 사용하는 easeIn 시간

        protected override void Awake()
        {
            base.Awake();
            OnValidate();          // interactable 자동 할당
            SubscribeToSelection(); // 이벤트 연결
        }

        // 물체를 잡거나 놓았을 때의 이벤트 등록
        private void SubscribeToSelection()
        {
            interactable.selectEntered.AddListener(TryStartPosing);   // 잡았을 때 손 포즈 적용
            interactable.selectExited.AddListener(TryReleaseHand);    // 놓았을 때 손 포즈 해제
        }

        // 물체를 잡았을 때 호출됨
        private void TryStartPosing(SelectEnterEventArgs x)
        {
            // 손 객체 찾기
            var hand = x.interactorObject.transform.GetComponentInParent<HandReference>();
            if (!hand) return;

            // 멀리서 잡는 경우(원격 XR 레이)에는 포즈 적용 생략
            if (hand.NearFarInteractor != null && hand.NearFarInteractor.interactionAttachController.hasOffset)
            {
                Debug.Log("Hand poser skipped also");
                return;
            }

            // 손 포즈 적용
            BeginNewHandPoses(hand.Hand);
        }

        // 물체를 놓았을 때 호출됨
        private void TryReleaseHand(SelectExitEventArgs x)
        {
            if (!x.interactorObject.transform.GetComponentInParent<HandReference>()) return;
            Release(); // 손 포즈 해제
        }

        // 실제 손을 포즈 위치로 이동시키는 함수
        private void MoveHandToPoseTransforms(HandAnimator hand)
        {
            // 왼손/오른손에 따라 해당 포즈 위치 가져옴
            var attachPoint = hand.handType == LeftRight.Left ? leftHandAttach : rightHandAttach;

            // 손을 지정한 포즈 위치로 천천히 이동시킴
            hand.MoveHandToTarget(attachPoint, GetEaseInTime(), WaitTillEaseInTimeToMaintainPosition);
        }

        // 손 포즈 적용 (부모 클래스 override)
        protected override void BeginNewHandPoses(HandAnimator hand)
        {
            if (!hand || !CheckIfPoseExistForHand(hand)) return;

            base.BeginNewHandPoses(hand); // 기본 처리 실행

            if (MaintainHandOnObject)     // 손을 물체 위에 유지할 경우
                MoveHandToPoseTransforms(hand);
        }

        // 해당 손 타입(왼손/오른손)에 대한 포즈가 존재하는지 확인
        private bool CheckIfPoseExistForHand(HandAnimator hand)
        {
            if (leftHandPose && hand.handType == LeftRight.Left)
                return true;
            if (rightHandPose && hand.handType == LeftRight.Right)
                return true;
            return false;
        }

        // 손이 포즈 위치까지 천천히 이동할 때 걸리는 시간 반환
        private float GetEaseInTime()
        {
            float time = 0;

            // XRGrabInteractable이 있을 경우 그 값 사용
            interactable.TryGetComponent(out XRGrabInteractable xrGrabInteractable);
            if (xrGrabInteractable)
                time = xrGrabInteractable.attachEaseInTime;

            // override 옵션이 켜져 있으면 해당 값으로 덮어쓰기
            if (overrideEaseTime)
                time = easeInTimeOverride;

            return time;
        }

        // 에디터에서 컴포넌트가 할당되지 않은 경우 자동 할당 시도
        private void OnValidate()
        {
            if (!interactable)
                interactable = GetComponent<XRBaseInteractable>();
            if (!interactable)
                interactable = GetComponentInParent<XRBaseInteractable>();
            if (!interactable)
                Debug.LogWarning(gameObject + " XRGrabPoser does not have an XRGrabInteractable assigned." + "  (Parent name) " + transform.parent);
        }
    }
}
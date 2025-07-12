using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// VR에서 손(Hand)이 어떤 오브젝트(예: 손잡이, 버튼 등) 근처로 다가가면
    /// 자동으로 손 포즈(모양)를 바꿔주는 스크립트
    /// (버튼에 손을 올려놓기만 해도 손모양 변환, 잡지 않아도 적용 가능)
    /// </summary>
    public class XRHandPoserHoverActivate : HandPoser
    {
        // Inspector에서 지정하는, 이 손포즈를 활성화시킬 XRBaseInteractable
        [SerializeField] private XRBaseInteractable mainInteractable;
        private HandAnimator currentHand; // 현재 포즈를 적용 중인 손(HandAnimator)

        public UnityEvent OnActivate;   // 손 포즈가 활성화될 때 실행될 이벤트
        public UnityEvent OnDeactivate; // 손 포즈가 비활성화될 때 실행될 이벤트

        // 초기화 (Awake 시점에 실행)
        protected override void Awake()
        {
            base.Awake();     // 부모 클래스의 Awake 실행
            OnValidate();     // mainInteractable 자동 연결

            // XRBaseInteractable에 selectExited(잡았다가 놓는 이벤트) 등록
            if (mainInteractable != null)
            {
                mainInteractable.selectExited.AddListener(OnSelectExited);
            }
        }

        // 에디터/런타임에서 mainInteractable 자동 연결
        private void OnValidate()
        {
            if (!mainInteractable)
                mainInteractable = GetComponentInParent<XRBaseInteractable>();
        }

        // 트리거 콜라이더에 뭔가 들어오면 실행
        private void OnTriggerEnter(Collider other)
        {
            if (currentHand) return; // 이미 손 포즈 적용 중이면 무시

            var handReference = other.GetComponentInParent<HandReference>();
            if (handReference == null) return; // 손이 아니면 무시

            // 손(HandReference) 아래에서 NearFarInteractor(그랩 컨트롤) 찾기
            var interactor = handReference.GetComponentInChildren<NearFarInteractor>();
            // 이미 뭔가 잡고 있으면 무시 (잡은 상태에선 적용 X)
            if (interactor != null && interactor.firstInteractableSelected != null) return;

            // 손 포즈 적용!
            currentHand = handReference.Hand;
            BeginNewHandPoses(currentHand); // 손 포즈 변경
            OnActivate.Invoke();            // (이벤트 발생, 외부에서 사운드 등 연동 가능)
            currentHand.NewPoseStarting += ReleaseHand; // 새로운 포즈가 시작될 때 ReleaseHand 실행
        }

        // 트리거 콜라이더에서 뭔가 나가면 실행
        private void OnTriggerExit(Collider other)
        {
            var handReference = other.GetComponentInParent<HandReference>();
            if (handReference == null) return;

            var interactor = handReference.GetComponentInChildren<NearFarInteractor>();
            if (interactor != null && interactor.firstInteractableSelected != null) return;
            ReleaseHand(); // 손이 영역 밖으로 나가면 포즈 해제
        }

        // XRBaseInteractable에서 selectExited(손을 놓았을 때) 실행
        private void OnSelectExited(SelectExitEventArgs args) => ReleaseHand();

        // 손이 뭔가를 잡으면 포즈 해제
        private void ReleaseHand(bool isGrabbingItem)
        {
            if (isGrabbingItem) ReleaseHand();
        }

        // 손 포즈 해제 (손이 나가거나, 잡았을 때 등)
        private void ReleaseHand()
        {
            if (currentHand == null) return;

            currentHand.NewPoseStarting -= ReleaseHand; // 이벤트 해제
            currentHand = null;                         // 손 레퍼런스 해제
            Release();                                 // 부모 클래스 Release (포즈 해제)
            OnDeactivate.Invoke();                      // (이벤트 발생, 외부에서 사운드 등 연동 가능)
        }

        // 실제 손 위치를 지정된 포즈 위치로 이동시키는 함수
        private void MoveHandToPoseTransforms(HandAnimator hand)
        {
            // 왼손/오른손에 따라 각 포즈 위치로 이동
            var attachPoint = hand.handType == LeftRight.Left ? leftHandAttach : rightHandAttach;
            hand.MoveHandToTarget(attachPoint, 0, false); // 즉시 이동
        }

        // 손 포즈 적용 시작 (부모 클래스의 함수 오버라이드)
        protected override void BeginNewHandPoses(HandAnimator hand)
        {
            if (!hand || !CheckIfCorrectHand(hand)) return;

            base.BeginNewHandPoses(hand); // 부모 클래스 처리
            MoveHandToPoseTransforms(hand); // 실제 위치로 이동
        }

        // 이 포즈가 적용될 손(왼손/오른손)인지 체크
        private bool CheckIfCorrectHand(HandAnimator hand) =>
            (leftHandPose && hand.handType == LeftRight.Left) ||
            (rightHandPose && hand.handType == LeftRight.Right);
    }
}
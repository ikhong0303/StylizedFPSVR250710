// Author MikeNspired. 

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// �� �ִϸ��̼��� �����ϴ� ���� ��ũ��Ʈ.
    /// �����ۺ� �� ��� ������ ������ ��, �������� ����� �� �ش� ��� �տ� �����ϴ� ���� ����.
    /// Unity XR �ý��ۿ��� XRGrabInteractable�� �Բ� �۵��ϸ�, onSelectEnter / onSelectExit �̺�Ʈ�� �����.
    /// </summary>
    public class XRHandPoser : HandPoser
    {
        public XRBaseInteractable interactable; // XR ���ͷ��� ��� (��: �ѱ�, ������ ��)
        public bool MaintainHandOnObject = true; // ���� ��ü ������ �����ǵ��� ���� ����
        public bool WaitTillEaseInTimeToMaintainPosition = true; // ease �ð� ���� ��ٸ��� ����
        public bool overrideEaseTime = false; // easeInTime ���� override���� ����
        public float easeInTimeOverride = 0; // override�� ��� ����ϴ� easeIn �ð�

        protected override void Awake()
        {
            base.Awake();
            OnValidate();          // interactable �ڵ� �Ҵ�
            SubscribeToSelection(); // �̺�Ʈ ����
        }

        // ��ü�� ��ų� ������ ���� �̺�Ʈ ���
        private void SubscribeToSelection()
        {
            interactable.selectEntered.AddListener(TryStartPosing);   // ����� �� �� ���� ����
            interactable.selectExited.AddListener(TryReleaseHand);    // ������ �� �� ���� ����
        }

        // ��ü�� ����� �� ȣ���
        private void TryStartPosing(SelectEnterEventArgs x)
        {
            // �� ��ü ã��
            var hand = x.interactorObject.transform.GetComponentInParent<HandReference>();
            if (!hand) return;

            // �ָ��� ��� ���(���� XR ����)���� ���� ���� ����
            if (hand.NearFarInteractor != null && hand.NearFarInteractor.interactionAttachController.hasOffset)
            {
                Debug.Log("Hand poser skipped also");
                return;
            }

            // �� ���� ����
            BeginNewHandPoses(hand.Hand);
        }

        // ��ü�� ������ �� ȣ���
        private void TryReleaseHand(SelectExitEventArgs x)
        {
            if (!x.interactorObject.transform.GetComponentInParent<HandReference>()) return;
            Release(); // �� ���� ����
        }

        // ���� ���� ���� ��ġ�� �̵���Ű�� �Լ�
        private void MoveHandToPoseTransforms(HandAnimator hand)
        {
            // �޼�/�����տ� ���� �ش� ���� ��ġ ������
            var attachPoint = hand.handType == LeftRight.Left ? leftHandAttach : rightHandAttach;

            // ���� ������ ���� ��ġ�� õõ�� �̵���Ŵ
            hand.MoveHandToTarget(attachPoint, GetEaseInTime(), WaitTillEaseInTimeToMaintainPosition);
        }

        // �� ���� ���� (�θ� Ŭ���� override)
        protected override void BeginNewHandPoses(HandAnimator hand)
        {
            if (!hand || !CheckIfPoseExistForHand(hand)) return;

            base.BeginNewHandPoses(hand); // �⺻ ó�� ����

            if (MaintainHandOnObject)     // ���� ��ü ���� ������ ���
                MoveHandToPoseTransforms(hand);
        }

        // �ش� �� Ÿ��(�޼�/������)�� ���� ��� �����ϴ��� Ȯ��
        private bool CheckIfPoseExistForHand(HandAnimator hand)
        {
            if (leftHandPose && hand.handType == LeftRight.Left)
                return true;
            if (rightHandPose && hand.handType == LeftRight.Right)
                return true;
            return false;
        }

        // ���� ���� ��ġ���� õõ�� �̵��� �� �ɸ��� �ð� ��ȯ
        private float GetEaseInTime()
        {
            float time = 0;

            // XRGrabInteractable�� ���� ��� �� �� ���
            interactable.TryGetComponent(out XRGrabInteractable xrGrabInteractable);
            if (xrGrabInteractable)
                time = xrGrabInteractable.attachEaseInTime;

            // override �ɼ��� ���� ������ �ش� ������ �����
            if (overrideEaseTime)
                time = easeInTimeOverride;

            return time;
        }

        // �����Ϳ��� ������Ʈ�� �Ҵ���� ���� ��� �ڵ� �Ҵ� �õ�
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
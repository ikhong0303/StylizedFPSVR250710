using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MikeNspired.XRIStarterKit
{
    // Target: Ÿ�� ������Ʈ�� ���� ��� ����ϴ� ��ũ��Ʈ
    public class Target : MonoBehaviour
    {
        public UnityEventFloat onHit;  // �¾��� �� ȣ��Ǵ� �̺�Ʈ (������ ����)
        public bool canActivate;       // �ܺο��� Ȱ��ȭ ������ ��������
        public AnimateTransform animator;     // �������� ����ϴ� �ִϸ��̼� ������Ʈ
        public AnimateBounce bounceAnimation; // �¿�� ��鸮�� �ִϸ��̼� ���

        [FormerlySerializedAs("canTakeDamage")]
        public bool isActive;  // ���� Ÿ���� Ȱ��ȭ �������� (���� ���� �� �ִ� ����)

        [SerializeField] private TargetPoints[] targetPoints; // Ÿ�� ���ο� ���Ե� ���� ���� �� �ִ� ����Ʈ��
        [SerializeField] private Animator textAnimator;       // ������ �ؽ�Ʈ �ִϸ�����

        private void Start()
        {
            // �ִϸ��̼��� ������ canActivate�� true�� ����
            animator.OnFinishedAnimatingTowards.AddListener(() => canActivate = true);

            // �� Ÿ�� ����Ʈ�� �������� �޾��� �� TargetHit ȣ���ϵ��� ����
            foreach (var target in targetPoints)
                target.onHit.AddListener(TargetHit);

            // ó������ ������ �ؽ�Ʈ ��Ȱ��ȭ
            textAnimator.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            // �����Ϳ��� �� ���� �� �ڵ����� �ڽ� TargetPoints���� ã�Ƽ� �迭�� ����
            targetPoints = GetComponentsInChildren<TargetPoints>();
        }

        // �׽�Ʈ��: �������� Ÿ���� ���� ȿ���� ����
        public void TestHit()
        {
            TargetHit(1);
        }

        // Ÿ���� �¾��� �� ����Ǵ� �Լ�
        private void TargetHit(float damage)
        {
            if (!isActive) return;  // ��Ȱ��ȭ ���¸� ����

            isActive = false;       // �� �� ������ ��Ȱ��ȭ
            SetTargetPointsState(false);  // ���� ����Ʈ�� ��Ȱ��ȭ
            canActivate = false;    // �ٽ� Ȱ��ȭ���� �ʵ��� ���
            onHit.Invoke(damage);   // �ܺη� �̺�Ʈ ���� (���� �� ó����)
            animator.AnimateTo();   // Ÿ�� �ִϸ��̼�: ��
            bounceAnimation.Stop(); // ��鸮�� �ִϸ��̼� ����
            SetDamageText(damage);  // ������ �ؽ�Ʈ ǥ��
        }

        // �ܺο��� Ÿ���� Ȱ��ȭ�� �� ȣ��
        public void Activate()
        {
            SetTargetPointsState(true);  // ���� ����Ʈ Ȱ��ȭ
            isActive = true;             // Ÿ�� Ȱ��ȭ
            canActivate = false;         // ���� �ٽ� �� �°� ����
            animator.AnimateReturn();    // ���� ��ġ�� ���ƿ��� �ִϸ��̼� ����
        }

        // �¿�� ��鸮�� �ִϸ��̼� ����
        public void StartSideToSideAnimation()
        {
            bounceAnimation.StartAnimation();
        }

        // �ִϸ��̼� ���� ��� ��Ȱ��ȭ ��ġ�� ��ȯ
        public void SetToDeactivatedInstant()
        {
            SetTargetPointsState(false);
            isActive = false;
            animator.SetToEndPosition();
            bounceAnimation.Stop();
        }

        // �ִϸ��̼��� ���� ��Ȱ��ȭ ���·� ��ȯ
        public void SetToDeactivatedPosition()
        {
            SetTargetPointsState(false);
            isActive = false;
            animator.AnimateTo();
            bounceAnimation.Stop();
        }

        // ���� Ÿ�� ����Ʈ�� Ȱ��/��Ȱ�� ���� �ϰ� ����
        private void SetTargetPointsState(bool state)
        {
            foreach (var target in targetPoints)
                target.canTakeDamage = state;
        }

        // �ִϸ��̼��� ���� �ٽ� Ȱ��ȭ ���·� �̵�
        public void SetToActivatedPosition()
        {
            animator.AnimateReturn();
        }

        // �������� �ؽ�Ʈ�� ǥ��
        private void SetDamageText(float damage)
        {
            textAnimator.gameObject.SetActive(false);  // ��Ȱ��ȭ�� ���� ���� ���ٰ�
            textAnimator.gameObject.SetActive(true);   // �ٽ� Ŵ
            textAnimator.GetComponent<TextMeshPro>().text = damage.ToString(CultureInfo.InvariantCulture);
        }
    }
}
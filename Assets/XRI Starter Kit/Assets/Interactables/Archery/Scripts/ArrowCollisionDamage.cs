using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // �� ��ũ��Ʈ�� SimpleCollisionDamage�� Ȯ���Ͽ�, ȭ�� ������Ʈ�� Ưȭ�� ������ ó���� ����մϴ�.
    public class ArrowCollisionDamage : SimpleCollisionDamage
    {
        // Rigidbody�� ���� �ӵ�, �浹 � ���õ� ������Ʈ�Դϴ�.
        private Rigidbody rb;

        // �������� �� �� �ִ��� ���� (�� ���� �������� �ֱ� ���� ���)
        private bool canDamage = true;

        // ������Ʈ�� ������ �� Rigidbody�� �����ɴϴ�.
        private void Awake() => rb = GetComponent<Rigidbody>();

        // �ܺο��� �������� ������ �� �ֵ��� �� (��: ȭ���� ���� ���� ������ ����)
        public void AdjustDamage(float power) => damage *= power;

        // �������� �� ���� ���� ó��
        // SimpleCollisionDamage���� ��ӹ��� Damage �޼��带 ������(override)��
        protected override void Damage(IDamageable damageable)
        {
            // �� �� �浹�� �ڿ��� �� �̻� �������� ���� �ʵ��� ����
            if (!canDamage) return;

            // ��󿡰� �������� ����
            damageable.TakeDamage(damage, gameObject);

            // ���� �ٽ� �������� ���� �ʵ��� ����
            canDamage = false;
        }
    }
}
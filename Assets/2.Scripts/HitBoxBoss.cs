using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// ���� Ư�� ������ �ٴ� ��Ʈ�ڽ�. �������� �����ϴ� ������ �Ѵ�.
    /// </summary>
    public class HitBoxBoss : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1;
        // ������ ����. (��: �Ӹ� 2��, ���� 1�� ��)

        private BossHealth damageable;
        // ���� ü���� �����ϴ� EnemyHealth�� ���� (�θ𿡼� ������)

        private void Awake() =>
            damageable = GetComponentInParent<BossHealth>();
        // �� ��Ʈ�ڽ��� ���� ������Ʈ�� �θ� �� EnemyHealth�� ã�Ƽ� ����

        // IDamageable �������̽� ����
        public void TakeDamage(float damage, GameObject damager) =>
            damageable?.TakeDamage(damage * damageMultiplier, gameObject);
        // ���޹��� damage ���� ������ ���ؼ� ���� ü�� �ý��ۿ� ����
    }
}
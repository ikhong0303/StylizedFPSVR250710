using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// ������ ��Ʈ�ڽ�. �������� FishHealth�� �����ϴ� ������ �մϴ�.
    /// </summary>
    public class HitBoxFish : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1f;

        private FishHealth damageable;

        private void Awake() =>
            damageable = GetComponentInParent<FishHealth>();
        

        public void TakeDamage(float damage, GameObject damager)
        {
            damageable?.TakeDamage(damage * damageMultiplier, gameObject);
        }
    }
}

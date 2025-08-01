using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 물고기용 히트박스. 데미지를 FishHealth에 전달하는 역할을 합니다.
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

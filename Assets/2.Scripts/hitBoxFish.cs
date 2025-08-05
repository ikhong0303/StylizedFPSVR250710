using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 물고기용 히트박스. 데미지를 FishHealth에 전달하는 역할을 합니다.
    /// </summary>
    public class HitBoxFish : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1;


        private FishHealth damageable;


        private void Awake() =>
            damageable = GetComponentInParent<FishHealth>();



        public void TakeDamage(float damage, GameObject damager)
        {
            Debug.Log($"[HitBoxFish] 충돌 감지! 받은 damage: {damage}, FishHealth={damageable}");
            damageable?.TakeDamage(damage * damageMultiplier, gameObject);
        }
    }
}

using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 적의 특정 부위에 붙는 히트박스. 데미지를 전달하는 역할을 한다.
    /// </summary>
    public class HitBox : MonoBehaviour, IDamageable
    {
        [SerializeField] private float damageMultiplier = 1;
        // 데미지 배율. (예: 머리 2배, 몸통 1배 등)

        private EnemyHealth damageable;
        // 실제 체력을 관리하는 EnemyHealth를 참조 (부모에서 가져옴)

        private void Awake() =>
            damageable = GetComponentInParent<EnemyHealth>();
        // 이 히트박스가 붙은 오브젝트의 부모 중 EnemyHealth를 찾아서 참조

        // IDamageable 인터페이스 구현
        public void TakeDamage(float damage, GameObject damager) =>
            damageable?.TakeDamage(damage * damageMultiplier, gameObject);
        // 전달받은 damage 값에 배율을 곱해서 실제 체력 시스템에 전달
    }
}

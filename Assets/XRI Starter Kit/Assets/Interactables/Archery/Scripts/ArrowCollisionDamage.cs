using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 이 스크립트는 SimpleCollisionDamage를 확장하여, 화살 오브젝트에 특화된 데미지 처리를 담당합니다.
    public class ArrowCollisionDamage : SimpleCollisionDamage
    {
        // Rigidbody는 물리 속도, 충돌 등에 관련된 컴포넌트입니다.
        private Rigidbody rb;

        // 데미지를 줄 수 있는지 여부 (한 번만 데미지를 주기 위해 사용)
        private bool canDamage = true;

        // 오브젝트가 생성될 때 Rigidbody를 가져옵니다.
        private void Awake() => rb = GetComponent<Rigidbody>();

        // 외부에서 데미지를 조정할 수 있도록 함 (예: 화살의 힘에 따라 데미지 증가)
        public void AdjustDamage(float power) => damage *= power;

        // 데미지를 줄 때의 실제 처리
        // SimpleCollisionDamage에서 상속받은 Damage 메서드를 재정의(override)함
        protected override void Damage(IDamageable damageable)
        {
            // 한 번 충돌한 뒤에는 더 이상 데미지를 주지 않도록 차단
            if (!canDamage) return;

            // 대상에게 데미지를 전달
            damageable.TakeDamage(damage, gameObject);

            // 이후 다시 데미지를 주지 않도록 설정
            canDamage = false;
        }
    }
}
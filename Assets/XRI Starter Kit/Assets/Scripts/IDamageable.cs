using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 이 인터페이스는 '데미지를 받을 수 있는 대상'에게 구현됩니다.
    /// 예: 플레이어, 적, 파괴 가능한 오브젝트 등.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// 데미지를 적용합니다.
        /// </summary>
        /// <param name="damage">입힐 데미지 양</param>
        /// <param name="damager">공격한 주체 (총알, 검 등)</param>
        void TakeDamage(float damage, GameObject damager);
    }

    /// <summary>
    /// 이 인터페이스는 '충돌했을 때 어떤 종류의 이펙트를 발생시킬지'를 정의합니다.
    /// 예: 총알이 금속, 살점, 나무 등에 부딪혔을 때 서로 다른 이펙트 재생.
    /// </summary>
    public interface IImpactType
    {
        /// <summary>
        /// 충돌 시 어떤 종류의 표면인지 반환합니다.
        /// </summary>
        ImpactType GetImpactType();

        /// <summary>
        /// 충돌 시 이 오브젝트가 '자식으로 붙어야 하는지' 여부를 결정합니다.
        /// 예: 화살이 벽에 박히는 경우 true
        /// </summary>
        bool ShouldReparent { get; }
    }

    /// <summary>
    /// 표면의 종류를 나열한 열거형입니다.
    /// 이 값에 따라 피격 이펙트, 사운드 등을 다르게 적용할 수 있습니다.
    /// </summary>
    public enum ImpactType
    {
        Metal,   // 금속 표면
        Flesh,   // 살점 (적, 동물 등)
        Wood,    // 나무
        Neutral  // 기본값 또는 특수처리 없는 표면
    }
}
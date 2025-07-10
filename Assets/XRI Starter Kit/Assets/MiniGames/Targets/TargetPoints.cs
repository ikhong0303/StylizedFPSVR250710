using UnityEngine;
using UnityEngine.Serialization;

namespace MikeNspired.XRIStarterKit
{
    // 이 스크립트는 '타겟'의 개별 맞는 지점(예: 과녁의 중앙/가장자리 등)을 제어하는 데 사용됨
    public class TargetPoints : MonoBehaviour, IDamageable
    {
        // 맞았을 때 발생하는 이벤트 (부모 Target에서 이 이벤트를 듣고 반응함)
        public UnityEventFloat onHit;

        // 예전 이름은 damageMultiplier였던 변수. 맞았을 때 줄 점수(또는 데미지)
        [FormerlySerializedAs("damageMultiplier")]
        public float points = 1;

        // 맞았을 때 재생될 사운드 이펙트
        public AudioRandomize hitSoundEffect;

        // 현재 이 타겟이 데미지를 받을 수 있는 상태인지
        public bool canTakeDamage;

        // 데미지를 받는 인터페이스 함수 (총알 등에서 호출됨)
        public void TakeDamage(float damage, GameObject damager)
        {
            if (!canTakeDamage) return;           // 데미지 받을 수 없는 상태면 무시
            if (hitSoundEffect)
                hitSoundEffect.Play();            // 효과음 재생
            onHit.Invoke(points);                 // 점수(or 데미지) 전달
        }

        // 데미지를 전달하지 않고 단순히 타겟을 맞았다는 신호를 줄 때 사용
        public void Hit()
        {
            if (!canTakeDamage) return;
            if (hitSoundEffect)
                hitSoundEffect.Play();
            onHit.Invoke(points);
        }

        // 외부에서 타겟을 활성화할 때 호출
        public void Activate() => canTakeDamage = true;

        // 외부에서 타겟을 비활성화할 때 호출
        public void Deactivate() => canTakeDamage = false;
    }
}
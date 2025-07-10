using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 무기의 충돌 강도(속도)에 따라 데미지를 가하는 컴포넌트.
    /// 대상 오브젝트에 IDamageable이 구현되어 있어야 함.
    /// </summary>
    public class CollisionDamage : MonoBehaviour
    {
        [Header("Damage Settings")]
        [SerializeField] private float minImpactForce = 1f;     // 최소 충돌 속도 (이보다 낮으면 무시)
        [SerializeField] private float maxImpactForce = 10f;    // 최대 충돌 속도 (이 이상은 일정 데미지 고정)
        [SerializeField] private float damage = 10f;            // 최대 데미지 (maxImpactForce 시 이 수치만큼 피해)

        [SerializeField] private bool useDamageCurve = false;   // 커브를 사용할지 여부
        [SerializeField] private AnimationCurve damageCurve = AnimationCurve.Linear(0, 0, 1, 1); // 충돌 강도에 따른 데미지 커브

        [SerializeField] private float timeBetweenDamage = 0.1f; // 연속 데미지를 방지하기 위한 쿨타임

        [Header("Damageable Layers")]
        [SerializeField] private LayerMask damageableLayers;   // 데미지를 입힐 수 있는 레이어만 처리

        [Header("Debug Options")]
        [SerializeField] private bool debugVelocity = false;   // 충돌 속도 디버그 출력 여부

        private float lastDamageTime = 0f; // 마지막으로 데미지를 준 시간

        private void OnCollisionEnter(Collision collision)
        {
            // 쿨다운 안 지났으면 무시
            if (Time.time - lastDamageTime < timeBetweenDamage)
                return;

            // 충돌한 오브젝트가 damageableLayers에 속하는지 확인
            if ((damageableLayers.value & (1 << collision.gameObject.layer)) == 0)
                return;

            // 상대 속도 크기(충돌 세기) 계산
            float impactForce = collision.relativeVelocity.magnitude;

            // 디버깅용 로그 출력
            if (debugVelocity)
            {
                Debug.Log($"Collision relative velocity: {impactForce}, hit object: {collision.gameObject.name}", gameObject);
            }

            // 너무 약한 충돌이면 무시
            if (impactForce < minImpactForce)
                return;

            // 충돌 세기를 설정한 범위 내로 제한
            impactForce = Mathf.Clamp(impactForce, minImpactForce, maxImpactForce);

            // 0~1 사이로 정규화
            float normalizedForce = (impactForce - minImpactForce) / (maxImpactForce - minImpactForce);

            // 커브를 사용한다면, 해당 곡선으로 보정
            if (useDamageCurve)
                normalizedForce = damageCurve.Evaluate(normalizedForce);

            // 최종 데미지 계산
            float finalDamage = normalizedForce * damage;

            // 상대 오브젝트 또는 부모에게 IDamageable 인터페이스가 있는지 검사
            IDamageable damageable = collision.collider.GetComponent<IDamageable>()
                                     ?? collision.collider.GetComponentInParent<IDamageable>();

            // 데미지 전달
            if (damageable != null)
            {
                damageable.TakeDamage(finalDamage, gameObject);
                lastDamageTime = Time.time;
            }
        }
    }
}
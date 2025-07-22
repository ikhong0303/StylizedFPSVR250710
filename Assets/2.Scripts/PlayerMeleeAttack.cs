using MikeNspired.XRIStarterKit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // XRBaseController로 진동

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("근접 공격 설정")]
    public float attackDamage = 15f;
    public float attackCooldown = 0.5f; // 연타 방지(초)
    public LayerMask targetLayer;       // 공격 대상만 타격
    public AudioClip hitSound;
    public GameObject hitEffectPrefab;

    [Header("XR Haptic (진동)")]
    public XRBaseController controller; // 내 손(왼/오른쪽) XRBaseController 연결
    public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;

    private float lastAttackTime = -100f;

    private void OnTriggerEnter(Collider other)
    {
        // 쿨타임 체크
        if (Time.time - lastAttackTime < attackCooldown) return;

        // 공격 대상 체크 (IDamageable + LayerMask)
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        // 데미지 적용
        damageable.TakeDamage(attackDamage, gameObject);
        lastAttackTime = Time.time;

        // 효과음
        if (hitSound && Camera.main) // AudioSource가 손에 없으면, 카메라에서 재생
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);

        // 이펙트(파티클 등)
        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

        // 진동
        if (controller != null)
            controller.SendHapticImpulse(hapticAmplitude, hapticDuration);
    }
}

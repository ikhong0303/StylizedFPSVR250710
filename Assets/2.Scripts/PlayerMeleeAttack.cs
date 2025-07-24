using MikeNspired.XRIStarterKit;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("근접 공격 설정")]
    public float attackDamage = 15f;
    public float attackCooldown = 0.5f; // 연타 방지(초)
    public LayerMask targetLayer;       // 공격 대상만 타격
    public AudioClip hitSound;
    public GameObject hitEffectPrefab;

    [Header("펀치 속도 설정")]
    public float minAttackSpeed = 2.0f; // 이 속도 이상일 때만 데미지
    private Vector3 lastPosition;

    private float lastAttackTime = -100f;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // 위치를 항상 저장해둔다 (OnTriggerEnter용)
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 쿨타임 체크
        if (Time.time - lastAttackTime < attackCooldown) return;

        // 공격 대상 체크 (IDamageable + LayerMask)
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        // 속도 체크 (직전 프레임과의 거리 변화)
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        if (speed < minAttackSpeed) return;

        // 데미지 적용
        damageable.TakeDamage(attackDamage, gameObject);
        lastAttackTime = Time.time;

        // 효과음
        if (hitSound && Camera.main)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);

        // 이펙트(파티클 등)
        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
    }
}
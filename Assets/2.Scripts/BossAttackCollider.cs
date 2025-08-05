using UnityEngine;

/// <summary>
/// 보스 공격 판정 콜라이더 (양손 모두 사용 가능)
/// - 공격 애니메이션에서 AttackActivate/AttackDeactivate Animation Event로 판정 타이밍 관리
/// </summary>
public class BossAttackCollider : MonoBehaviour
{
    public float damage = 10f;


    // Animation Event에서 호출 (판정 켜기)

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어에 데미지 적용
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
            // 필요시, 한 번만 데미지 주려면 canAttack = false; 추가 가능
        }
    }
}

using UnityEngine;

/// <summary>
/// 보스 공격 판정 콜라이더 (양손 모두 사용 가능)
/// - 공격 애니메이션에서 AttackActivate/AttackDeactivate Animation Event로 판정 타이밍 관리
/// </summary>
public class BossAttackCollider : MonoBehaviour
{
    public float damage = 20f;
    public string playerTag = "Player";

    private bool canAttack = false; // 판정 타이밍 제어

    // Animation Event에서 호출 (판정 켜기)
    public void AttackActivate()
    {
        canAttack = true;
    }
    // Animation Event에서 호출 (판정 끄기)
    public void AttackDeactivate()
    {
        canAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canAttack) return;
        if (other.CompareTag(playerTag))
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

using MikeNspired.XRIStarterKit;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    public enum BossState { Idle, Walk, Attack, Death }
    public BossState state = BossState.Idle;

    public Transform player;
    public float attackDistance = 2f;
    public float attackCooldown = 1.2f;
    private float lastAttackTime = -100f;

    private float postAttackDelay = 0.2f;
    private float postAttackTimer = 0f;

    private NavMeshAgent agent;
    private BossHealth bossHealth;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bossHealth = GetComponent<BossHealth>();
        animator = GetComponent<Animator>();
        if (bossHealth != null)
            bossHealth.OnDeath += OnDeath;
    }

    void Update()
    {
        if (state == BossState.Death) return;
        if (bossHealth != null && bossHealth.IsDead)
        {
            ChangeState(BossState.Death);
            return;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case BossState.Idle:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);

                // 공격 후 딜레이 중엔 아무것도 안함
                if (postAttackTimer > 0f)
                {
                    postAttackTimer -= Time.deltaTime;
                    // 필요하면 플레이어 바라보기만 추가
                    Vector3 targetDir = player.position - transform.position;
                    targetDir.y = 0f;
                    if (targetDir.sqrMagnitude > 0.01f)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(targetDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 8f);
                    }
                    break;
                }

                if (dist > attackDistance)
                {
                    ChangeState(BossState.Walk);
                }
                else if (dist <= attackDistance && Time.time >= lastAttackTime + attackCooldown)
                {
                    ChangeState(BossState.Attack);
                }
                break;

            case BossState.Walk:
                agent.isStopped = false;
                animator.SetBool("isWalking", true);
                agent.SetDestination(player.position);

                if (postAttackTimer > 0f)
                {
                    postAttackTimer -= Time.deltaTime;
                    break;
                }

                if (dist <= attackDistance && Time.time >= lastAttackTime + attackCooldown)
                {
                    ChangeState(BossState.Attack);
                }
                else if (dist <= attackDistance)
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Attack:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                // ★ 공격 도중엔 아무것도 안 함 (상태전이 X, 이동 X, 회전 X)
                break;

            case BossState.Death:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                animator.SetBool("isDead", true);
                break;
        }
    }

    void ChangeState(BossState next)
    {
        if (state == next) return;
        state = next;

        switch (next)
        {
            case BossState.Idle:
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                break;

            case BossState.Walk:
                animator.SetBool("isWalking", true);
                agent.isStopped = false;
                break;

            case BossState.Attack:
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                int randomAttack = Random.Range(0, 3); // 0~2
                animator.SetInteger("attackIndex", randomAttack);
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                break;

            case BossState.Death:
                animator.SetBool("isDead", true);
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                break;
        }
    }

    // ★ Attack 애니메이션 마지막 프레임 AnimationEvent에서만 호출!
    public void OnAttackEnd()
    {
        if (state != BossState.Attack) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (bossHealth != null && bossHealth.IsDead)
        {
            ChangeState(BossState.Death);
            return;
        }

        // Attack → Walk or Idle (이 때 거리 체크)
        if (dist > attackDistance)
            ChangeState(BossState.Walk);
        else
            ChangeState(BossState.Idle);

        // 공격 후 딜레이
        postAttackTimer = postAttackDelay;

        // 트리거/attackIndex 리셋
        animator.ResetTrigger("Attack");
        animator.SetInteger("attackIndex", -1);
    }

    void OnDeath()
    {
        ChangeState(BossState.Death);
        Debug.Log("Boss Dead!");
    }
}

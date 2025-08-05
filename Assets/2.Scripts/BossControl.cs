using MikeNspired.XRIStarterKit;
using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    public enum BossState { Appear, Idle, Walk, Attack, React, Death }
    public BossState state = BossState.Appear;

    [Header("플레이어/에이전트")]
    public Transform player;
    public float attackCooldown = 1.2f;
    private float lastAttackTime = -100f;

    [Header("공격 패턴 설정")]
    public float[] attackDistances = new float[4] { 4.5f, 3.5f, 6f, 7f }; // 각 공격별 사거리
    [Range(0f, 1f)] public float[] attackProbabilities = new float[4] { 0.3f, 0.3f, 0.2f, 0.2f }; // 각 공격별 확률(합 1.0)
    public AudioClip[] attackVoices;
    [SerializeField] private AudioSource audioSource;

    private int nextAttackIndex = 0;

    private float postAttackDelay = 0.2f;
    private float postAttackTimer = 0f;

    public BossHealthBarUI healthBarInstance;

    private NavMeshAgent agent;
    private BossHealth bossHealth;
    private Animator animator;


    [Header("등장 연출 설정")]
    public Transform appearPoint;        // 등장 지점 (씬에서 따로 empty로 만들어서 할당)
    public float appearSpeed = 1.5f;     // 등장 연출 걷기 속도
    public float battleSpeed = 4.5f;     // 전투 시작 후 이동속도

    private bool hasRoared = false;      // Roar 애니메이션 중복방지
    private bool hasStartedAppear = false;

    [Header("React 연출")]
    [Range(0f, 1f)]
    public float reactChance = 0.1f; // Walk 상태에서 데미지 시 10% 확률로 React
    private bool isReacting = false; // React 상태 플래그

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bossHealth = GetComponent<BossHealth>();
        animator = GetComponent<Animator>();

        if (bossHealth != null)
            bossHealth.OnDeath += OnDeath;

        if (bossHealth != null)
            bossHealth.OnTakeDamage += TryReactOnDamage; // ★ 보스 피격 이벤트 연결

        if (healthBarInstance != null)
        {
            healthBarInstance.SetTarget(bossHealth, player);
            healthBarInstance.gameObject.SetActive(false); // 처음엔 비활성화 (원하는 경우)
        }

        int minLen = Mathf.Min(attackDistances.Length, attackProbabilities.Length);
        System.Array.Resize(ref attackDistances, minLen);
        System.Array.Resize(ref attackProbabilities, minLen);
        NormalizeProbabilities();
        PickNextAttack();

        // 등장상태로 시작!
        state = BossState.Appear;
        hasRoared = false;
        agent.speed = appearSpeed;
    }

    void PlayAttackVoice(int attackIndex)
    {
        if (attackVoices == null || attackVoices.Length <= attackIndex) return;
        var clip = attackVoices[attackIndex];
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    void FixedUpdate()
    {
        if (state == BossState.Death) return;
        if (bossHealth != null && bossHealth.IsDead)
        {
            ChangeState(BossState.Death);
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case BossState.Appear:
                agent.speed = appearSpeed;
                agent.isStopped = false;
                if (appearPoint != null)
                    agent.SetDestination(appearPoint.position);

                // 1회만 AppearWalk 트리거
                if (!hasStartedAppear)
                {
                    animator.SetTrigger("AppearWalk");
                    hasStartedAppear = true;
                }

                float distToAppear = Vector3.Distance(transform.position, appearPoint.position);

                // 등장 도착: Roar 트리거로 바로 전환!
                if (distToAppear < 1f && !hasRoared)
                {
                    agent.isStopped = true;
                    animator.SetTrigger("Roar");
                    animator.ResetTrigger("AppearWalk");
                    hasRoared = true;
                }
                break;

            case BossState.Idle:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);

                if (postAttackTimer > 0f)
                {
                    postAttackTimer -= Time.deltaTime;
                    // Idle 연출: 플레이어 바라보기
                    Vector3 targetDir = player.position - transform.position;
                    targetDir.y = 0f;
                    if (targetDir.sqrMagnitude > 0.01f)
                    {
                        Quaternion lookRot = Quaternion.LookRotation(targetDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 8f);
                    }
                    break;
                }

                if (distToPlayer > attackDistances[nextAttackIndex])
                {
                    ChangeState(BossState.Walk);
                }
                else if (distToPlayer <= attackDistances[nextAttackIndex] && Time.time >= lastAttackTime + attackCooldown)
                {
                    ChangeState(BossState.Attack);
                }
                break;

            case BossState.Walk:
                if (isReacting) break; // React 중엔 아무것도 안함
                agent.isStopped = false;
                animator.SetBool("isWalking", true);
                agent.SetDestination(player.position);

                if (postAttackTimer > 0f)
                {
                    postAttackTimer -= Time.deltaTime;
                    break;
                }

                if (distToPlayer <= attackDistances[nextAttackIndex] && Time.time >= lastAttackTime + attackCooldown)
                {
                    ChangeState(BossState.Attack);
                }
                else if (distToPlayer <= attackDistances[nextAttackIndex])
                {
                    ChangeState(BossState.Idle);
                }
                break;

            case BossState.Attack:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                // 공격 도중엔 아무것도 안 함!
                break;

            case BossState.React:
                agent.isStopped = true;
                animator.SetBool("isWalking", false);
                // React 애니메이션이 끝날 때까지 아무것도 안함
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
                agent.speed = battleSpeed; // ★ Idle 진입 시 속도 리셋!
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                isReacting = false;
                if (healthBarInstance != null && !healthBarInstance.gameObject.activeSelf)
                    healthBarInstance.gameObject.SetActive(true);
                break;

            case BossState.Walk:
                agent.speed = battleSpeed; // ★ Walk 진입 시 속도 리셋!
                animator.SetBool("isWalking", true);
                agent.isStopped = false;
                isReacting = false;
                break;

            case BossState.Attack:
                agent.speed = battleSpeed; // ★ Attack 진입 시 속도 리셋!
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                animator.SetInteger("attackIndex", nextAttackIndex);
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                PlayAttackVoice(nextAttackIndex);
                break;

            case BossState.React:
                // React에서는 이동 안 하니까 speed 세팅 필요 없음
                agent.isStopped = true;
                break;

            case BossState.Death:
                animator.SetBool("isDead", true);
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
                break;
        }
    }


    /// <summary>
    /// BossHealth 등에서 Walk 상태일 때 데미지 입으면 10% 확률로 React 애니메이션
    /// (delegate로 연결됨)
    /// </summary>
    public void TryReactOnDamage()
    {
        if (state == BossState.Walk && !isReacting)
        {
            if (Random.value < reactChance)
            {
                isReacting = true;
                ChangeState(BossState.React);
                animator.ResetTrigger("React"); // 혹시 모를 중복 방지(습관적으로)
                animator.SetTrigger("React");
            }
        }
    }

    /// <summary>
    /// React 애니메이션 마지막에 호출 (Animation Event)
    /// </summary>
    public void OnReactEnd()
    {
        isReacting = false;
        ChangeState(BossState.Walk);
    }

    /// <summary>
    /// Roar 애니메이션 마지막 프레임 AnimationEvent에서 호출!
    /// 등장 연출 끝 → Idle로 진입(전투 FSM 활성화)
    /// </summary>
    public void OnRoarEnd()
    {
        ChangeState(BossState.Idle);
    }

    /// <summary>
    /// Attack 애니메이션 마지막 프레임 AnimationEvent에서만 호출!
    /// </summary>
    public void OnAttackEnd()
    {
        if (state != BossState.Attack) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (bossHealth != null && bossHealth.IsDead)
        {
            ChangeState(BossState.Death);
            return;
        }

        // Attack → Walk or Idle (현재 공격 사거리 기준)
        if (dist > attackDistances[nextAttackIndex])
            ChangeState(BossState.Walk);
        else
            ChangeState(BossState.Idle);

        // 다음 공격 패턴 미리 선정
        PickNextAttack();

        postAttackTimer = postAttackDelay;
        animator.ResetTrigger("Attack");
        animator.SetInteger("attackIndex", -1);
    }

    void OnDeath()
    {
        ChangeState(BossState.Death);
        Debug.Log("Boss Dead!");
    }

    void NormalizeProbabilities()
    {
        float sum = 0f;
        for (int i = 0; i < attackProbabilities.Length; i++)
            sum += attackProbabilities[i];
        if (Mathf.Approximately(sum, 1f) || sum == 0f)
            return;
        for (int i = 0; i < attackProbabilities.Length; i++)
            attackProbabilities[i] /= sum;
    }

    void PickNextAttack()
    {
        float r = Random.value;
        float sum = 0f;
        for (int i = 0; i < attackProbabilities.Length; i++)
        {
            sum += attackProbabilities[i];
            if (r <= sum)
            {
                nextAttackIndex = i;
                return;
            }
        }
        nextAttackIndex = attackProbabilities.Length - 1;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IDamageable
{
    void TakeDamage(float damage);
}

// EnemyNav: NavMeshAgent를 이용해 목표지점까지 이동하고, 도착 시 일정 주기로 총알을 발사하는 적 AI 스크립트
public class EnemyNav : MonoBehaviour, IDamageable
{
    NavMeshAgent navMeshAgent; // 네비메시 에이전트 컴포넌트
    public GameObject player; // 플레이어 오브젝트
    float distance; // 목표지점과의 거리
    public float targetDistance = 2f; // 목표지점 도착 판정 거리
    public Transform standingPosition; // 목표지점(적이 이동할 위치)
    public Animator anim;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 5; // 최대 체력
    private float currentHealth;                 // 현재 체력
    private bool hasAimed = false;
    private bool isDead = false;   // 사망 여부를 추적하는 변수


    [Header("References")]
    public Animator Deatanimator; // 죽음 애니메이션용 (선택사항)
    public GameObject deathEffect; // 죽었을 때 이펙트 프리팹 (선택사항)

    // 아주 단순한 총알 오브젝트 풀
    private List<GameObject> bulletPool = new List<GameObject>();

    // 도착 후 경과 시간 측정용 타이머
    private float arrivedTimer = 0f;
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint;     // 총알이 발사될 위치(총구 등)
    public float bulletForce = 500f; // AddForce에 사용할 힘

    // 컴포넌트 초기화
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // IDamageable 인터페이스 구현
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // 피격 애니메이션 (선택)
        if (Deatanimator != null)
        {
            Deatanimator.SetTrigger("Hit");
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // 죽음 애니메이션 실행
        if (Deatanimator != null)
        {
            Deatanimator.SetTrigger("Die");
        }

        // 이펙트 생성
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 일정 시간 후 삭제 (애니메이션용 딜레이)
        Destroy(gameObject, 5f);
    }


    // 물리 프레임마다 호출
    private void FixedUpdate()
    {
        // 목표지점과 현재 위치의 거리 계산
        distance = Vector3.Distance(standingPosition.position, transform.position);
        anim.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if (navMeshAgent.isStopped == true)
        {   
            // 멈춰있을 때 플레이어를 바라봄
            Vector3 lookPos = player.transform.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            if (!hasAimed)
            {
      
                hasAimed = true;

            }
        }

        // 목표지점에 도착하지 않았으면 이동
        if (distance > targetDistance)
        {
            navMeshAgent.isStopped = false;
            Movement();
            arrivedTimer = 0f; // 이동 중에는 타이머 초기화
            anim.SetBool("Aiming", false);

        }
        else // 목표지점에 도착했을 때
        {
            navMeshAgent.isStopped = true;
            anim.SetBool("Aiming",true);
    

            arrivedTimer += Time.fixedDeltaTime;
            if (arrivedTimer >= 2f) // 2초마다 실행
            {
                Debug.Log("목표지점에 도착한지 2초 경과");
                arrivedTimer = 0f;

                // 풀에서 비활성화된 총알 찾기
                GameObject bullet = null;
                foreach (var b in bulletPool)
                {
                    if (!b.activeInHierarchy)
                    {
                        bullet = b;
                        break;
                    }
                }
                // 비활성화된 총알이 없으면 새로 생성해서 풀에 추가
                if (bullet == null)
                {
                    bullet = Instantiate(bulletPrefab);
                    bulletPool.Add(bullet);
                }

                // 총알 위치/회전 세팅 및 활성화
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;
                bullet.SetActive(true);


            }
        }
    }

    // NavMeshAgent를 이용해 목표지점으로 이동
    void Movement()
    {
        navMeshAgent.SetDestination(standingPosition.position);
    }
}

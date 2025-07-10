using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace MikeNspired.XRIStarterKit
{
    // 좀비 캐릭터 AI를 제어하는 클래스
    public class EnemyControl : MonoBehaviour, IEnemy
    {
        [Header("References")]
        [SerializeField] private EnemyHealth enemyHealth; // 체력 관리 컴포넌트
        [SerializeField] private NPCSoundController soundController; // 사운드 재생 컨트롤러
        [SerializeField] private DamageText damageText; // 데미지 텍스트 프리팹 (ex. "10" 같은 숫자 뜨는 것)
        [SerializeField] private Transform damageTextSpawn; // 데미지 텍스트가 생성될 위치
        [SerializeField] private Animator animator; // 애니메이터 컴포넌트
        [SerializeField] private GameObject player; // 플레이어 오브젝트


        [Header("Combat")]
        //[SerializeField] private float attackRange = 0.5f; // 공격 가능한 거리
        [SerializeField] private float screamChance = 0.05f; // 접근 중 비명을 지를 확률
        [SerializeField] private float hitAnimationChance = 50f; // 피격 시 리액션 애니메이션 확률
        [SerializeField] private float targetDistance = 2f; // 목표지점 도착 판정 거리
        [SerializeField] private Transform movePosition; // 목표지점(적이 이동할 위치)
        private float arrivedTimer = 0f;


        float distance; // 목표지점과의 거리

        private bool hasAimed = false;

        private NavMeshAgent navMeshAgent;
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletForce = 500f;
        private List<GameObject> bulletPool = new List<GameObject>();
        public GameObject deathEffect; // 죽었을 때 이펙트 프리팹 (선택사항)

        // 내부 상태 제어 변수
        private bool willScream;
        private bool hasScreamed;
        private float initialDistanceToPlayer;

        private float accelerateTimer;
        private bool isDead = false;
        private bool isAttacking;
        private bool isEmerging;
        private bool isSinking;

        // 애니메이터 파라미터 해시 (성능 최적화용)
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Scream = Animator.StringToHash("Scream");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Attack = Animator.StringToHash("Attack");
        //private static readonly int DissolveAmountHash = Shader.PropertyToID("_DissolveAmount");

        // 전역 이벤트 (다른 시스템에서 구독 가능)
        public static event Action<EnemyControl> OnZombieDied;
        public static event Action OnZombieAttacked;




        private void Start()
        {
            // 플레이어를 XR Origin으로 자동 탐색
            navMeshAgent = GetComponent<NavMeshAgent>();

            // 데미지를 받았을 때 사운드 재생 및 리액션 설정
            if (enemyHealth != null)
            {
                enemyHealth.OnTakeDamage += _ => soundController.PlayImpact();
                enemyHealth.OnTakeDamage += OnEnemyTakeDamage;
            }
        }

        private void FixedUpdate()
        {
            // 목표지점과 현재 위치의 거리 계산
            distance = Vector3.Distance(movePosition.position, transform.position);
            animator.SetFloat("speed", navMeshAgent.velocity.magnitude);
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
                animator.SetBool("Aiming", false);

            }
            else // 목표지점에 도착했을 때
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("Aiming", true);


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

                    // 총알 발사(Bullet 스크립트의 Fire 호출)
                    EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.Fire(firePoint.forward, bulletForce);
                    }
                }
            }
        }


        #region Initialization & Emergence
        void Movement()
        {
            navMeshAgent.SetDestination(movePosition.position);
        }

        // 좀비 초기화 (속도, 가속도 설정 및 등장 시작)
        public void Initialize(float speed, float timeToSpeed)
        {
            //maxSpeed = speed;
            //timeToMaxSpeed = timeToSpeed;

            // 비명을 지를 확률 결정
            willScream = UnityEngine.Random.value <= screamChance;
            hasScreamed = false;
            isAttacking = false;

        }
        #endregion




        #region Death & Damage

        // 외부에서 호출: 좀비 사망 처리
        public void Die()
        {
            if (isDead) return;

            isDead = true;

        // 죽음 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 이펙트 생성
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // 일정 시간 후 삭제 (애니메이션용 딜레이)
        Destroy(gameObject, 5f);
        }

        // 데미지를 받았을 때 반응
        private void OnEnemyTakeDamage(float damage)
        {
            if (isDead) return;

            // 데미지 숫자 생성
            Instantiate(damageText, damageTextSpawn.position, Quaternion.identity, damageTextSpawn)
                .SetText(damage.ToString("f1"));

            // 일정 확률로 피격 리액션 재생
            if (UnityEngine.Random.value <= hitAnimationChance)
                animator.SetTrigger(Hit);


        }


        // 디졸브(녹아내림) 후 삭제
        public void FadeAndDestroy()
        {
            StopAllCoroutines();
            //StartCoroutine(AnimateAndDestroy());
        }


        #endregion

        #region Debug

#if UNITY_EDITOR
        // 에디터에서 공격 범위 시각화
        //private void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, attackRange);
        //}
#endif

        #endregion
    }



    // 좀비 인터페이스 (사망 기능만 정의됨)
    public interface IEnemy
    {
        void Die();
    }


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 적(좀비) AI 행동 제어 스크립트
    /// - 지정 지점까지 이동 → 도착 후 조준/사격 반복
    /// - 사격 전 조준 레이저 연출 포함
    /// </summary>
    public class EnemyControl : MonoBehaviour, IEnemy
    {
        [Header("References")]
        [SerializeField] private EnemyHealth enemyHealth;                  // 체력 관리 컴포넌트
        [SerializeField] private NPCSoundController soundController;       // 사운드 재생 컨트롤러
        [SerializeField] private DamageText damageText;                    // 데미지 텍스트 프리팹
        [SerializeField] private Transform damageTextSpawn;                // 데미지 텍스트 스폰 위치
        [SerializeField] private Animator animator;                        // 애니메이터
        [SerializeField] private GameObject player;                        // 플레이어 오브젝트
        [SerializeField] private SkinnedMeshRenderer bodyRenderer;         // 본체 메쉬 렌더러


        [Header("Combat")]
        [SerializeField] private float screamChance = 0.05f;               // 비명 확률
        [SerializeField] private float hitAnimationChance = 50f;           // 피격 리액션 확률
        [SerializeField] private float targetDistance = 2f;                // 목표 도달 거리
        [SerializeField] private Transform movePosition;                   // 이동 목표 지점
        private float arrivedTimer = 0f;

        float distance;                            // 목표와의 거리
        private NavMeshAgent navMeshAgent;         // 네비게이션 에이전트(이동용)
        public GameObject bulletPrefab;            // 적 총알 프리팹
        public Transform firePoint;                // 총구 위치
        public float bulletForce = 500f;           // 총알 힘
        private List<GameObject> bulletPool = new List<GameObject>();   // 총알 풀
        public GameObject laserTubePrefab;         // 원통(프리팹)
        private GameObject activeLaserTube;

        private Material hitFlashMaterial;         // 피격 플래시 머티리얼
        private Coroutine hitFlashRoutine;         // 피격 코루틴

        // --- 조준 레이저 ---
        [Header("Aim Laser")]
        public LineRenderer aimLaser;              // 조준 레이저(LineRenderer)
        public Color laserColor = Color.red;       // 레이저 색상
        public float laserWidth = 0.05f;           // 레이저 굵기
        public float laserLength = 30f;            // 레이저 길이

        // --- 상태머신 ---
        enum EnemyState { Move, AimPrepare, AimLaser, Shoot }
        EnemyState currentState = EnemyState.Move;
        float stateTimer = 0f;                     // 상태 타이머
        Vector3 aimDirection = Vector3.forward;    // 사격 방향

        private bool isDead = false;               // 사망 여부

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            // 조준 레이저 기본 설정
            if (aimLaser != null)
            {
                aimLaser.enabled = false;
                aimLaser.startColor = laserColor;
                aimLaser.endColor = laserColor;
                aimLaser.startWidth = laserWidth;
                aimLaser.endWidth = laserWidth;
            }
            // 피격 이벤트 연결
            if (enemyHealth != null)
            {
                enemyHealth.OnTakeDamage += _ => soundController.PlayImpact();
                enemyHealth.OnTakeDamage += OnEnemyTakeDamage;
            }
        }

        /// <summary>
        /// 상태머신 메인 루프 (FixedUpdate)
        /// </summary>
        private void FixedUpdate()
        {
            if (isDead) return;

            switch (currentState)
            {
                case EnemyState.Move:
                    UpdateMove();
                    break;
                case EnemyState.AimPrepare:
                    UpdateAimPrepare();
                    break;
                case EnemyState.AimLaser:
                    UpdateAimLaser();
                    break;
                case EnemyState.Shoot:
                    UpdateShoot();
                    break;
            }
        }

        // -------------------------------
        // 상태별 함수
        // -------------------------------

        /// <summary>
        /// 1. 목표지점까지 이동 (도달하면 AimPrepare로 전환)
        /// </summary>
        void UpdateMove()
        {
            distance = Vector3.Distance(movePosition.position, transform.position);
            animator.SetFloat("speed", navMeshAgent.velocity.magnitude);

            if (distance > targetDistance)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(movePosition.position);
                arrivedTimer = 0f;
                animator.SetBool("Aiming", false);
            }
            else
            {
                // 도착 순간!
                navMeshAgent.isStopped = true;
                animator.SetBool("Aiming", true);

                animator.SetFloat("speed", 0f); // ← 이 한줄 추가! (Idle 트리거)

                // 정지 후 1초 준비
                stateTimer = 0f;
                currentState = EnemyState.AimPrepare;
            }
        }

        /// <summary>
        /// 2. 도착 후 1초 동안 조준 준비(플레이어 바라보기)
        /// </summary>
        void UpdateAimPrepare()
        {
            animator.SetBool("Aiming", true);
            LookAtPlayer();
            stateTimer += Time.fixedDeltaTime;
            if (stateTimer >= 1f)
            {
                // 1초 후, 조준 방향 고정 & 레이저 표시
                aimDirection = (player.transform.position - firePoint.position).normalized;
                ShowAimLaser(aimDirection, true);
                stateTimer = 0f;
                currentState = EnemyState.AimLaser;
            }
        }

        /// <summary>
        /// 3. 1초간 레이저 표시(조준 고정), 이후 사격
        /// </summary>
        void UpdateAimLaser()
        {
            animator.SetBool("Aiming", true);
            ShowAimLaser(aimDirection, true);
            stateTimer += Time.fixedDeltaTime;
            if (stateTimer >= 1f)
            {
                ShowAimLaser(aimDirection, false);
                FireAtDirection(aimDirection);
                stateTimer = 0f;
                currentState = EnemyState.Shoot;
            }
        }

        /// <summary>
        /// 4. 사격 후 0.5초 대기, 이후 다시 이동 (Move)
        /// </summary>
        void UpdateShoot()
        {
            animator.SetBool("Aiming", true); // ✅ 계속 조준 유지!

            stateTimer += Time.fixedDeltaTime;
            if (stateTimer >= 0.5f)
            {
                navMeshAgent.isStopped = false;
                currentState = EnemyState.Move;
            }
        }

        // -------------------------------
        // 유틸 함수
        // -------------------------------

        /// <summary>
        /// 플레이어 바라보기 (Y축 고정)
        /// </summary>
        void LookAtPlayer()
        {
            Vector3 lookPos = player.transform.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }

        /// <summary>
        /// 조준 레이저 on/off 및 위치 설정
        /// </summary>
        void ShowAimLaser(Vector3 dir, bool show)
        {
            float playerDist = Vector3.Distance(firePoint.position, player.transform.position);
            float offset = 0.3f;
            float laserDist = Mathf.Max(0.1f, playerDist - offset);

            if (show)
            {
                if (activeLaserTube == null)
                    activeLaserTube = Instantiate(laserTubePrefab);

                activeLaserTube.SetActive(true);

                // 위치: firePoint에서 dir 방향으로 laserDist*0.5 만큼 이동 (Cylinder 중심)
                activeLaserTube.transform.position = firePoint.position + dir * (laserDist * 0.5f);

                // 이 부분이 핵심! → Cylinder의 Y축(위)을 dir로!
                activeLaserTube.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

                // 스케일: Y축(길이/2)
                activeLaserTube.transform.localScale = new Vector3(
                    0.04f,            // X축(굵기)
                    laserDist * 0.5f, // Y축(길이의 절반, Cylinder는 중앙 pivot)
                    0.04f             // Z축(굵기)
                );
            }
            else
            {
                if (activeLaserTube != null)
                    activeLaserTube.SetActive(false);
            }
        }

        /// <summary>
        /// 사격(총알 풀에서 발사)
        /// </summary>
        void FireAtDirection(Vector3 dir)
        {
            // 풀에서 비활성화 총알 찾기
            GameObject bullet = null;
            foreach (var b in bulletPool)
            {
                if (!b.activeInHierarchy)
                {
                    bullet = b;
                    break;
                }
            }
            if (bullet == null)
            {
                bullet = Instantiate(bulletPrefab);
                bulletPool.Add(bullet);
            }

            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.LookRotation(dir);
            bullet.SetActive(true);

            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
                bulletScript.Fire(dir, bulletForce);
        }

        // -------------------------------
        // 데미지/사망 처리 (기존과 동일)
        // -------------------------------
        #region Death & Damage

        /// <summary>
        /// 사망 처리(애니메이션/이펙트 후 삭제)
        /// </summary>
        public void Die()
        {
            if (isDead) return;
            isDead = true;

            if (animator != null)
                animator.SetTrigger("Die");
            Destroy(gameObject, 5f);
        }

        /// <summary>
        /// 피격 시 리액션/데미지 텍스트/플래시
        /// </summary>
        private void OnEnemyTakeDamage(float damage)
        {
            if (isDead) return;

            Instantiate(damageText, damageTextSpawn.position, Quaternion.identity, damageTextSpawn)
                .SetText(damage.ToString("f1"));
            if (UnityEngine.Random.value <= hitAnimationChance)
                animator.SetTrigger("Hit");
            FlashHitColor(0.1f);
        }

        /// <summary>
        /// 피격 시 색상 플래시
        /// </summary>
        public void FlashHitColor(float duration = 0.2f)
        {
            if (bodyRenderer == null) return;
            if (hitFlashRoutine != null)
                StopCoroutine(hitFlashRoutine);
            hitFlashRoutine = StartCoroutine(HitFlashRoutine(duration));
        }

        private IEnumerator HitFlashRoutine(float duration)
        {
            if (hitFlashMaterial == null)
                hitFlashMaterial = bodyRenderer.material;
            hitFlashMaterial.color = new Color(2f, 0f, 0f);
            yield return new WaitForSeconds(duration);
            hitFlashMaterial.color = Color.white;
        }

        public void FadeAndDestroy()
        {
            StopAllCoroutines();
        }

        #endregion

#if UNITY_EDITOR
        //private void OnDrawGizmosSelected() { ... }
#endif
    }

    /// <summary>
    /// IEnemy 인터페이스 (사망 기능만)
    /// </summary>
    public interface IEnemy { void Die(); }
}

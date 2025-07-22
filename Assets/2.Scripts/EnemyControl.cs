using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 적(좀비) AI 행동 제어 스크립트 (Cylinder 레이저만 사용, SetActive 방식)
    /// </summary>
    public class EnemyControl : MonoBehaviour, IEnemy
    {
        [Header("References")]
        [SerializeField] private EnemyHealth enemyHealth;
        [SerializeField] private NPCSoundController soundController;
        [SerializeField] private DamageText damageText;
        [SerializeField] private Transform damageTextSpawn;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject player;
        [SerializeField] private SkinnedMeshRenderer bodyRenderer;

        [Header("Combat")]
        [SerializeField] private float screamChance = 0.05f;
        [SerializeField] private float hitAnimationChance = 50f;
        [SerializeField] private float targetDistance = 2f;
        [SerializeField] private Transform movePosition;
        private float arrivedTimer = 0f;

        float distance;
        private NavMeshAgent navMeshAgent;
        public GameObject bulletPrefab;
        public Transform firePoint;
        public float bulletForce = 500f;
        private List<GameObject> bulletPool = new List<GameObject>();
        public GameObject laserTubePrefab;
        private GameObject activeLaserTube;

        private Material hitFlashMaterial;
        private Coroutine hitFlashRoutine;

        // --- 상태머신 ---
        enum EnemyState { Move, AimPrepare, AimLaser, Shoot }
        EnemyState currentState = EnemyState.Move;
        float stateTimer = 0f;
        Vector3 aimDirection = Vector3.forward;

        private bool isDead = false;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            // 피격 이벤트 연결
            if (enemyHealth != null)
            {
                enemyHealth.OnTakeDamage += _ => soundController.PlayImpact();
                enemyHealth.OnTakeDamage += OnEnemyTakeDamage;
            }

            // 시작 시 레이저 비활성화
            if (activeLaserTube != null)
                activeLaserTube.SetActive(false);
        }

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
                navMeshAgent.isStopped = true;
                animator.SetBool("Aiming", true);
                animator.SetFloat("speed", 0f);

                stateTimer = 0f;
                currentState = EnemyState.AimPrepare;
            }
        }

        void UpdateAimPrepare()
        {
            animator.SetBool("Aiming", true);
            LookAtPlayer();
            stateTimer += Time.fixedDeltaTime;
            if (stateTimer >= 1f)
            {
                aimDirection = (player.transform.position - firePoint.position).normalized;
                ShowAimLaser(aimDirection, true);
                stateTimer = 0f;
                currentState = EnemyState.AimLaser;
            }
        }

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

        void UpdateShoot()
        {
            animator.SetBool("Aiming", true);
            stateTimer += Time.fixedDeltaTime;
            if (stateTimer >= 0.5f)
            {
                navMeshAgent.isStopped = false;
                currentState = EnemyState.Move;
            }
        }

        void LookAtPlayer()
        {
            Vector3 lookPos = player.transform.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }

        // Cylinder 조준 레이저 SetActive 방식
        void ShowAimLaser(Vector3 dir, bool show)
        {
            if (isDead)
            {
                if (activeLaserTube != null)
                    activeLaserTube.SetActive(false);
                return;
            }

            float playerDist = Vector3.Distance(firePoint.position, player.transform.position);
            float offset = 0.3f;
            float laserDist = Mathf.Max(0.1f, playerDist - offset);

            if (show)
            {
                if (activeLaserTube == null)
                    activeLaserTube = Instantiate(laserTubePrefab);

                if (!activeLaserTube.activeSelf)
                    activeLaserTube.SetActive(true);

                activeLaserTube.transform.position = firePoint.position + dir * (laserDist * 0.5f);
                activeLaserTube.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
                activeLaserTube.transform.localScale = new Vector3(
                    0.04f,
                    laserDist * 0.5f,
                    0.04f
                );
            }
            else
            {
                if (activeLaserTube != null && activeLaserTube.activeSelf)
                    activeLaserTube.SetActive(false);
            }
        }

        void FireAtDirection(Vector3 dir)
        {
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

        #region Death & Damage

        public void Die()
        {
            if (isDead) return;
            isDead = true;

            // 이동 즉시 멈춤
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = true;
                navMeshAgent.enabled = false;
            }

            // 조준 레이저 즉시 비활성화!
            if (activeLaserTube != null)
                activeLaserTube.SetActive(false);

            if (animator != null)
                animator.SetTrigger("Die");
            Destroy(gameObject, 5f);
        }

        private void OnEnemyTakeDamage(float damage)
        {
            if (isDead) return;

            Instantiate(damageText, damageTextSpawn.position, Quaternion.identity, damageTextSpawn)
                .SetText(damage.ToString("f1"));
            if (UnityEngine.Random.value <= hitAnimationChance)
                animator.SetTrigger("Hit");
            FlashHitColor(0.1f);
        }

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

    public interface IEnemy { void Die(); }
}

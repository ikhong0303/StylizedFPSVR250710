using UnityEngine;
using System;
using System.Collections;
using Unity.XR.CoreUtils;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 좀비 AI 스크립트. 등장, 추적, 공격, 피격 반응, 사망 애니메이션 및 사라지는 연출까지 포함.
    /// </summary>
    public class Zombie : MonoBehaviour
    {
        #region Fields

        [Header("Health & Effects")]
        [SerializeField] private EnemyHealth enemyHealth;           // 체력 시스템
        [SerializeField] private NPCSoundController soundController; // 사운드 컨트롤러
        [SerializeField] private DamageText damageText;             // 데미지 텍스트 프리팹
        [SerializeField] private Transform damageTextSpawn;         // 데미지 텍스트 위치



        [SerializeField] private Animator animator;                 // 애니메이터

        [Header("Other Settings")]
        [SerializeField] private float attackRange = 0.5f;          // 공격 거리
        [SerializeField] private float hitAnimationChance = 0.5f;   // 히트 애니메이션 확률

        // 내부 상태 변수들
        private bool hasScreamed, isDead, isAttacking;
        private Transform player;

        // 애니메이터 해시값들
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Attack = Animator.StringToHash("Attack");


        #endregion

        #region Unity Methods

        private void Start()
        {
            // 플레이어 탐색 (VR 환경용)
            player = FindFirstObjectByType<XROrigin>().transform;

            // 데미지 받을 때 반응 설정
            if (enemyHealth != null)
            {
                enemyHealth.OnTakeDamage += _ => soundController.PlayImpact();
                enemyHealth.OnTakeDamage += OnEnemyTakeDamage;
            }
        }


        #endregion


        #region Death & Damaged


        private void OnEnemyTakeDamage(float damage)
        {
            if (isDead) return;

            // 데미지 텍스트 생성
            Instantiate(damageText, damageTextSpawn.position, Quaternion.identity, damageTextSpawn)
                .SetText(damage.ToString("f1"));

            if (UnityEngine.Random.value <= hitAnimationChance)
                animator.SetTrigger(Hit);
        }



        #endregion

#if UNITY_EDITOR
#endif
    }
}

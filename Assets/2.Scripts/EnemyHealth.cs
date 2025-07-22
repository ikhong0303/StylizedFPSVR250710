using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 적의 체력을 관리하고, 데미지를 받고 사망 처리를 하는 컴포넌트.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100; // 최대 체력
        [SerializeField] private float currentHealth; // 현재 체력

        private IEnemy enemy; // Die()를 호출하기 위한 인터페이스 참조

        public event Action<float> OnTakeDamage; // 데미지를 받았을 때 발생하는 이벤트 (UI/사운드 등에 사용)

        public float MaxHealth => maxHealth; // 외부에서 최대 체력 접근용 프로퍼티

        [SerializeField] private float damageCooldown = 0.1f; // 데미지를 연속으로 받지 않게 하는 쿨타임
        private float lastDamageTime;                          // 마지막으로 데미지를 받은 시간 기록

        private void Awake()
        {
            currentHealth = maxHealth;            // 체력 초기화
            enemy = GetComponent<IEnemy>();       // 이 오브젝트에서 Die()를 실행할 인터페이스 가져오기
            lastDamageTime = -damageCooldown;     // 시작하자마자 바로 피격 가능하게 설정
        }


        /// <summary>
        /// 외부에서 데미지를 가할 때 호출하는 함수.
        /// </summary>
        /// <param name="damage">가할 데미지 값</param>
        /// <param name="damager">공격한 오브젝트 (사용 안 하지만 확장 가능)</param>
        public void TakeDamage(float damage, GameObject damager)
        {
            // 쿨다운이 지나지 않았다면 무시
            if (Time.time - lastDamageTime < damageCooldown)
                return;

            lastDamageTime = Time.time;

            // UI 등에서 사용 가능한 데미지 알림
            OnTakeDamage?.Invoke(damage);

            // 체력 감소
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // 체력이 0 이하이면 사망 처리
            if (currentHealth <= 0)
                enemy.Die();
        }

        /// <summary>
        /// 외부에서 최대 체력을 새로 설정.
        /// </summary>
        public void SetMaxHealth(float newMaxHealth)
        {
            maxHealth = Mathf.Max(0, newMaxHealth);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        /// <summary>
        /// 외부에서 현재 체력을 설정.
        /// </summary>
        public void SetCurrentHealth(float newHealth)
        {
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        }

        /// <summary>
        /// 체력을 회복할 때 사용.
        /// </summary>
    }
}
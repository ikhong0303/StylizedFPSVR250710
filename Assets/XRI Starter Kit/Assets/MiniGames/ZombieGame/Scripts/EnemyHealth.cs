using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 적의 체력을 관리하고, 데미지를 받고 사망 처리를 하는 컴포넌트.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 5; // 최대 체력
        private float currentHealth;                 // 현재 체력


        public event Action<float> OnTakeDamage; // 데미지를 받았을 때 발생하는 이벤트 (UI/사운드 등에 사용)


        [SerializeField] private float damageCooldown = 0.1f; // 데미지를 연속으로 받지 않게 하는 쿨타임
        private float lastDamageTime;                          // 마지막으로 데미지를 받은 시간 기록


        /// <summary>
        /// 디버깅용 함수. 임의로 5 데미지를 입힘.
        /// </summary>
        public void TestDamage()
        {
            TakeDamage(5, gameObject);
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
        //public void SetCurrentHealth(float newHealth)
        //{
        //    currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        //}

        /// 체력을 회복할 때 사용.
    }
}

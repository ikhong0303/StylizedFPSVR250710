using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class BossHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 500f;
        [SerializeField] private float currentHealth;

        [SerializeField] private float damageCooldown = 0.1f;
        private float lastDamageTime;
        public event Action OnTakeDamage;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;

        public event Action OnDeath; // 죽음 이벤트

        private bool isDead = false;
        public bool IsDead => isDead;

        [Header("테스트용 (Inspector 체크박스)")]
        public bool testTakeDamage = false;
        public float testDamageAmount = 10f;

        private void Awake()
        {
            currentHealth = maxHealth;
            lastDamageTime = -damageCooldown;
        }

        private void FixedUpdate()
        {
            if (testTakeDamage)
            {
                testTakeDamage = false;
                TakeDamage(testDamageAmount, null);
            }
        }

        public void TakeDamage(float damage, GameObject damager)
        {
            if (isDead) return;

            if (Time.time - lastDamageTime < damageCooldown)
                return;

            lastDamageTime = Time.time;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnTakeDamage?.Invoke(); // ★ 이 줄 추가!

            if (currentHealth <= 0f)
                Die();
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            OnDeath?.Invoke();
        }
    }
}

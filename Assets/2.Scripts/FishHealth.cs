using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class FishHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 500f;
        [SerializeField] private float currentHealth;

        [SerializeField] private float damageCooldown = 0.1f;
        private float lastDamageTime;

        public event Action OnDeath; // 죽음 이벤트

        private bool isDead = false;

        [Header("테스트용")]
        [SerializeField] private bool takeDamageTest = false;
        [SerializeField] private float testDamageAmount = 10f;

        private void Awake()
        {
            currentHealth = maxHealth;
            lastDamageTime = -damageCooldown;
        }

        private void Update()
        {
            if (takeDamageTest)
            {
                TakeDamage(testDamageAmount, null);
                takeDamageTest = false; // 호출 후 자동 해제
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

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

        public event Action OnDeath; // Á×À½ ÀÌº¥Æ®

        private bool isDead = false;

        private void Awake()
        {
            currentHealth = maxHealth;
            lastDamageTime = -damageCooldown;
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

using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class FishHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [SerializeField] private float damageCooldown = 0.1f;
        private float lastDamageTime;

        [SerializeField] private FishFlashEffect flashEffect;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        public event Action OnDeath;

        private bool isDead = false;

        [Header("테스트용")]
        [SerializeField] private bool takeDamageTest = false;
        [SerializeField] private float testDamageAmount = 10f;

        private void Awake()
        {
            currentHealth = maxHealth;
            lastDamageTime = -damageCooldown;

            // 자동 할당
            if (flashEffect == null)
                flashEffect = GetComponent<FishFlashEffect>();
        }

        private void FixedUpdate()
        {
            if (takeDamageTest)
            {
                TakeDamage(testDamageAmount, null);
                takeDamageTest = false;
            }
        }

        public void TakeDamage(float damage, GameObject damager)
        {

            Debug.Log($"[FishHealth] 실제 체력 감소 호출! damage: {damage} (Before: {currentHealth})");
            if (isDead) return;
            if (Time.time - lastDamageTime < damageCooldown)
                return;

            lastDamageTime = Time.time;
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            // ★ 피격 플래시
            if (flashEffect != null)
                flashEffect.Flash();

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

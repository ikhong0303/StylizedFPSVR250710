using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<float> OnTakeDamage; // 데미지 받으면(실제값) 알림
    public event Action OnDie;               // 사망시 알림 (필요시)

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // 외부에서 체력 깎기
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnTakeDamage?.Invoke(damage);

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        // 필요하면 OnHeal 이벤트도 만들 수 있음
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDie?.Invoke();
        // 여기서 사망 연출/애니메이션/컨트롤 제한 등
        Debug.Log("플레이어 사망!");
    }

    // 외부에서 현재 체력 읽기
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
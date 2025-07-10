using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public event Action<float> OnTakeDamage; // ������ ������(������) �˸�
    public event Action OnDie;               // ����� �˸� (�ʿ��)

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // �ܺο��� ü�� ���
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
        // �ʿ��ϸ� OnHeal �̺�Ʈ�� ���� �� ����
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDie?.Invoke();
        // ���⼭ ��� ����/�ִϸ��̼�/��Ʈ�� ���� ��
        Debug.Log("�÷��̾� ���!");
    }

    // �ܺο��� ���� ü�� �б�
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
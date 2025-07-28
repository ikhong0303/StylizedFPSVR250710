using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] private float damageCooldown = 0.1f; // �������� �������� ���� �ʰ� �ϴ� ��Ÿ��
    private float lastDamageTime;
    [SerializeField] private PlayerHitEffect hitEffect; // �� Inspector���� ���� �Ҵ�

    public event Action<float> OnTakeDamage; // ������ ������(������) �˸�
    public event Action OnDie;               // ����� �˸� (�ʿ��)

    private bool isDead = false;

    private void Start()
    {
        GameManager.Instance.player = this.gameObject;
        currentHealth = maxHealth;
        lastDamageTime = -damageCooldown;     // �������ڸ��� �ٷ� �ǰ� �����ϰ� ����
        //hitEffect = GetComponent<PlayerHitEffect>();

    }

    // �ܺο��� ü�� ���
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // ��ٿ��� ������ �ʾҴٸ� ����
        if (Time.time - lastDamageTime < damageCooldown)
            return;
         
        lastDamageTime = Time.time;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (hitEffect != null)
            hitEffect.ShowHitEffect();

        OnTakeDamage?.Invoke(damage);
        Debug.Log("������");
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
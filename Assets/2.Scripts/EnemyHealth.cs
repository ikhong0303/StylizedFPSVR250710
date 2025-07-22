using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// ���� ü���� �����ϰ�, �������� �ް� ��� ó���� �ϴ� ������Ʈ.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100; // �ִ� ü��
        [SerializeField] private float currentHealth; // ���� ü��

        private IEnemy enemy; // Die()�� ȣ���ϱ� ���� �������̽� ����

        public event Action<float> OnTakeDamage; // �������� �޾��� �� �߻��ϴ� �̺�Ʈ (UI/���� � ���)

        public float MaxHealth => maxHealth; // �ܺο��� �ִ� ü�� ���ٿ� ������Ƽ

        [SerializeField] private float damageCooldown = 0.1f; // �������� �������� ���� �ʰ� �ϴ� ��Ÿ��
        private float lastDamageTime;                          // ���������� �������� ���� �ð� ���

        private void Awake()
        {
            currentHealth = maxHealth;            // ü�� �ʱ�ȭ
            enemy = GetComponent<IEnemy>();       // �� ������Ʈ���� Die()�� ������ �������̽� ��������
            lastDamageTime = -damageCooldown;     // �������ڸ��� �ٷ� �ǰ� �����ϰ� ����
        }


        /// <summary>
        /// �ܺο��� �������� ���� �� ȣ���ϴ� �Լ�.
        /// </summary>
        /// <param name="damage">���� ������ ��</param>
        /// <param name="damager">������ ������Ʈ (��� �� ������ Ȯ�� ����)</param>
        public void TakeDamage(float damage, GameObject damager)
        {
            // ��ٿ��� ������ �ʾҴٸ� ����
            if (Time.time - lastDamageTime < damageCooldown)
                return;

            lastDamageTime = Time.time;

            // UI ��� ��� ������ ������ �˸�
            OnTakeDamage?.Invoke(damage);

            // ü�� ����
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            // ü���� 0 �����̸� ��� ó��
            if (currentHealth <= 0)
                enemy.Die();
        }

        /// <summary>
        /// �ܺο��� �ִ� ü���� ���� ����.
        /// </summary>
        public void SetMaxHealth(float newMaxHealth)
        {
            maxHealth = Mathf.Max(0, newMaxHealth);
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        /// <summary>
        /// �ܺο��� ���� ü���� ����.
        /// </summary>
        public void SetCurrentHealth(float newHealth)
        {
            currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        }

        /// <summary>
        /// ü���� ȸ���� �� ���.
        /// </summary>
    }
}
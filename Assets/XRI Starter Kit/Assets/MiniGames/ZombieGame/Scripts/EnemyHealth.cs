using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// ���� ü���� �����ϰ�, �������� �ް� ��� ó���� �ϴ� ������Ʈ.
    /// </summary>
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 5; // �ִ� ü��
        private float currentHealth;                 // ���� ü��


        public event Action<float> OnTakeDamage; // �������� �޾��� �� �߻��ϴ� �̺�Ʈ (UI/���� � ���)


        [SerializeField] private float damageCooldown = 0.1f; // �������� �������� ���� �ʰ� �ϴ� ��Ÿ��
        private float lastDamageTime;                          // ���������� �������� ���� �ð� ���


        /// <summary>
        /// ������ �Լ�. ���Ƿ� 5 �������� ����.
        /// </summary>
        public void TestDamage()
        {
            TakeDamage(5, gameObject);
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
        //public void SetCurrentHealth(float newHealth)
        //{
        //    currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        //}

        /// ü���� ȸ���� �� ���.
    }
}

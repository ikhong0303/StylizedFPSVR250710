using UnityEngine;
using System.Collections;


    public class EnemyBullet : MonoBehaviour
    {    /// <summary>
         /// �Ѿ��� ������ ����� ������ �߻��Ѵ�.
         /// </summary>
         /// <param name="direction">�߻� ����</param>
         /// <param name="force">�߻� ��</param>
        public void Fire(Vector3 direction, float force)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // �߻� �� ������ ���� �ӵ� �ʱ�ȭ
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // ������ �������� ���� ���� �Ѿ��� �߻�
                rb.AddForce(direction * force);
            }

            // ���� �ð� �� �Ѿ��� ��Ȱ��ȭ�ϴ� �ڷ�ƾ ����
            StartCoroutine(DisableAfterSeconds(2f));
        }

        /// <summary>
        /// ���� �ð� �Ŀ� �Ѿ� ������Ʈ�� ��Ȱ��ȭ�Ѵ� (������Ʈ Ǯ�� �뵵).
        /// </summary>
        /// <param name="seconds">��Ȱ��ȭ���� ����� �ð� (��)</param>
        IEnumerator DisableAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false); // ������Ʈ�� ��Ȱ��ȭ (Destroy ���)
        }
    }

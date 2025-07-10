using UnityEngine;
using System.Collections;


    public class EnemyBullet : MonoBehaviour
    {    /// <summary>
         /// 총알을 지정된 방향과 힘으로 발사한다.
         /// </summary>
         /// <param name="direction">발사 방향</param>
         /// <param name="force">발사 힘</param>
        public void Fire(Vector3 direction, float force)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 발사 전 기존의 물리 속도 초기화
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // 지정된 방향으로 힘을 가해 총알을 발사
                rb.AddForce(direction * force);
            }

            // 일정 시간 후 총알을 비활성화하는 코루틴 실행
            StartCoroutine(DisableAfterSeconds(2f));
        }

        /// <summary>
        /// 일정 시간 후에 총알 오브젝트를 비활성화한다 (오브젝트 풀링 용도).
        /// </summary>
        /// <param name="seconds">비활성화까지 대기할 시간 (초)</param>
        IEnumerator DisableAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            gameObject.SetActive(false); // 오브젝트를 비활성화 (Destroy 대신)
        }
    }

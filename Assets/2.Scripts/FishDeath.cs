using System.Collections;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class FishDeath : MonoBehaviour
    {
        [SerializeField] private FishHealth fishHealth;
        [SerializeField] private Animator animator;
        // [SerializeField] private Rigidbody rb;   // ← 완전 삭제

        // 연출 파라미터
        public float deathRotateZ = 90f;
        public float rotateDuration = 1.0f;
        public float floatHeight = 0.15f;
        public int floatCount = 3;
        public float floatSpeed = 0.4f;
        public float sinkDuration = 2.0f;
        public float sinkDistance = 1.0f;
        public event System.Action OnSinkStart;

        private void Awake()
        {
            if (fishHealth != null)
                fishHealth.OnDeath += OnFishDeath;
        }
        private void OnDestroy()
        {
            if (fishHealth != null)
                fishHealth.OnDeath -= OnFishDeath;
        }

        private void OnFishDeath()
        {
            StartCoroutine(DeathRoutine());
        }

        private IEnumerator DeathRoutine()
        {
            AudioManager3.Instance.PlaySFX("Fish_Death"); // 물고기 죽음 사운드 재생
            if (animator != null)
            {
                animator.SetTrigger("Die");    // Death 빈 상태로 이동
                yield return new WaitForSeconds(1f);
                animator.enabled = false;      // 완전히 꺼서 이후 연출 제어
            }
            // Rigidbody 관련 코드 완전 제거!
            // if (rb != null) { ... } ← 통째로 삭제

            BgmManager.Instance.StopBGM(); // BGM 정지
            // Z축 회전 (동일)
            Quaternion startRot = transform.rotation;
            Quaternion endRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, deathRotateZ);
            float t = 0f;
            while (t < rotateDuration)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRot, endRot, t / rotateDuration);
                yield return null;
            }
            transform.rotation = endRot;

            yield return new WaitForSeconds(0.5f);

            // Y축 float 연출 (이 부분 수정!)
            for (int i = 0; i < floatCount; i++)
            {
                Vector3 upPos = transform.position + Vector3.up * floatHeight;
                Vector3 downPos = transform.position;

                // 위로 이동
                t = 0f;
                while (t < floatSpeed)
                {
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(downPos, upPos, t / floatSpeed);
                    yield return null;
                }
                transform.position = upPos;

                // 아래로 이동
                t = 0f;
                while (t < floatSpeed)
                {
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(upPos, downPos, t / floatSpeed);
                    yield return null;
                }
                transform.position = downPos;
            }

            OnSinkStart?.Invoke(); // 가라앉기 시작 알림(구독자 호출)
            // 가라앉기 (동일)
            Vector3 sinkTarget = transform.position + Vector3.down * sinkDistance;
            t = 0f;
            Vector3 currPos = transform.position;
            while (t < sinkDuration)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(currPos, sinkTarget, t / sinkDuration);
                yield return null;
            }
            transform.position = sinkTarget;

            // 필요하면 여기서 사라짐/파티클 추가 가능
        }
    }
}

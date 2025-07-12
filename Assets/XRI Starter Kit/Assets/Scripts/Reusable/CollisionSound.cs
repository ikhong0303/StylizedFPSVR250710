using UnityEngine;
using Random = UnityEngine.Random;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 이 스크립트를 붙인 오브젝트가 부딪힐 때, 효과음을 자동으로 재생한다!
    /// (충돌 세기에 따라 소리 크기/피치/클립 등을 자동 조절)
    /// </summary>
    public class CollisionSound : MonoBehaviour
    {
        // [Inspector에서 세팅]
        [SerializeField] private AudioClip[] Clips = null;      // 부딪힐 때 재생할 효과음 클립 리스트
        [SerializeField] private AudioSource audioSource = null; // 실제 소리를 내는 AudioSource 컴포넌트
        [SerializeField] private AnimationCurve volumeCurve = null; // (선택) 소리 크기 변화 커브
        [SerializeField] private bool randomizePitch = false, useVolumeCurve;
        [SerializeField] private float minPitchChange = -.1f, maxPitchChange = .1f; // 랜덤 피치 변화 범위
        [SerializeField] private float maxVolume = 1;         // 최대 볼륨값
        [SerializeField] private float timeTillCanPlayAgain = .1f; // 연속 충돌 시 소리 재생 쿨타임(초)
        [SerializeField] private float maxVelocity = 5;       // 볼륨 계산용 최대 속도

        private float originalPitch;      // AudioSource 원래 피치 값 저장용
        private float timer;              // 마지막 재생 시각 저장 (쿨타임 체크)

        private void Awake()
        {
            OnValidate(); // audioSource 자동 연결(없으면 본인에게서 찾기)
            CheckValid(); // audioSource 없으면 스크립트 꺼버림
            originalPitch = audioSource.pitch; // 원래 피치값 저장
        }

        private void OnValidate()
        {
            // Editor에서 값 바뀔 때마다 실행됨 (AudioSource 없으면 본인에서 찾아봄)
            if (!audioSource)
                audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision other)
        {
            // 레벨이 시작된 직후(1초 이내)에는 효과음 안 나옴 (초기 충돌 무시)
            if (Time.timeSinceLevelLoad < 1.0f)
                return;

            // 아직 쿨타임이 안 지났으면 효과음 안 나옴 (너무 자주 재생 방지)
            if (Time.time - timer < timeTillCanPlayAgain) return;
            timer = Time.time; // 마지막 재생 시각 갱신

            // 랜덤 피치 사용시, 피치값을 랜덤으로 변경
            if (randomizePitch)
            {
                audioSource.pitch = originalPitch;
                audioSource.pitch += Random.Range(minPitchChange, maxPitchChange);
            }

            // 충돌 속도에 따라 볼륨 계산 (속도→0~1로 정규화→볼륨 계산)
            var volume = Remap(Mathf.Clamp(other.relativeVelocity.magnitude, 0, maxVelocity), 0, maxVelocity, 0, 1);

            // 볼륨 커브 사용시 커브로 변환 (없으면 그냥 볼륨값)
            volume = useVolumeCurve ?
                Remap(volumeCurve.Evaluate(volume), 0, 1, 0, maxVolume) :
                Remap(volume, 0, 1, 0, maxVolume);

            // 효과음 클립 중 랜덤으로 하나 선택
            AudioClip randomClip = Clips[Random.Range(0, Clips.Length)];
            audioSource.clip = randomClip;
            audioSource.volume = volume;

            // 효과음 재생!
            audioSource.Play();
        }

        // 값의 범위를 0~1에서 원하는 값으로 변환
        private float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        // AudioSource가 없으면 스크립트 작동 중지 (경고도 출력)
        private void CheckValid()
        {
            if (audioSource != null) return;
            Debug.LogWarning("Collision sound does not have audio source on : " + gameObject);
            enabled = false;
        }
    }
}

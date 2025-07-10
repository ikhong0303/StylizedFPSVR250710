using System.Collections;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 권총 발사 시 슬라이더(총 몸체 움직이는 부분)와 해머(뇌관을 치는 부품)의 애니메이션을 구현하는 스크립트입니다.
    /// GunCocking과 연동되어 슬라이더 움직임과 해머 위치를 조정합니다.
    /// </summary>
    public class HandGunFireAnimation : MonoBehaviour
    {
        [SerializeField] private GunCocking gunCocking = null; // 장전 슬라이드 컴포넌트 참조
        [SerializeField] private float movePositionAnimationTime = 0.03f; // 애니메이션 시간 (짧게 설정됨)

        [SerializeField] private Transform slider = null; // 슬라이드 모델 (움직이는 부분)
        [SerializeField] private Transform sliderGoalPosition = null; // 슬라이드가 이동할 목표 위치 (좌표 참조용)

        [SerializeField] private Transform hammer = null; // 해머 모델 (타격 부품)
        [SerializeField] private Transform hammerOpen = null; // 해머가 열렸을 때의 위치/회전 참조

        private Vector3 hammerStartPosition; // 해머의 원래 위치 저장
        private Quaternion hammerStartRotation; // 해머의 원래 회전값 저장

        private void Start()
        {
            // 시작 시 해머의 초기 위치와 회전을 저장
            hammerStartPosition = hammer.transform.localPosition;
            hammerStartRotation = hammer.transform.localRotation;
        }

        /// <summary>
        /// 슬라이더를 앞뒤로 빠르게 움직이는 애니메이션을 실행합니다 (발사 시 효과).
        /// </summary>
        private IEnumerator MoveSlider(Transform mover, Transform goalPosition)
        {
            float timer = 0;

            SetKeyBangerClosed(); // 해머를 닫은 상태로 설정

            // 슬라이드를 앞으로 이동
            while (timer <= movePositionAnimationTime)
            {
                var newPosition = Vector3.Lerp(gunCocking.GetStartPoint(), gunCocking.GetEndPoint(), timer / movePositionAnimationTime);
                mover.localPosition = newPosition;
                timer += Time.deltaTime;
                yield return null;
            }

            SetKeyBangerOpen(); // 해머를 열림 상태로 설정

            timer = 0;
            // 슬라이드를 원래 위치로 복귀
            while (timer <= movePositionAnimationTime + Time.deltaTime)
            {
                var newPosition = Vector3.Lerp(gunCocking.GetEndPoint(), gunCocking.GetStartPoint(), timer / movePositionAnimationTime);
                mover.localPosition = newPosition;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 슬라이더를 한 방향으로만 당기는 애니메이션을 실행합니다 (슬라이더 강제 오픈용).
        /// </summary>
        private IEnumerator OpenSlider(Transform mover, Transform goalPosition)
        {
            var startingPosition = mover.localPosition;
            float timer = 0;

            SetKeyBangerClosed(); // 해머 닫기

            // 슬라이드를 앞으로 당김
            while (timer <= movePositionAnimationTime + Time.deltaTime)
            {
                var newPosition = Vector3.Lerp(startingPosition, gunCocking.GetEndPoint(), timer / movePositionAnimationTime);
                mover.localPosition = newPosition;
                timer += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// 발사 시 슬라이더를 애니메이션처럼 앞뒤로 움직이게 함
        /// </summary>
        public void AnimateSliderOnFire() => StartCoroutine(MoveSlider(slider, sliderGoalPosition));

        /// <summary>
        /// 외부에서 슬라이더를 강제로 열린 상태로 설정할 때 사용
        /// </summary>
        public void SetSliderOpen()
        {
            gunCocking.Pause(); // GunCocking의 자동 복귀 애니메이션 멈춤
            StopAllCoroutines(); // 기존 애니메이션 중단
            StartCoroutine(OpenSlider(slider, sliderGoalPosition));
        }

        /// <summary>
        /// 해머를 열림 상태(타격 후 상태)로 설정
        /// </summary>
        public void SetKeyBangerOpen()
        {
            hammer.transform.position = hammerOpen.transform.position;
            hammer.transform.rotation = hammerOpen.transform.rotation;
        }

        /// <summary>
        /// 해머를 닫힌 상태(정위치)로 되돌림
        /// </summary>
        public void SetKeyBangerClosed()
        {
            hammer.transform.localPosition = hammerStartPosition;
            hammer.transform.localRotation = hammerStartRotation;
        }
    }
}
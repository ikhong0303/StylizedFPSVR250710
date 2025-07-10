using System.Collections;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class AnimateBounce : MonoBehaviour // 오브젝트를 두 위치 사이에서 반복적으로 튕기듯이 이동시키는 컴포넌트
    {
        [SerializeField] private Transform MovingObject = null; // 움직일 대상 오브젝트 (이것이 움직입니다)
        [SerializeField] private Transform firstPosition = null; // 첫 번째 위치 (처음에 이동할 위치)
        [SerializeField] private Transform secondPosition = null; // 두 번째 위치 (다시 돌아올 위치)
        private TransformStruct endingTransform; // 마지막에 도달한 위치 저장용 (현재 사용되지 않음)
        public bool animatePosition = true;// 위치 애니메이션을 할 것인지 여부
        public bool animateRotation = true;// 회전 애니메이션을 할 것인지 여부
        public float animateTime = .1f;// 한 번 이동하는 데 걸리는 시간

        public void Stop()// 현재 실행 중인 애니메이션을 모두 중단시킵니다.
        {
            StopAllCoroutines();
        }


        public void StartAnimation()// 애니메이션을 시작합니다.
        {
            StopAllCoroutines();
            StartCoroutine(Animate());
        }


        private IEnumerator Animate()// 실제 애니메이션을 처리하는 반복 루프입니다.
        {
            TransformStruct startingPosition;
            startingPosition.position = MovingObject.localPosition;
            startingPosition.rotation = MovingObject.localRotation;
            float timer = 0;

            // 첫 번째 위치로 부드럽게 이동
            while (timer <= animateTime)
            {
                var newPosition = Vector3.Lerp(startingPosition.position, firstPosition.localPosition, timer / animateTime);
                var newRotation = Quaternion.Lerp(startingPosition.rotation, firstPosition.localRotation, timer / animateTime);

                if (animatePosition)
                    MovingObject.localPosition = newPosition;
                if (animateRotation)
                    MovingObject.localRotation = newRotation;

                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            startingPosition.position = MovingObject.localPosition;
            startingPosition.rotation = MovingObject.localRotation;
            timer = 0;
            while (timer <= animateTime)
            {
                var newPosition = Vector3.Lerp(startingPosition.position, secondPosition.localPosition, timer / animateTime);
                var newRotation = Quaternion.Lerp(startingPosition.rotation, secondPosition.localRotation, timer / animateTime);

                if (animatePosition)
                    MovingObject.localPosition = newPosition;
                if (animateRotation)
                    MovingObject.localRotation = newRotation;

                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }

            StartCoroutine(Animate());
        }
    }
}
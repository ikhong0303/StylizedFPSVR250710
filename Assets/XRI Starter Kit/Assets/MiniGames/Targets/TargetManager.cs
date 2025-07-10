using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MikeNspired.XRIStarterKit
{
    public class TargetManager : MonoBehaviour
    {
        // 타겟들이 들어있는 부모 오브젝트
        public Transform targetParent;
        // 씬에 있는 모든 Target 스크립트를 담을 리스트
        public List<Target> targets;

        // 각 난이도별 타겟 배치 위치
        public Transform levelZero;
        public List<Transform> targetPositionsLevelZero;
        public Transform levelOne;
        public List<Transform> targetPositionsLevelOne;
        public Transform levelTwo;
        public List<Transform> targetPositionsLevelTwo;

        // HUD (점수, 타이머 등 보여주는 UI)
        public CanvasGroup headsUpDisplay;

        // 제한 시간
        public float timer = 60;

        // 게임 상태 변수
        public bool isGameActive = false;
        public int difficulty = 0;

        // 타겟 위치 이동 애니메이션에 걸리는 시간
        private float movePositionAnimationTime = 1;

        // 게임 시간 및 점수 저장용 ScriptableObject
        public FloatSO gameTimer;
        public FloatSO totalTargetsHit;

        private void Start()
        {
            // 자식들 중 Target 컴포넌트 찾아 리스트에 저장
            targetParent.GetComponentsInChildren<Target>(targets);

            // 각 레벨에서 타겟 위치 리스트 추출 (자신 제외)
            levelZero.GetComponentsInChildren<Transform>(targetPositionsLevelZero);
            targetPositionsLevelZero.Remove(targetPositionsLevelZero[0]); // 첫 번째는 부모 자신

            levelOne.GetComponentsInChildren<Transform>(targetPositionsLevelOne);
            targetPositionsLevelOne.Remove(targetPositionsLevelOne[0]);

            levelTwo.GetComponentsInChildren<Transform>(targetPositionsLevelTwo);
            targetPositionsLevelTwo.Remove(targetPositionsLevelTwo[0]);

            // 각 타겟에 onHit 이벤트 연결
            foreach (var target in targets)
            {
                target.onHit.AddListener(TargetHit);
            }

            // 게임 시작 전에는 UI 숨김
            headsUpDisplay.alpha = 0;
        }

        // 난이도(레벨)을 변경할 때 호출됨
        public void ChangeGame(int x)
        {
            difficulty = x;

            // 게임이 진행 중이면 중단
            if (isGameActive)
            {
                foreach (var target in targets)
                {
                    target.canActivate = false;
                    target.SetToDeactivatedPosition();
                }

                isGameActive = false;
            }

            StopAllCoroutines(); // 모든 코루틴 정지

            // 각 난이도에 따라 타겟 위치 이동
            switch (x)
            {
                case 0:
                    MoveToPositions(targetPositionsLevelZero);
                    break;
                case 1:
                    MoveToPositions(targetPositionsLevelOne);
                    break;
                default:
                    MoveToPositions(targetPositionsLevelTwo);
                    break;
            }
        }

        // 타겟들을 주어진 위치 리스트로 이동시킴
        private void MoveToPositions(List<Transform> list)
        {
            GetComponent<AudioSource>().Play(); // 효과음 재생
            for (var i = 0; i < list.Count; i++)
            {
                StartCoroutine(MoveToPosition(targets[i].transform, list[i]));
            }
        }

        // 타겟 하나를 애니메이션으로 새로운 위치로 이동시키는 코루틴
        private IEnumerator MoveToPosition(Transform mover, Transform goalPosition)
        {
            var startingPosition = mover.position;
            float timer = 0;
            while (timer <= movePositionAnimationTime)
            {
                var newPosition = Vector3.Lerp(startingPosition, goalPosition.position, timer / movePositionAnimationTime);
                mover.position = newPosition;
                timer += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }

        // 게임 시작 시 호출
        public void StartGame()
        {
            StopAllCoroutines();             // 기존 진행 중인 동작 초기화
            headsUpDisplay.alpha = 1;        // HUD 표시
            timer = 60;                      // 제한 시간 초기화
            totalTargetsHit.SetValue(0);     // 점수 초기화
            gameTimer.SetValue(timer);
            isGameActive = true;

            // 모든 타겟 비활성화 후 초기화
            foreach (var target in targets)
            {
                target.canActivate = true;
                target.SetToDeactivatedInstant();
            }

            // 무작위 타겟 2개 활성화
            StartCoroutine(ActivateAnotherTarget());
            StartCoroutine(ActivateAnotherTarget());
        }

        // 활성화 코루틴용 변수 (사용은 안 하는 듯)
        private Coroutine activateTarget;

        private void Update()
        {
            if (!isGameActive) return;

            // 타이머 감소
            timer -= Time.deltaTime;
            gameTimer.SetValue(timer);

            if (timer <= 0)
            {
                gameTimer.SetValue(0);
                StopAllCoroutines();
                StartCoroutine(CheckGameOver());
            }
        }

        // 타겟이 맞았을 때 실행됨
        private void TargetHit(float damage)
        {
            // 점수 증가
            totalTargetsHit.SetValue(totalTargetsHit.GetValue() + damage);

            // 다음 타겟 하나 더 활성화
            StartCoroutine(ActivateAnotherTarget());
        }

        // 랜덤한 타겟 하나를 활성화하는 코루틴
        private IEnumerator ActivateAnotherTarget()
        {
            int random = Random.Range(0, targets.Count);

            // 아직 활성화 불가능한 타겟이면 다른 타겟으로 변경
            while (!targets[random].canActivate)
            {
                random = Random.Range(0, targets.Count);
                yield return null;
            }

            // 타겟 활성화
            targets[random].Activate();

            // 어려운 난이도일 경우 추가 애니메이션 실행
            if (difficulty == 2)
                targets[random].StartSideToSideAnimation();
        }

        // 게임 종료 처리
        private IEnumerator CheckGameOver()
        {
            isGameActive = false;

            // 모든 타겟 비활성화
            foreach (var target in targets)
            {
                target.canActivate = false;
                target.SetToDeactivatedPosition();
            }

            // 5초 후 UI 숨김
            yield return new WaitForSeconds(5);
            headsUpDisplay.alpha = 0;
        }
    }
}
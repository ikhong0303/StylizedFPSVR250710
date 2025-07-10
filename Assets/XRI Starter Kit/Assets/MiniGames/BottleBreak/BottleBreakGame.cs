using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    public class BottleBreakGame : MonoBehaviour
    {
        // 병 타겟들이 배치될 부모 오브젝트
        public Transform targetParent;

        // 병 타겟 스포너들 (BottleTargetSpawner 스크립트가 붙어 있는 오브젝트들)
        public List<BottleTargetSpawner> targets;

        // 레벨별 병 타겟 위치 기준 오브젝트들
        public Transform levelZero;
        public List<Transform> targetPositionsLevelZero;
        public Transform levelOne;
        public List<Transform> targetPositionsLevelOne;
        public Transform levelTwo;
        public List<Transform> targetPositionsLevelTwo;

        // 조명들 (게임 시작 시 켜지고, 종료 시 꺼짐)
        private Light[] spotLights;

        // 게임 HUD (UI 투명도 조절용)
        public CanvasGroup headsUpDisplay;

        // 남은 시간
        public float timer = 60;

        // 게임이 진행 중인지 여부
        public bool isGameActive = false;

        // 병 타겟들이 움직이는 애니메이션 시간
        private float movePositionAnimationTime = 1;

        // 남은 시간을 외부 UI와 공유하기 위한 ScriptableObject
        public FloatSO gameTimer;

        // 맞춘 타겟 수를 저장하는 ScriptableObject
        public FloatSO totalTargetsHit;

        private void Start()
        {
            // 자식 오브젝트 중 BottleTargetSpawner를 가진 오브젝트들을 가져옴
            targetParent.GetComponentsInChildren<BottleTargetSpawner>(targets);

            // 각 병 타겟이 부서졌을 때 호출될 이벤트 등록
            foreach (var target in targets)
                target.OnBottleBroke.AddListener(TargetHit);

            // 조명 초기화 및 끄기
            spotLights = GetComponentsInChildren<Light>();
            foreach (var light in spotLights)
                light.enabled = false;

            // UI 숨기기
            headsUpDisplay.alpha = 0;

            // 각 레벨의 병 위치 기준 Transform 리스트 구성
            levelZero.GetComponentsInChildren<Transform>(targetPositionsLevelZero);
            targetPositionsLevelZero.Remove(targetPositionsLevelZero[0]); // 부모 제거

            levelOne.GetComponentsInChildren<Transform>(targetPositionsLevelOne);
            targetPositionsLevelOne.Remove(targetPositionsLevelOne[0]);

            levelTwo.GetComponentsInChildren<Transform>(targetPositionsLevelTwo);
            targetPositionsLevelTwo.Remove(targetPositionsLevelTwo[0]);
        }

        // 게임 모드를 변경하는 함수 (Unity 이벤트에서 슬라이더로 호출됨)
        public void ChangeGame(int x)
        {
            // 기존 게임 종료
            if (isGameActive)
            {
                isGameActive = false;
                foreach (var light in spotLights)
                    light.enabled = false;
            }

            // 기존 코루틴 정지
            StopAllCoroutines();

            // 선택한 난이도에 따라 병 위치 이동
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

        // 타겟들을 해당 위치로 이동시키는 함수
        private void MoveToPositions(List<Transform> list)
        {
            GetComponent<AudioSource>().Play(); // 사운드 재생
            for (var i = 0; i < list.Count; i++)
            {
                StartCoroutine(MoveToPosition(targets[i].transform, list[i]));
            }
        }

        // 부드럽게 지정된 위치로 이동하는 코루틴
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

        // 게임 시작
        public void StartGame()
        {
            StopAllCoroutines();

            headsUpDisplay.alpha = 1; // UI 표시
            timer = 60;
            totalTargetsHit.SetValue(0);
            gameTimer.SetValue(timer);
            isGameActive = true;

            foreach (var light in spotLights)
                light.enabled = true;

            // TODO: 조명이 켜지는 효과음 추가 가능
        }

        private void Update()
        {
            if (!isGameActive) return;

            // 시간 감소
            timer -= Time.deltaTime;
            gameTimer.SetValue(timer);

            // 시간이 0이 되면 게임 종료
            if (timer <= 0)
            {
                isGameActive = false;
                gameTimer.SetValue(0);
                StopAllCoroutines();
                StartCoroutine(CheckGameOver());

                foreach (var light in spotLights)
                    light.enabled = false;
            }
        }

        // 병 타겟이 맞았을 때 호출됨
        private void TargetHit()
        {
            if (!isGameActive) return;

            float totalTargets = totalTargetsHit.GetValue() + 1;
            totalTargetsHit.SetValue(totalTargets);
        }

        // 게임 종료 후 HUD 숨기기
        private IEnumerator CheckGameOver()
        {
            yield return new WaitForSeconds(5);
            headsUpDisplay.alpha = 0;
        }
    }
}
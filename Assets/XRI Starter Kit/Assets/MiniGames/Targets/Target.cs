using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace MikeNspired.XRIStarterKit
{
    // Target: 타겟 오브젝트에 대한 제어를 담당하는 스크립트
    public class Target : MonoBehaviour
    {
        public UnityEventFloat onHit;  // 맞았을 때 호출되는 이벤트 (데미지 전달)
        public bool canActivate;       // 외부에서 활성화 가능한 상태인지
        public AnimateTransform animator;     // 움직임을 담당하는 애니메이션 컴포넌트
        public AnimateBounce bounceAnimation; // 좌우로 흔들리는 애니메이션 담당

        [FormerlySerializedAs("canTakeDamage")]
        public bool isActive;  // 현재 타겟이 활성화 상태인지 (공격 받을 수 있는 상태)

        [SerializeField] private TargetPoints[] targetPoints; // 타겟 내부에 포함된 하위 맞출 수 있는 포인트들
        [SerializeField] private Animator textAnimator;       // 데미지 텍스트 애니메이터

        private void Start()
        {
            // 애니메이션이 끝나면 canActivate를 true로 설정
            animator.OnFinishedAnimatingTowards.AddListener(() => canActivate = true);

            // 각 타겟 포인트에 데미지를 받았을 때 TargetHit 호출하도록 연결
            foreach (var target in targetPoints)
                target.onHit.AddListener(TargetHit);

            // 처음에는 데미지 텍스트 비활성화
            textAnimator.gameObject.SetActive(false);
        }

        private void OnValidate()
        {
            // 에디터에서 값 변경 시 자동으로 자식 TargetPoints들을 찾아서 배열에 넣음
            targetPoints = GetComponentsInChildren<TargetPoints>();
        }

        // 테스트용: 수동으로 타겟을 맞춘 효과를 실행
        public void TestHit()
        {
            TargetHit(1);
        }

        // 타겟이 맞았을 때 실행되는 함수
        private void TargetHit(float damage)
        {
            if (!isActive) return;  // 비활성화 상태면 무시

            isActive = false;       // 한 번 맞으면 비활성화
            SetTargetPointsState(false);  // 하위 포인트도 비활성화
            canActivate = false;    // 다시 활성화되지 않도록 잠금
            onHit.Invoke(damage);   // 외부로 이벤트 전달 (점수 등 처리용)
            animator.AnimateTo();   // 타겟 애니메이션: 들어감
            bounceAnimation.Stop(); // 흔들리는 애니메이션 중지
            SetDamageText(damage);  // 데미지 텍스트 표시
        }

        // 외부에서 타겟을 활성화할 때 호출
        public void Activate()
        {
            SetTargetPointsState(true);  // 하위 포인트 활성화
            isActive = true;             // 타겟 활성화
            canActivate = false;         // 아직 다시 못 맞게 설정
            animator.AnimateReturn();    // 원래 위치로 돌아오는 애니메이션 실행
        }

        // 좌우로 흔들리는 애니메이션 시작
        public void StartSideToSideAnimation()
        {
            bounceAnimation.StartAnimation();
        }

        // 애니메이션 없이 즉시 비활성화 위치로 전환
        public void SetToDeactivatedInstant()
        {
            SetTargetPointsState(false);
            isActive = false;
            animator.SetToEndPosition();
            bounceAnimation.Stop();
        }

        // 애니메이션을 통해 비활성화 상태로 전환
        public void SetToDeactivatedPosition()
        {
            SetTargetPointsState(false);
            isActive = false;
            animator.AnimateTo();
            bounceAnimation.Stop();
        }

        // 하위 타겟 포인트의 활성/비활성 상태 일괄 설정
        private void SetTargetPointsState(bool state)
        {
            foreach (var target in targetPoints)
                target.canTakeDamage = state;
        }

        // 애니메이션을 통해 다시 활성화 상태로 이동
        public void SetToActivatedPosition()
        {
            animator.AnimateReturn();
        }

        // 데미지를 텍스트로 표시
        private void SetDamageText(float damage)
        {
            textAnimator.gameObject.SetActive(false);  // 재활성화를 위해 먼저 껐다가
            textAnimator.gameObject.SetActive(true);   // 다시 킴
            textAnimator.GetComponent<TextMeshPro>().text = damage.ToString(CultureInfo.InvariantCulture);
        }
    }
}
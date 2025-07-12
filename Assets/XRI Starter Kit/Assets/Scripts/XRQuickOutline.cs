using MikeNspired.XRIStarterKit.ChrisNolet; // Outline 스크립트(외부 코드) 상속용
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// XR(VR)에서 “오브젝트에 손을 가까이 가져가면(hover)”
    /// 오브젝트에 “윤곽선(Outline)” 효과를 자동으로 켜주고,
    /// 잡거나(Select) 손을 떼면 효과를 끄는 스크립트!
    /// </summary>
    public class XRQuickOutline : Outline // Outline(외부 라이브러리/기본 효과) 상속!
    {
        [SerializeField] private XRBaseInteractable _baseInteractable; // 손/컨트롤러가 상호작용할 오브젝트
        [SerializeField] private bool onlyHighlightsWhenNotSelected;   // 잡고 있는 동안은 아웃라인 꺼둘지
        private Color startingColor;                                   // 아웃라인 원래 색 저장

        // 에디터에서 값 바뀔 때마다 실행. _baseInteractable이 없으면 자동 연결
        private void OnValidate()
        {
            if (!_baseInteractable)
                _baseInteractable = GetComponentInParent<XRBaseInteractable>();
        }

        // 시작 시점에 (게임 시작)
        private void Start()
        {
            OnValidate(); // interactable 자동 연결
            startingColor = OutlineColor; // 원래 윤곽선 색 저장

            // XR 상호작용 이벤트 연결
            _baseInteractable.hoverEntered.AddListener(OnHoverEnter); // 손/컨트롤러가 가까이 오면
            _baseInteractable.hoverExited.AddListener(OnHoverExit);   // 멀어지면
            _baseInteractable.selectEntered.AddListener(OnSelectEnter); // 잡았을 때
            _baseInteractable.selectExited.AddListener(OnSelectExit);   // 손에서 뗄 때

            enabled = false; // 기본적으로 윤곽선 비활성화
        }

        // 오브젝트가 파괴될 때 이벤트 해제
        private void OnDestroy()
        {
            if (_baseInteractable != null)
            {
                _baseInteractable.hoverEntered.RemoveListener(OnHoverEnter);
                _baseInteractable.hoverExited.RemoveListener(OnHoverExit);
                _baseInteractable.selectEntered.RemoveListener(OnSelectEnter);
                _baseInteractable.selectExited.RemoveListener(OnSelectExit);
            }
        }

        // 이벤트 핸들러: 손이 가까이 오면 아웃라인 켜기
        private void OnHoverEnter(HoverEnterEventArgs args) => Highlight(args);
        // 손이 멀어지면 아웃라인 끄기
        private void OnHoverExit(HoverExitEventArgs args) => StopHighlight();
        // 잡으면(Select) 아웃라인 끄기
        private void OnSelectEnter(SelectEnterEventArgs args) => StopHighlight();

        // 손을 놓았을 때, 아직 Hover 중이면 다시 윤곽선 켜기
        private void OnSelectExit(SelectExitEventArgs args)
        {
            if (_baseInteractable.isHovered)
                Highlight(null);
        }

        // 윤곽선(Outline) 켜기 (손 Hover, 직접 호출 등)
        public void Highlight(HoverEnterEventArgs args)
        {
            if (onlyHighlightsWhenNotSelected && _baseInteractable.isSelected) return; // 잡고 있으면 무시
            // 이미 다른 Interactor가 잡고 있으면 무시 (손 여러개/멀티 터치 방지)
            if (args != null && args.interactorObject.transform.GetComponent<XRBaseInteractor>().hasSelection) return;
            OutlineColor = startingColor; // 기본 색으로
            enabled = true;               // 아웃라인 효과 켜기
        }

        // 특정 색으로 윤곽선 켜기 (외부에서 호출 가능)
        public void HighlightWithColor(Color color)
        {
            if (onlyHighlightsWhenNotSelected && _baseInteractable.isSelected) return;
            OutlineColor = color;
            enabled = true;
        }

        // 윤곽선 끄기
        public void StopHighlight() => enabled = false;
    }
}

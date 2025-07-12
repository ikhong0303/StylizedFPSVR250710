using MikeNspired.XRIStarterKit.ChrisNolet; // Outline ��ũ��Ʈ(�ܺ� �ڵ�) ��ӿ�
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// XR(VR)���� ��������Ʈ�� ���� ������ ��������(hover)��
    /// ������Ʈ�� ��������(Outline)�� ȿ���� �ڵ����� ���ְ�,
    /// ��ų�(Select) ���� ���� ȿ���� ���� ��ũ��Ʈ!
    /// </summary>
    public class XRQuickOutline : Outline // Outline(�ܺ� ���̺귯��/�⺻ ȿ��) ���!
    {
        [SerializeField] private XRBaseInteractable _baseInteractable; // ��/��Ʈ�ѷ��� ��ȣ�ۿ��� ������Ʈ
        [SerializeField] private bool onlyHighlightsWhenNotSelected;   // ��� �ִ� ������ �ƿ����� ������
        private Color startingColor;                                   // �ƿ����� ���� �� ����

        // �����Ϳ��� �� �ٲ� ������ ����. _baseInteractable�� ������ �ڵ� ����
        private void OnValidate()
        {
            if (!_baseInteractable)
                _baseInteractable = GetComponentInParent<XRBaseInteractable>();
        }

        // ���� ������ (���� ����)
        private void Start()
        {
            OnValidate(); // interactable �ڵ� ����
            startingColor = OutlineColor; // ���� ������ �� ����

            // XR ��ȣ�ۿ� �̺�Ʈ ����
            _baseInteractable.hoverEntered.AddListener(OnHoverEnter); // ��/��Ʈ�ѷ��� ������ ����
            _baseInteractable.hoverExited.AddListener(OnHoverExit);   // �־�����
            _baseInteractable.selectEntered.AddListener(OnSelectEnter); // ����� ��
            _baseInteractable.selectExited.AddListener(OnSelectExit);   // �տ��� �� ��

            enabled = false; // �⺻������ ������ ��Ȱ��ȭ
        }

        // ������Ʈ�� �ı��� �� �̺�Ʈ ����
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

        // �̺�Ʈ �ڵ鷯: ���� ������ ���� �ƿ����� �ѱ�
        private void OnHoverEnter(HoverEnterEventArgs args) => Highlight(args);
        // ���� �־����� �ƿ����� ����
        private void OnHoverExit(HoverExitEventArgs args) => StopHighlight();
        // ������(Select) �ƿ����� ����
        private void OnSelectEnter(SelectEnterEventArgs args) => StopHighlight();

        // ���� ������ ��, ���� Hover ���̸� �ٽ� ������ �ѱ�
        private void OnSelectExit(SelectExitEventArgs args)
        {
            if (_baseInteractable.isHovered)
                Highlight(null);
        }

        // ������(Outline) �ѱ� (�� Hover, ���� ȣ�� ��)
        public void Highlight(HoverEnterEventArgs args)
        {
            if (onlyHighlightsWhenNotSelected && _baseInteractable.isSelected) return; // ��� ������ ����
            // �̹� �ٸ� Interactor�� ��� ������ ���� (�� ������/��Ƽ ��ġ ����)
            if (args != null && args.interactorObject.transform.GetComponent<XRBaseInteractor>().hasSelection) return;
            OutlineColor = startingColor; // �⺻ ������
            enabled = true;               // �ƿ����� ȿ�� �ѱ�
        }

        // Ư�� ������ ������ �ѱ� (�ܺο��� ȣ�� ����)
        public void HighlightWithColor(Color color)
        {
            if (onlyHighlightsWhenNotSelected && _baseInteractable.isSelected) return;
            OutlineColor = color;
            enabled = true;
        }

        // ������ ����
        public void StopHighlight() => enabled = false;
    }
}

using TMPro;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // DamageText: ������ ���� �ؽ�Ʈ�� ǥ���ϰ�, ī�޶� �ٶ󺸸� �Ÿ� ��� ũ�⸦ �����ϴ� ��ũ��Ʈ
    public class DamageText : MonoBehaviour
    {
        private Transform playerCamera;  // �÷��̾� ī�޶� Ʈ������ (Main Camera ����)

        [SerializeField] private GameObject criticalIcon;  // ũ��Ƽ�� ��Ʈ ������ (�ɼǿ�)
        [SerializeField] private GameObject onBeatIcon;    // ��Ʈ ��Ʈ ������ (�ɼǿ�)
        [SerializeField] private TextMeshProUGUI textMesh; // �������� ǥ���� �ؽ�Ʈ �޽�
        [SerializeField] private float sizeFactor = 5f;    // ���� �Ÿ� (�� �Ÿ����� �⺻ ũ��� ����)

        private void Start()
        {
            // ���� ī�޶� ã�� ���� (�÷��̾ ���� ����)
            if (Camera.main != null)
                playerCamera = Camera.main.transform;
        }

        private void Update()
        {
            if (playerCamera == null) return;

            // �ؽ�Ʈ�� �׻� ī�޶� �ٶ󺸵��� ȸ��
            transform.LookAt(playerCamera);

            // �ؽ�Ʈ ũ�⸦ ī�޶� �Ÿ� ��ʷ� ����
            AdjustScale();
        }

        // �ؽ�Ʈ ���� �Լ� (�ܺο��� �ؽ�Ʈ �ٲٱ��)
        public void SetText(string text)
        {
            textMesh.text = text;
        }

        // ī�޶� �Ÿ� ��ʷ� �ؽ�Ʈ ũ�� ����
        private void AdjustScale()
        {
            float distance = Vector3.Distance(transform.position, playerCamera.position);
            float scaleFactor = distance / sizeFactor;

            // �Ÿ� ������� ������ ���� (�ָ� Ŀ����, ������ �۾���)
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
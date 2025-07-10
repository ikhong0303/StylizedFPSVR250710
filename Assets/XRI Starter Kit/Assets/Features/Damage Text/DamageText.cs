using TMPro;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // DamageText: 데미지 숫자 텍스트를 표시하고, 카메라를 바라보며 거리 비례 크기를 조정하는 스크립트
    public class DamageText : MonoBehaviour
    {
        private Transform playerCamera;  // 플레이어 카메라 트랜스폼 (Main Camera 기준)

        [SerializeField] private GameObject criticalIcon;  // 크리티컬 히트 아이콘 (옵션용)
        [SerializeField] private GameObject onBeatIcon;    // 비트 히트 아이콘 (옵션용)
        [SerializeField] private TextMeshProUGUI textMesh; // 데미지를 표시할 텍스트 메쉬
        [SerializeField] private float sizeFactor = 5f;    // 기준 거리 (이 거리에서 기본 크기로 보임)

        private void Start()
        {
            // 메인 카메라를 찾고 참조 (플레이어가 보는 방향)
            if (Camera.main != null)
                playerCamera = Camera.main.transform;
        }

        private void Update()
        {
            if (playerCamera == null) return;

            // 텍스트가 항상 카메라를 바라보도록 회전
            transform.LookAt(playerCamera);

            // 텍스트 크기를 카메라 거리 비례로 조정
            AdjustScale();
        }

        // 텍스트 설정 함수 (외부에서 텍스트 바꾸기용)
        public void SetText(string text)
        {
            textMesh.text = text;
        }

        // 카메라 거리 비례로 텍스트 크기 조정
        private void AdjustScale()
        {
            float distance = Vector3.Distance(transform.position, playerCamera.position);
            float scaleFactor = distance / sizeFactor;

            // 거리 기반으로 스케일 조정 (멀면 커지고, 가까우면 작아짐)
            transform.localScale = Vector3.one * scaleFactor;
        }
    }
}
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 이 컴포넌트는 플레이어의 CharacterController와 자식 콜라이더들 간의 충돌을 무시하도록 설정합니다.
    public class IgnoreCharacterControllerCollider : MonoBehaviour
    {
        // 이 오브젝트 및 자식 오브젝트에 포함된 모든 Collider를 저장합니다.
        private Collider[] mainColliders;

        // 게임 시작 시 호출됩니다.
        private void Start()
        {
            // 비활성화된 오브젝트까지 포함하여 모든 자식 Collider를 가져옵니다.
            mainColliders = GetComponentsInChildren<Collider>(true);

            // 씬에서 첫 번째 CharacterController를 찾습니다(플레이어).
            var playerCollider = FindFirstObjectByType<CharacterController>();
            if (!playerCollider) return; // CharacterController가 없으면 아무 작업도 하지 않습니다.

            // 각 자식 Collider와 플레이어의 CharacterController 간의 충돌을 무시합니다.
            foreach (var c in mainColliders)
            {
                Physics.IgnoreCollision(c, playerCollider);
            }
        }
    }
}
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // �� ������Ʈ�� �÷��̾��� CharacterController�� �ڽ� �ݶ��̴��� ���� �浹�� �����ϵ��� �����մϴ�.
    public class IgnoreCharacterControllerCollider : MonoBehaviour
    {
        // �� ������Ʈ �� �ڽ� ������Ʈ�� ���Ե� ��� Collider�� �����մϴ�.
        private Collider[] mainColliders;

        // ���� ���� �� ȣ��˴ϴ�.
        private void Start()
        {
            // ��Ȱ��ȭ�� ������Ʈ���� �����Ͽ� ��� �ڽ� Collider�� �����ɴϴ�.
            mainColliders = GetComponentsInChildren<Collider>(true);

            // ������ ù ��° CharacterController�� ã���ϴ�(�÷��̾�).
            var playerCollider = FindFirstObjectByType<CharacterController>();
            if (!playerCollider) return; // CharacterController�� ������ �ƹ� �۾��� ���� �ʽ��ϴ�.

            // �� �ڽ� Collider�� �÷��̾��� CharacterController ���� �浹�� �����մϴ�.
            foreach (var c in mainColliders)
            {
                Physics.IgnoreCollision(c, playerCollider);
            }
        }
    }
}
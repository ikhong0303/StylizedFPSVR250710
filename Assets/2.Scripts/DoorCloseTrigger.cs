using UnityEngine;

public class DoorCloseTrigger : MonoBehaviour
{
    public DoorController doorController;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;
        triggered = true;

        doorController.CloseDoor();

        // 트리거 오브젝트 자체를 비활성화해서 더이상 작동 안 하게
        gameObject.SetActive(false);
    }
}

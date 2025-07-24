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

        // Ʈ���� ������Ʈ ��ü�� ��Ȱ��ȭ�ؼ� ���̻� �۵� �� �ϰ�
        gameObject.SetActive(false);
    }
}

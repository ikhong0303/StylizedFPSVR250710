using UnityEngine;

public class DoorOpenTrigger : MonoBehaviour
{
    public DoorController doorController;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggered) return;
        triggered = true;

        doorController.OpenDoor();
    }
}
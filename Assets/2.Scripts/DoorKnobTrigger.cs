using UnityEngine;

public class DoorKnobTrigger : MonoBehaviour
{
    [HideInInspector]
    public bool knobTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            knobTriggered = true;
        }
    }
}

// Trigger에 들어가면 대화 시작
using UnityEngine;

public class MadamDialogueTrigger : MonoBehaviour
{
    public MadamDialogueManager dialogueManager;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        dialogueManager.StartDialogue();
        gameObject.SetActive(false); // 한 번만 발동
    }
}

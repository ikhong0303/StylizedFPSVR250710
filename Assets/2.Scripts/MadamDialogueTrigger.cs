// Trigger�� ���� ��ȭ ����
using UnityEngine;

public class MadamDialogueTrigger : MonoBehaviour
{
    public MadamDialogueManager dialogueManager;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        dialogueManager.StartDialogue();
        gameObject.SetActive(false); // �� ���� �ߵ�
    }
}

using UnityEngine;

public class MadamDialogueTrigger : MonoBehaviour
{
    public MadamDialogueManager dialogueManager;
    public PlayerHealth playerHealth;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerHealth != null)
        {
            // 체력 최대치로 회복
            float healAmount = playerHealth.GetMaxHealth() - playerHealth.GetCurrentHealth();
            playerHealth.Heal(healAmount);
        }

        dialogueManager.StartDialogue();
        gameObject.SetActive(false); // 한 번만 발동
        BgmManager.Instance.PlayBGM("Bgm03", true); // 대화 시작 시 배경음악 변경
    }
}

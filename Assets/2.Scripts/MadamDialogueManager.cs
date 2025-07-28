using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MadamDialogueManager : MonoBehaviour
{
    [Header("�ڸ�/����")]
    public AudioSource audioSource;
    public AudioClip[] linesAudio;
    public string[] linesText;
    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel;
    public Button nextButton;

    int currentLine = 0;

    public void StartDialogue()
    {
        Debug.Log("<color=cyan>[MadamDialogueManager]</color> StartDialogue() ȣ��� (Ʈ���� ����)");
        currentLine = 0;
        subtitlePanel.SetActive(true);
        nextButton.gameObject.SetActive(true);
        PlayCurrentLine();
    }

    public void NextLine()
    {
        currentLine++;
        if (currentLine >= linesText.Length)
        {
            EndDialogue();
            return;
        }
        PlayCurrentLine();
    }

    void PlayCurrentLine()
    {
        // �ڸ� ��� �����
        Debug.Log($"<color=yellow>[MadamDialogueManager]</color> �ڸ� ���: {linesText[currentLine]}");

        // ����� ��� �����
        if (linesAudio[currentLine] != null)
        {
            Debug.Log($"<color=green>[MadamDialogueManager]</color> ����� ���: {linesAudio[currentLine].name}");
            audioSource.clip = linesAudio[currentLine];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("[MadamDialogueManager] ����� Ŭ���� �Ҵ���� �ʾҽ��ϴ�!");
        }

        subtitleText.text = linesText[currentLine];
    }

    void EndDialogue()
    {
        Debug.Log("<color=red>[MadamDialogueManager]</color> ��ȭ ���� (EndDialogue)");
        subtitlePanel.SetActive(false);
        audioSource.Stop();
    }

    void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }
}

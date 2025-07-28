using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MadamDialogueManager : MonoBehaviour
{
    [Header("자막/음성")]
    public AudioSource audioSource;
    public AudioClip[] linesAudio;
    public string[] linesText;
    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel;
    public Button nextButton;

    int currentLine = 0;

    public void StartDialogue()
    {
        Debug.Log("<color=cyan>[MadamDialogueManager]</color> StartDialogue() 호출됨 (트리거 진입)");
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
        // 자막 출력 디버그
        Debug.Log($"<color=yellow>[MadamDialogueManager]</color> 자막 출력: {linesText[currentLine]}");

        // 오디오 출력 디버그
        if (linesAudio[currentLine] != null)
        {
            Debug.Log($"<color=green>[MadamDialogueManager]</color> 오디오 재생: {linesAudio[currentLine].name}");
            audioSource.clip = linesAudio[currentLine];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("[MadamDialogueManager] 오디오 클립이 할당되지 않았습니다!");
        }

        subtitleText.text = linesText[currentLine];
    }

    void EndDialogue()
    {
        Debug.Log("<color=red>[MadamDialogueManager]</color> 대화 종료 (EndDialogue)");
        subtitlePanel.SetActive(false);
        audioSource.Stop();
    }

    void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }
}

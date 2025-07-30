using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MadamDialogueManager : MonoBehaviour
{
    [Header("자막/음성")]
    public AudioSource audioSource;
    public AudioClip[] linesAudio;
    [TextArea(2, 5)] // Inspector에서 미리보기용 (필수는 아님)
    public string[] linesText; // Inspector에서 입력하지 않을 거라 초기화만

    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel;
    public Button nextButton;

    int currentLine = 0;

    void Awake()
    {
        // 여기서 대사 내용을 코드로 직접 초기화
        linesText = new string[]
        {
            "당신이군요…\n미래에서 왔다던 그 사람.",
            "실험실을 빠져나온 건, 쉬운 일이 아니었을 텐데요. \n이제 마지막이에요.",
            "카게무라.\n그를 끝내야 당신은 돌아갈 수 있어요.",
            "그는 류세이라는 잉어를 키워요.\n그의 기억과 혼이 깃든 존재죠.",
            "그걸 먼저 제거해야만, 그를 쓰러뜨릴 수 있어요.",
            "이걸 가져가요. 청혈 강화약.\n반응속도를 끌어올려 줄 거예요.",
            "1층 술장 뒤에 뒷문이 있어요.\n그 문을 지나면 지하수도, 그리고 카게무라의 본거지가 나옵니다.",
            "명심하세요.\n류세이가 살아있는 한... 그도 끝나지 않아요."
        };

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue()
    {
        Debug.Log("<color=cyan>[MadamDialogueManager]</color> StartDialogue() 호출됨 (트리거 진입)");
        currentLine = 0;
        subtitlePanel.SetActive(true);
        PlayCurrentLine();
    }

    public void NextLine()
    {
        Debug.Log("<color=magenta>[MadamDialogueManager]</color> [Button] NextLine() 호출됨 (버튼 클릭 감지)");
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
        Debug.Log($"<color=yellow>[MadamDialogueManager]</color> 자막 출력: {linesText[currentLine]}");

        if (linesAudio.Length > currentLine && linesAudio[currentLine] != null)
        {
            Debug.Log($"<color=green>[MadamDialogueManager]</color> 오디오 재생: {linesAudio[currentLine].name}");
            audioSource.clip = linesAudio[currentLine];
            audioSource.Play();
        }
        subtitleText.text = linesText[currentLine];
    }

    void EndDialogue()
    {
        Debug.Log("<color=red>[MadamDialogueManager]</color> 대화 종료 (EndDialogue)");
        subtitlePanel.SetActive(false);
        audioSource.Stop();
    }
}

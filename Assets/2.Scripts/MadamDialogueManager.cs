using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MadamDialogueManager : MonoBehaviour
{
    [Header("�ڸ�/����")]
    public AudioSource audioSource;
    public AudioClip[] linesAudio;
    [TextArea(2, 5)] // Inspector���� �̸������ (�ʼ��� �ƴ�)
    public string[] linesText; // Inspector���� �Է����� ���� �Ŷ� �ʱ�ȭ��

    public TextMeshProUGUI subtitleText;
    public GameObject subtitlePanel;
    public Button nextButton;

    int currentLine = 0;

    void Awake()
    {
        // ���⼭ ��� ������ �ڵ�� ���� �ʱ�ȭ
        linesText = new string[]
        {
            "����̱��䡦\n�̷����� �Դٴ� �� ���.",
            "������� �������� ��, ���� ���� �ƴϾ��� �ٵ���. \n���� �������̿���.",
            "ī�Թ���.\n�׸� ������ ����� ���ư� �� �־��.",
            "�״� �����̶�� �׾ Ű����.\n���� ���� ȥ�� ��� ������.",
            "�װ� ���� �����ؾ߸�, �׸� �����߸� �� �־��.",
            "�̰� ��������. û�� ��ȭ��.\n�����ӵ��� ����÷� �� �ſ���.",
            "1�� ���� �ڿ� �޹��� �־��.\n�� ���� ������ ���ϼ���, �׸��� ī�Թ����� �������� ���ɴϴ�.",
            "����ϼ���.\n�����̰� ����ִ� ��... �׵� ������ �ʾƿ�."
        };

        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);
    }

    public void StartDialogue()
    {
        Debug.Log("<color=cyan>[MadamDialogueManager]</color> StartDialogue() ȣ��� (Ʈ���� ����)");
        currentLine = 0;
        subtitlePanel.SetActive(true);
        PlayCurrentLine();
    }

    public void NextLine()
    {
        Debug.Log("<color=magenta>[MadamDialogueManager]</color> [Button] NextLine() ȣ��� (��ư Ŭ�� ����)");
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
        Debug.Log($"<color=yellow>[MadamDialogueManager]</color> �ڸ� ���: {linesText[currentLine]}");

        if (linesAudio.Length > currentLine && linesAudio[currentLine] != null)
        {
            Debug.Log($"<color=green>[MadamDialogueManager]</color> ����� ���: {linesAudio[currentLine].name}");
            audioSource.clip = linesAudio[currentLine];
            audioSource.Play();
        }
        subtitleText.text = linesText[currentLine];
    }

    void EndDialogue()
    {
        Debug.Log("<color=red>[MadamDialogueManager]</color> ��ȭ ���� (EndDialogue)");
        subtitlePanel.SetActive(false);
        audioSource.Stop();
    }
}

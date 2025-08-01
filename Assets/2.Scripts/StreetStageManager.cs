using UnityEngine;
using System.Collections;

public class StreetStageManager : MonoBehaviour
{
    [SerializeField] GameObject naviLine;    // ���� �ִ� NaviLine ������Ʈ

    private GameObject missionPopupPanel;

    private void Start()
    {
        // ���� ��η� Mission ������Ʈ ã�Ƽ� ����
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            Transform slots = gm.transform.Find("Slots");
            if (slots != null)
            {
                Transform mission = slots.Find("Mission");
                if (mission != null)
                {
                    missionPopupPanel = mission.gameObject;
                }
                else
                {
                    Debug.LogWarning("Mission ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("Slots ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager ������Ʈ�� ã�� �� �����ϴ�.");
        }

        StartCoroutine(StageFlow());
    }

    IEnumerator StageFlow()
    {
        yield return new WaitForSeconds(3f);

        if (missionPopupPanel != null) missionPopupPanel.SetActive(true);
        if (naviLine != null) naviLine.SetActive(true);

        yield return new WaitForSeconds(2f);

        // �� �̱������� ȣ��
        AudioManager2.Instance.PlayNarration("StNarr01");
        print("StNarr01 ���");
    }
}

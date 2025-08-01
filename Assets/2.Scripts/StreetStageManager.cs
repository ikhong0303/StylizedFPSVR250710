using UnityEngine;
using System.Collections;

public class StreetStageManager : MonoBehaviour
{
    [SerializeField] GameObject naviLine;    // 씬에 있는 NaviLine 오브젝트

    private GameObject missionPopupPanel;

    private void Start()
    {
        // 계층 경로로 Mission 오브젝트 찾아서 저장
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
                    Debug.LogWarning("Mission 오브젝트를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("Slots 오브젝트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("GameManager 오브젝트를 찾을 수 없습니다.");
        }

        StartCoroutine(StageFlow());
    }

    IEnumerator StageFlow()
    {
        yield return new WaitForSeconds(3f);

        if (missionPopupPanel != null) missionPopupPanel.SetActive(true);
        if (naviLine != null) naviLine.SetActive(true);

        yield return new WaitForSeconds(2f);

        // ★ 싱글톤으로 호출
        AudioManager2.Instance.PlayNarration("StNarr01");
        print("StNarr01 재생");
    }
}

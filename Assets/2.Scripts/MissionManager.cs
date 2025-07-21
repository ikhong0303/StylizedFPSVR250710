using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissionManager : MonoBehaviour
{
    public GameObject missionPanel;
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDesc;
    public TextMeshProUGUI missionLocation;
    public TextMeshProUGUI missionHint;

    [Header("미션 정보(Inspector에서 입력)")]
    public MissionInfo[] missions;

    private int missionStep = 0;

    public void NextMission()
    {
        missionStep++;
        if (missionStep >= missions.Length) missionStep = missions.Length - 1;
        UpdateMissionUI();
    }

    public void SetMission(int idx)
    {
        if (idx >= 0 && idx < missions.Length)
        {
            missionStep = idx;
            UpdateMissionUI();
        }
    }

    void UpdateMissionUI()
    {
        if (missionStep < missions.Length)
        {
            var m = missions[missionStep];
            missionTitle.text = m.title;
            missionDesc.text = m.description;
            missionLocation.text = "위치 : " + m.location;
            missionHint.text = string.IsNullOrEmpty(m.hint) ? "" : "힌트 : " + m.hint;
        }
        else
        {
            missionTitle.text = "미션 완료";
            missionDesc.text = "모든 미션을 완료했습니다!";
            missionLocation.text = "";
            missionHint.text = "";
        }
    }
}

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

    [Header("�̼� ����(Inspector���� �Է�)")]
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
            missionLocation.text = "��ġ : " + m.location;
            missionHint.text = string.IsNullOrEmpty(m.hint) ? "" : "��Ʈ : " + m.hint;
        }
        else
        {
            missionTitle.text = "�̼� �Ϸ�";
            missionDesc.text = "��� �̼��� �Ϸ��߽��ϴ�!";
            missionLocation.text = "";
            missionHint.text = "";
        }
    }
}

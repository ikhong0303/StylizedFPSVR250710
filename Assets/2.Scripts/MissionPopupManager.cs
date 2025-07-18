using UnityEngine;
using UnityEngine.InputSystem;

public class MissionPopupManager : MonoBehaviour
{
    public GameObject missionPanel; // Inspector에서 패널 드래그
    public InputActionReference showMissionInput; // Inspector에서 Input Action 드래그

    void OnEnable() => showMissionInput.action.performed += _ => ToggleMissionPanel();
    void OnDisable() => showMissionInput.action.performed -= _ => ToggleMissionPanel();

    private void ToggleMissionPanel()
    {
        missionPanel.SetActive(!missionPanel.activeSelf);
    }
}
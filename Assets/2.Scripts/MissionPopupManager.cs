using UnityEngine;
using UnityEngine.InputSystem;

public class MissionPopupManager : MonoBehaviour
{
    public GameObject missionPanel; // Inspector���� �г� �巡��
    public InputActionReference showMissionInput; // Inspector���� Input Action �巡��

    void OnEnable() => showMissionInput.action.performed += _ => ToggleMissionPanel();
    void OnDisable() => showMissionInput.action.performed -= _ => ToggleMissionPanel();

    private void ToggleMissionPanel()
    {
        missionPanel.SetActive(!missionPanel.activeSelf);
    }
}
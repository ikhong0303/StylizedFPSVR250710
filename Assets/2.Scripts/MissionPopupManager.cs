using UnityEngine;
using UnityEngine.InputSystem;

public class MissionPopupFollower : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform leftWrist;         // XR Rig의 왼손 손목 Transform (Inspector에서 직접 drag)
    public GameObject missionPanel;     // 미션 패널 루트 오브젝트 (Inspector drag)
    public InputActionReference showMissionInput; // 팝업 토글 입력 (Inspector drag)

    [Header("옵션")]
    public Vector3 offset = new Vector3(0f, 0f, 0.1f);  // 손목에서의 위치 오프셋
    public bool lookAtCamera = true;                    // 카메라를 바라보게 할지

    private bool isVisible = false;

    void OnEnable()
    {
        showMissionInput.action.performed += _ => ToggleMissionPanel();
    }
    void OnDisable()
    {
        showMissionInput.action.performed -= _ => ToggleMissionPanel();
    }

    void Update()
    {
        if (!missionPanel.activeSelf) return;
        if (leftWrist == null) return;

        // 위치: 손목 위치 + 오프셋
        missionPanel.transform.position = leftWrist.position + leftWrist.TransformDirection(offset);

        // 회전: 항상 카메라를 바라보게
        if (lookAtCamera && Camera.main != null)
        {
            Vector3 dir = (missionPanel.transform.position - Camera.main.transform.position).normalized;
            missionPanel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        // 아니면 고정 각도 원하면 아래처럼
        // missionPanel.transform.rotation = Quaternion.identity;
    }

    void ToggleMissionPanel()
    {
        isVisible = !missionPanel.activeSelf;
        missionPanel.SetActive(isVisible);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 미션 패널(팝업)을 VR 플레이어의 손목 위치에 따라다니게 하고,
/// 씬 전환 등으로 XR Origin이 바뀌어도 자동으로 손목 트랜스폼을 찾아 연결한다.
/// (팝업 표시/숨기기, 위치 오프셋, 카메라 빌보드 지원)
/// </summary>
public class MissionPopupFollower : MonoBehaviour
{
    [Header("필수 연결")]
    [Tooltip("미션 패널 루트 오브젝트 (Inspector에서 drag)")]
    public GameObject missionPanel;

    [Tooltip("팝업 토글 입력(Input System)")]
    public InputActionReference showMissionInput;

    [Header("자동 바인딩 옵션")]
    [Tooltip("손목 트랜스폼을 찾기 위한 경로 (Player 하위, 예: 'Origin/XROrigin/CameraOffset/LeftController')")]
    public string leftWristPath = "Camera Offset/Left Controller"; // 프로젝트 구조에 맞게 수정

    [Header("위치/회전 옵션")]
    [Tooltip("손목에서의 위치 오프셋")]
    public Vector3 offset = new Vector3(0f, 0f, 0.1f);

    [Tooltip("팝업이 항상 카메라를 바라보게 할지")]
    public bool lookAtCamera = true;

    private Transform leftWrist;
    private bool isVisible = false;

    void OnEnable()
    {
        // 씬 전환 등으로 Player가 새로 생성되었을 수 있으니 자동 바인딩 시도
        TryAutoAssignWrist();

        // 인풋 액션에 토글 함수 연결
        showMissionInput.action.performed += OnShowMissionInput;
    }

    void OnDisable()
    {
        showMissionInput.action.performed -= OnShowMissionInput;
    }

    /// <summary>
    /// Player가 새로 생성됐거나 leftWrist가 null이면,
    /// GameManager.Instance.player에서 leftWristPath 경로로 Transform을 찾아 자동 할당한다.
    /// </summary>
    void TryAutoAssignWrist()
    {
        if (leftWrist != null) return;

        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            // Player 하위에서 leftWristPath 경로로 Transform 찾기
            Transform wrist = GameManager.Instance.player.transform.Find(leftWristPath);
            if (wrist != null)
                leftWrist = wrist;
            else
                Debug.LogWarning($"MissionPopupFollower: '{leftWristPath}' 경로의 손목 Transform을 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 만약 씬 전환 등으로 leftWrist가 끊어졌으면 자동 재바인딩 시도
        if (leftWrist == null)
            TryAutoAssignWrist();

        // 패널이 비활성화 상태면 위치 갱신 필요 없음
        if (!missionPanel.activeSelf) return;
        if (leftWrist == null) return;

        // 1. 위치: 손목 위치 + 오프셋
        missionPanel.transform.position = leftWrist.position + leftWrist.TransformDirection(offset);

        // 2. 회전: 카메라를 바라보게 (빌보드 효과)
        if (lookAtCamera && Camera.main != null)
        {
            Vector3 dir = (missionPanel.transform.position - Camera.main.transform.position).normalized;
            missionPanel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        // else: 고정 각도 원하면 아래처럼 (예시)
        // missionPanel.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 팝업 토글 입력 시 호출되는 함수
    /// </summary>
    void OnShowMissionInput(InputAction.CallbackContext ctx)
    {
        ToggleMissionPanel();
    }

    /// <summary>
    /// 미션 패널 표시/숨기기 토글
    /// </summary>
    void ToggleMissionPanel()
    {
        isVisible = !missionPanel.activeSelf;
        missionPanel.SetActive(isVisible);
    }
}

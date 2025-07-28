using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// �̼� �г�(�˾�)�� VR �÷��̾��� �ո� ��ġ�� ����ٴϰ� �ϰ�,
/// �� ��ȯ �Ŀ��� �ڵ����� �ո� ���ε��ǵ��� �Ѵ�.
/// (�˾� ǥ��/�����, ��ġ ������, ī�޶� ������, �ڵ� �ո� ���� ����)
/// </summary>
public class MissionPopupFollower : MonoBehaviour
{
    [Header("�ʼ� ����")]
    [Tooltip("�÷��̾��� �޼� �ո� Transform. Inspector���� drag�ϰų�, �ڵ� �Ҵ� ���")]
    public Transform leftWrist;

    [Tooltip("�̼� �г� ��Ʈ ������Ʈ")]
    public GameObject missionPanel;

    [Tooltip("�˾� ��� �Է�(Input System)")]
    public InputActionReference showMissionInput;

    [Header("�ڵ� ���ε� �ɼ�")]
    [Tooltip("�ո� Ʈ�������� ã�� ���� ��� (Player ����)")]
    public string leftWristPath = "HandL/PopupAttachPoint";

    [Header("��ġ/ȸ�� �ɼ�")]
    [Tooltip("�ո񿡼��� ��ġ ������")]
    public Vector3 offset = new Vector3(0f, 0f, 0.1f);

    [Tooltip("�˾��� �׻� ī�޶� �ٶ󺸰� ����")]
    public bool lookAtCamera = true;

    private bool isVisible = false;

    void OnEnable()
    {
        // �� ��ȯ ������ Player�� ���� �����Ǿ��� �� ������ �ڵ� ���ε� �õ�
        TryAutoAssignWrist();

        // ��ǲ �׼ǿ� ��� �Լ� ����
        showMissionInput.action.performed += OnShowMissionInput;
    }

    void OnDisable()
    {
        showMissionInput.action.performed -= OnShowMissionInput;
    }

    /// <summary>
    /// Player�� ���� �����ưų� leftWrist�� null�̸�,
    /// GameManager.Instance.player���� leftWristPath ��η� Transform�� ã�� �ڵ� �Ҵ��Ѵ�.
    /// </summary>
    void TryAutoAssignWrist()
    {
        if (leftWrist != null) return;
        if (GameManager.Instance != null && GameManager.Instance.player != null)
        {
            Transform wrist = GameManager.Instance.player.transform.Find(leftWristPath);
            if (wrist != null)
                leftWrist = wrist;
        }
    }

    void Update()
    {
        // ���� �� ��ȯ ������ leftWrist�� ���������� �ڵ� ����ε� �õ�
        if (leftWrist == null)
            TryAutoAssignWrist();

        // �г��� ��Ȱ��ȭ ���¸� ��ġ ���� �ʿ� ����
        if (!missionPanel.activeSelf) return;
        if (leftWrist == null) return;

        // 1. ��ġ: �ո� ��ġ + ������
        missionPanel.transform.position = leftWrist.position + leftWrist.TransformDirection(offset);

        // 2. ȸ��: ī�޶� �ٶ󺸰� (������ ȿ��)
        if (lookAtCamera && Camera.main != null)
        {
            Vector3 dir = (missionPanel.transform.position - Camera.main.transform.position).normalized;
            missionPanel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        // else: ���� ���� ���ϸ� �Ʒ�ó�� (����)
        // missionPanel.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// �˾� ��� �Է� �� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="ctx"></param>
    void OnShowMissionInput(InputAction.CallbackContext ctx)
    {
        ToggleMissionPanel();
    }

    /// <summary>
    /// �̼� �г� ǥ��/����� ���
    /// </summary>
    void ToggleMissionPanel()
    {
        isVisible = !missionPanel.activeSelf;
        missionPanel.SetActive(isVisible);
    }
}

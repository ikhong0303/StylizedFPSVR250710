using UnityEngine;
using UnityEngine.InputSystem;

public class MissionPopupFollower : MonoBehaviour
{
    [Header("�ʼ� ����")]
    public Transform leftWrist;         // XR Rig�� �޼� �ո� Transform (Inspector���� ���� drag)
    public GameObject missionPanel;     // �̼� �г� ��Ʈ ������Ʈ (Inspector drag)
    public InputActionReference showMissionInput; // �˾� ��� �Է� (Inspector drag)

    [Header("�ɼ�")]
    public Vector3 offset = new Vector3(0f, 0f, 0.1f);  // �ո񿡼��� ��ġ ������
    public bool lookAtCamera = true;                    // ī�޶� �ٶ󺸰� ����

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

        // ��ġ: �ո� ��ġ + ������
        missionPanel.transform.position = leftWrist.position + leftWrist.TransformDirection(offset);

        // ȸ��: �׻� ī�޶� �ٶ󺸰�
        if (lookAtCamera && Camera.main != null)
        {
            Vector3 dir = (missionPanel.transform.position - Camera.main.transform.position).normalized;
            missionPanel.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        // �ƴϸ� ���� ���� ���ϸ� �Ʒ�ó��
        // missionPanel.transform.rotation = Quaternion.identity;
    }

    void ToggleMissionPanel()
    {
        isVisible = !missionPanel.activeSelf;
        missionPanel.SetActive(isVisible);
    }
}
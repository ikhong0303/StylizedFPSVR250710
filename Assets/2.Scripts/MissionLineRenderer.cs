using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public List<MissionStep> missionSteps = new List<MissionStep>();
    public LineRenderer lineRenderer;

    [Tooltip("�˾� ��� �Է�(Input System)")]
    public InputActionReference showMissionInput;
    public GameObject Line; // ���� ��ü(�θ�) ������Ʈ

    [Header("���� ����")]
    public float lineWidth = 0.07f;
    public float waypointReachDistance = 1.5f;
    public float missionTargetReachDistance = 2.0f;

    int currentMissionIndex = 0;
    MissionStep currentStep => missionSteps[currentMissionIndex];

    void OnEnable()
    {
        showMissionInput.action.performed += OnShowMissionInput;
    }

    void OnDisable()
    {
        showMissionInput.action.performed -= OnShowMissionInput;
    }

    void FixedUpdate()
    {
        // �̼��̳� �÷��̾� ������ ���� ��Ȱ��ȭ
        if (player == null || missionSteps.Count == 0)
        {
            if (Line.activeSelf)
                Line.SetActive(false);
            return;
        }

        // --- ��������Ʈ �ڵ� ���� ---
        if (currentStep.waypoints.Count > 0)
        {
            float dist = Vector3.Distance(player.position, currentStep.waypoints[0].position);
            if (dist < waypointReachDistance)
                currentStep.waypoints.RemoveAt(0);
        }
        // --- ��ǥ ���� �� ---
        else
        {
            float dist = Vector3.Distance(player.position, currentStep.missionTarget.position);
            if (dist < missionTargetReachDistance)
            {
                if (currentMissionIndex < missionSteps.Count - 1)
                {
                    currentMissionIndex++;
                }
                else
                {
                    // ��� �̼� �Ϸ�: ���� �����
                    if (Line.activeSelf)
                        Line.SetActive(false);
                    // �ʿ�� Destroy(this.gameObject);
                    return;
                }
            }
        }

        // �̼� ���̸� ���� �׻� ǥ��
        if (!Line.activeSelf)
            Line.SetActive(true);

        DrawLine();
    }

    void DrawLine()
    {
        int count = 2 + currentStep.waypoints.Count;
        lineRenderer.positionCount = count;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);
        for (int i = 0; i < currentStep.waypoints.Count; i++)
            lineRenderer.SetPosition(i + 1, currentStep.waypoints[i].position);
        lineRenderer.SetPosition(count - 1, currentStep.missionTarget.position);
    }

    void OnShowMissionInput(InputAction.CallbackContext ctx)
    {
        ToggleLine();
    }

    // �̼� ���� ��ü ǥ��/����� ��� (GameObject�� SetActive�� ���)
    void ToggleLine()
    {
        Line.SetActive(!Line.activeSelf);
    }
}

[System.Serializable]
public class MissionStep
{
    public List<Transform> waypoints = new List<Transform>();
    public Transform missionTarget;
}

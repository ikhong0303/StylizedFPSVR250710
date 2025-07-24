using UnityEngine;
using System.Collections.Generic;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public List<MissionStep> missionSteps = new List<MissionStep>();
    public LineRenderer lineRenderer;

    [Header("���� ����")]
    public float lineWidth = 0.07f;  // Inspector���� ����

    public float waypointReachDistance = 1.5f;
    public float missionTargetReachDistance = 2.0f;

    int currentMissionIndex = 0;
    MissionStep currentStep => missionSteps[currentMissionIndex];

    void FixedUpdate()
    {
        if (player == null || missionSteps.Count == 0)
        {
            lineRenderer.enabled = false; return;
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
                    // ���� �̼� �ܰ�� �̵�
                    currentMissionIndex++;
                }
                else
                {
                    // ��� �̼� �Ϸ�!
                    lineRenderer.enabled = false;
                    // �ʿ�� �߰� ����
                    // Destroy(this.gameObject);
                    return;
                }
            }
        }

        DrawLine();
    }

    void DrawLine()
    {
        int count = 2 + currentStep.waypoints.Count;
        lineRenderer.enabled = true;
        lineRenderer.positionCount = count;

        // �� ����!
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);
        for (int i = 0; i < currentStep.waypoints.Count; i++)
            lineRenderer.SetPosition(i + 1, currentStep.waypoints[i].position);
        lineRenderer.SetPosition(count - 1, currentStep.missionTarget.position);
    }
}

// --- �Ʒ��� Inspector���� ���̰� �ϱ� ���� ---
[System.Serializable]
public class MissionStep
{
    public List<Transform> waypoints = new List<Transform>();
    public Transform missionTarget;
}

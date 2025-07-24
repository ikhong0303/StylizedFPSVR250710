using UnityEngine;
using System.Collections.Generic;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public List<MissionStep> missionSteps = new List<MissionStep>();
    public LineRenderer lineRenderer;

    [Header("라인 설정")]
    public float lineWidth = 0.07f;  // Inspector에서 조절

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

        // --- 웨이포인트 자동 제거 ---
        if (currentStep.waypoints.Count > 0)
        {
            float dist = Vector3.Distance(player.position, currentStep.waypoints[0].position);
            if (dist < waypointReachDistance)
                currentStep.waypoints.RemoveAt(0);
        }
        // --- 목표 도달 시 ---
        else
        {
            float dist = Vector3.Distance(player.position, currentStep.missionTarget.position);
            if (dist < missionTargetReachDistance)
            {
                if (currentMissionIndex < missionSteps.Count - 1)
                {
                    // 다음 미션 단계로 이동
                    currentMissionIndex++;
                }
                else
                {
                    // 모든 미션 완료!
                    lineRenderer.enabled = false;
                    // 필요시 추가 정리
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

        // 폭 적용!
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);
        for (int i = 0; i < currentStep.waypoints.Count; i++)
            lineRenderer.SetPosition(i + 1, currentStep.waypoints[i].position);
        lineRenderer.SetPosition(count - 1, currentStep.missionTarget.position);
    }
}

// --- 아래는 Inspector에서 보이게 하기 위함 ---
[System.Serializable]
public class MissionStep
{
    public List<Transform> waypoints = new List<Transform>();
    public Transform missionTarget;
}

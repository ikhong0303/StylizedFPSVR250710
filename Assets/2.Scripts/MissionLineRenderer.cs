using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public List<MissionStep> missionSteps = new List<MissionStep>();
    public LineRenderer lineRenderer;

    [Tooltip("팝업 토글 입력(Input System)")]
    public InputActionReference showMissionInput;
    public GameObject Line; // 라인 전체(부모) 오브젝트

    [Header("라인 설정")]
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
        // 미션이나 플레이어 없으면 라인 비활성화
        if (player == null || missionSteps.Count == 0)
        {
            if (Line.activeSelf)
                Line.SetActive(false);
            return;
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
                    currentMissionIndex++;
                }
                else
                {
                    // 모든 미션 완료: 라인 숨기기
                    if (Line.activeSelf)
                        Line.SetActive(false);
                    // 필요시 Destroy(this.gameObject);
                    return;
                }
            }
        }

        // 미션 중이면 라인 항상 표시
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

    // 미션 라인 전체 표시/숨기기 토글 (GameObject의 SetActive만 사용)
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

using UnityEngine;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public Transform missionTarget; // 목표 지점
    public LineRenderer lineRenderer;
    public GameObject panel; // 미션 패널 (팝업 여부 체크용)
    public Transform[] waypoints;

    void FixedUpdate()
    {
        if (panel.activeSelf && player != null && missionTarget != null)
        {
            int count = 2 + (waypoints != null ? waypoints.Length : 0);
            lineRenderer.enabled = true;
            lineRenderer.positionCount = count;
            lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);

            // 중간 경유지 여러 개!
            for (int i = 0; i < (waypoints?.Length ?? 0); i++)
            {
                lineRenderer.SetPosition(i + 1, waypoints[i].position);
            }

            lineRenderer.SetPosition(count - 1, missionTarget.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }

        float scrollSpeed = 2.0f;
        lineRenderer.material.mainTextureOffset += Vector2.right * scrollSpeed * Time.deltaTime;
        // 라인 렌더러의 텍스처 오프셋을 오른쪽으로 이동시켜 애니메이션 효과를 줌
    }
}
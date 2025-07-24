using UnityEngine;

/// <summary>
/// 플레이어와 목표지점(또는 경유지)을 잇는 라인을 보여주는 스크립트.
/// - 라인 끝점들(경로)을 순서대로 설정.
/// - 라인에 애니메이션(흐르는 텍스처 효과)도 적용.
/// </summary>
public class MissionLineRenderer : MonoBehaviour
{
    // 플레이어 위치를 참조 (라인의 시작점)
    public Transform player;

    // 미션 목표 지점 (라인의 마지막 점)
    public Transform missionTarget;

    // 라인렌더러 컴포넌트 (Inspector에서 직접 할당)
    public LineRenderer lineRenderer;

    // 중간 경유지(웨이포인트) 배열 (필요하면 Inspector에서 여러 개 할당)
    public Transform[] waypoints;

    /// <summary>
    /// FixedUpdate는 매 프레임 일정한 간격(물리 엔진 업데이트)마다 호출됨.
    /// (VR/3D에서는 움직임이 빠를 때 더 정확한 라인 갱신을 위해 FixedUpdate를 주로 사용)
    /// </summary>
    void FixedUpdate()
    {
        // 플레이어와 목표지점이 모두 정상적으로 할당돼 있을 때만 라인 표시
        if (player != null && missionTarget != null)
        {
            // 라인을 이루는 점 개수: (시작점 + 경유지 개수 + 마지막 목표지점)
            int count = 2 + (waypoints?.Length ?? 0); // null 체크 및 배열 길이 안전 처리

            lineRenderer.enabled = true;         // 라인 표시 ON
            lineRenderer.positionCount = count;  // 점 개수 설정

            // 1. 라인 시작점(플레이어 머리 위 등 약간 올려서 가시성 확보)
            Vector3 startPos = player.position + Vector3.up * 0.1f;
            lineRenderer.SetPosition(0, startPos);

            // 2. 중간 경유지(waypoints)들을 순서대로 점에 입력
            //   예: [시작] → [경유1] → [경유2] ... → [목표]
            for (int i = 0; i < (waypoints?.Length ?? 0); i++)
            {
                // i+1번째 점에 각 경유지 좌표 할당
                lineRenderer.SetPosition(i + 1, waypoints[i].position);
            }

            // 3. 라인 마지막 점에 목표지점 위치 입력
            lineRenderer.SetPosition(count - 1, missionTarget.position);

            // -------------------------
            // ★ 흐르는 라인 애니메이션 처리 ★
            // 라인에 텍스처가 할당돼 있다면, Offset을 조절해서 '흐르는 효과'를 만듦
            // (Scroll Speed: 초당 몇 픽셀씩 이동시킬지 조정)
            float scrollSpeed = 2.0f;
            // Offset 값(텍스처 위치)을 오른쪽(X축)으로 증가시켜서 애니메이션 효과 부여
            lineRenderer.material.mainTextureOffset += Vector2.right * scrollSpeed * Time.deltaTime;
        }
        else
        {
            // 플레이어나 목표지점이 없으면 라인 비활성화(숨김)
            lineRenderer.enabled = false;
        }
    }
}

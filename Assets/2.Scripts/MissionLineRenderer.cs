using UnityEngine;

public class MissionLineRenderer : MonoBehaviour
{
    public Transform player;
    public Transform missionTarget; // ��ǥ ����
    public LineRenderer lineRenderer;
    public GameObject panel; // �̼� �г� (�˾� ���� üũ��)
    public Transform[] waypoints;

    void FixedUpdate()
    {
        if (panel.activeSelf && player != null && missionTarget != null)
        {
            int count = 2 + (waypoints != null ? waypoints.Length : 0);
            lineRenderer.enabled = true;
            lineRenderer.positionCount = count;
            lineRenderer.SetPosition(0, player.position + Vector3.up * 0.1f);

            // �߰� ������ ���� ��!
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
        // ���� �������� �ؽ�ó �������� ���������� �̵����� �ִϸ��̼� ȿ���� ��
    }
}
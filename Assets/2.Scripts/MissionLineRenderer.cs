using UnityEngine;

/// <summary>
/// �÷��̾�� ��ǥ����(�Ǵ� ������)�� �մ� ������ �����ִ� ��ũ��Ʈ.
/// - ���� ������(���)�� ������� ����.
/// - ���ο� �ִϸ��̼�(�帣�� �ؽ�ó ȿ��)�� ����.
/// </summary>
public class MissionLineRenderer : MonoBehaviour
{
    // �÷��̾� ��ġ�� ���� (������ ������)
    public Transform player;

    // �̼� ��ǥ ���� (������ ������ ��)
    public Transform missionTarget;

    // ���η����� ������Ʈ (Inspector���� ���� �Ҵ�)
    public LineRenderer lineRenderer;

    // �߰� ������(��������Ʈ) �迭 (�ʿ��ϸ� Inspector���� ���� �� �Ҵ�)
    public Transform[] waypoints;

    /// <summary>
    /// FixedUpdate�� �� ������ ������ ����(���� ���� ������Ʈ)���� ȣ���.
    /// (VR/3D������ �������� ���� �� �� ��Ȯ�� ���� ������ ���� FixedUpdate�� �ַ� ���)
    /// </summary>
    void FixedUpdate()
    {
        // �÷��̾�� ��ǥ������ ��� ���������� �Ҵ�� ���� ���� ���� ǥ��
        if (player != null && missionTarget != null)
        {
            // ������ �̷�� �� ����: (������ + ������ ���� + ������ ��ǥ����)
            int count = 2 + (waypoints?.Length ?? 0); // null üũ �� �迭 ���� ���� ó��

            lineRenderer.enabled = true;         // ���� ǥ�� ON
            lineRenderer.positionCount = count;  // �� ���� ����

            // 1. ���� ������(�÷��̾� �Ӹ� �� �� �ణ �÷��� ���ü� Ȯ��)
            Vector3 startPos = player.position + Vector3.up * 0.1f;
            lineRenderer.SetPosition(0, startPos);

            // 2. �߰� ������(waypoints)���� ������� ���� �Է�
            //   ��: [����] �� [����1] �� [����2] ... �� [��ǥ]
            for (int i = 0; i < (waypoints?.Length ?? 0); i++)
            {
                // i+1��° ���� �� ������ ��ǥ �Ҵ�
                lineRenderer.SetPosition(i + 1, waypoints[i].position);
            }

            // 3. ���� ������ ���� ��ǥ���� ��ġ �Է�
            lineRenderer.SetPosition(count - 1, missionTarget.position);

            // -------------------------
            // �� �帣�� ���� �ִϸ��̼� ó�� ��
            // ���ο� �ؽ�ó�� �Ҵ�� �ִٸ�, Offset�� �����ؼ� '�帣�� ȿ��'�� ����
            // (Scroll Speed: �ʴ� �� �ȼ��� �̵���ų�� ����)
            float scrollSpeed = 2.0f;
            // Offset ��(�ؽ�ó ��ġ)�� ������(X��)���� �������Ѽ� �ִϸ��̼� ȿ�� �ο�
            lineRenderer.material.mainTextureOffset += Vector2.right * scrollSpeed * Time.deltaTime;
        }
        else
        {
            // �÷��̾ ��ǥ������ ������ ���� ��Ȱ��ȭ(����)
            lineRenderer.enabled = false;
        }
    }
}

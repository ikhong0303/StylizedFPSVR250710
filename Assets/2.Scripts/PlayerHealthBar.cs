using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾� �ո� ���� ���ٴϴ� ü�¹�. (���ʿ��� ����� ������ ����)
/// �¿� 90�� �̹���(leftBar, rightBar)�� ���� �����ϸ�,
/// ������ ��� �̹���(backgroundImage)�� �ð��� ���̰� �����.
///

/// ü�¹� ��ġ, ȸ��, ����, ������ �� ���� ������ �ּ� ����!
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    [Header("��ġ ����")]
    public Transform playerHead;    // XR Origin, Ȥ�� �÷��̾� ��ü(Inspector���� ���� drag)
    public Transform playerBody;
    public Vector3 localOffset = new Vector3(0, 1.0f, 0.2f); // �÷��̾� ���� �ｺ�� ��ġ(���� ����)

    [Header("ü�� ������")]
    public PlayerHealth playerHealth; // ü�� ����(�ܺο��� �Ҵ�, Ȥ�� GetComponent)

    [Header("�̹��� ���� �� (��/�� 90�� ����)")]
    public Image leftBar;      // ���� 90��(�ݿ�) �̹��� (Inspector���� drag)
    public Image rightBar;     // ������ 90��(�ݿ�) �̹��� (Inspector���� drag)
    public Image backgroundImage; // (����) ������ ��� �̹���(���̰� ����, �׻� �Ʒ��� ������!)

    [Header("����/ȿ�� �ɼ�")]
    public Color fullHealthColor = Color.green;                       // ü�� 100% ����
    public Color midHealthColor = Color.yellow;                       // ü�� 50% ����
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);         // ü�� 30% ���� ����

    [Header("������ ȿ��(�����)")]
    [Range(0f, 1f)] public float blinkAlphaMin = 0.1f;   // �����ӽ� �ּ� ����
    [Range(0f, 1f)] public float blinkAlphaMax = 1.0f;   // �����ӽ� �ִ� ����
    public float blinkSpeed = 10f;                       // ������ �ӵ� (�������� ������)

    void LateUpdate()
    {
        if (playerHead == null || playerBody == null) return;

        // 1. ��ġ: �Ӹ� offset ��ġ, Y�� body ����
        Vector3 pos = playerHead.TransformPoint(localOffset);
        pos.y = playerBody.position.y + localOffset.y;
        transform.position = pos;

        // 2. ȸ��: �Ӹ�(ī�޶�)�� Yaw(�¿�ȸ��)�� ���
        Vector3 euler = playerHead.rotation.eulerAngles;
        Quaternion yawOnly = Quaternion.Euler(0, euler.y, 0);

        transform.rotation = yawOnly;
        // �߰� ȸ��(�̹��� ���� ���� ������)
        transform.Rotate(90, 0, -180);

        // 3. ü�� �� �̹��� fillAmount/���� �� ����
        if (playerHealth != null && leftBar != null && rightBar != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            float percent = Mathf.Clamp01(current / max);

            // ���ʿ��� �߾����� ������ ȿ��:
            // fillAmount 1�̸� ��� �� ����, 0�̸� ���� ����
            leftBar.fillAmount = percent;      // ���� fillAmount ����(0~1)
            rightBar.fillAmount = percent;     // ���� fillAmount ����(0~1)

            // ü�� ������ ���� ���� (�ʿ�� ���� �� ���� Ŀ����)
            Color barColor;
            if (percent > 0.7f)
            {
                // 70%~100% ����: full �� mid ���� ����
                float t = (percent - 0.7f) / 0.3f; // (0.7~1.0 �� 0~1)
                barColor = Color.Lerp(midHealthColor, fullHealthColor, t);
                barColor.a = 1f;
            }
            else if (percent > 0.4f)
            {
                // 40%~70% ����: low �� mid ���� ����
                float t = (percent - 0.4f) / 0.3f; // (0.4~0.7 �� 0~1)
                barColor = Color.Lerp(lowHealthColor, midHealthColor, t);
                barColor.a = 1f;
            }
            else
            {
                // 0~40%: ����, ��ü��(������ ����)
                barColor = lowHealthColor;
                float blink = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
                barColor.a = blink;
            }
            leftBar.color = barColor;
            rightBar.color = barColor;

            // (����) ���� �¿� bar ���� �ٸ��� �ְ� ������ ���� ���� color ����!
        }

        // 4. ������ ��� �̹����� Inspector���� ���� �����ص� ������,
        //     �������� ����/���ĸ� �ٲ� ��� �Ʒ�ó��!
        // if (backgroundImage != null)
        //     backgroundImage.color = new Color(0.15f, 0.15f, 0.15f, 0.5f); // (r,g,b,alpha) ���ϴ� ������ ����
    }
}

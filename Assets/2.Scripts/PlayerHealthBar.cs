using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform playerHead;
    public PlayerHealth playerHealth;
    public Image curveBar;

    public float distance = 1f;
    public float yourOffset = -1.5f; // �÷��̾� �Ӹ� ���� �ø��� ����

    // ���� ����
    public Color fullHealthColor = Color.green;                  // 100% (���)
    public Color midHealthColor = Color.yellow;                  // 50% (�����)
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);    // 0% (������)

    // ������ ����
    public float blinkAlphaMin = 0.4f; // �ּ� ���İ�
    public float blinkAlphaMax = 1.0f; // �ִ� ���İ�
    public float blinkSpeed = 2.5f;    // 1�ʴ� ������ �ӵ�

    void LateUpdate()
    {
        // ��ġ ����
        Vector3 camPos = playerHead.position;
        Vector3 forward = playerHead.forward; forward.y = 0f; forward.Normalize();
        Vector3 barPos = camPos + forward * distance + Vector3.up * yourOffset;
        transform.position = barPos;

        transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(90, 0, 0);

        // ü�� ���� + ���� �׶��̼� + ������
        if (playerHealth != null && curveBar != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            float percent = Mathf.Clamp01(current / max);

            curveBar.fillAmount = percent;

            Color barColor;

            if (percent > 0.5f)
            {
                // ��� �� ����� (50~100%)
                float t = (percent - 0.5f) / 0.5f;
                barColor = Color.Lerp(midHealthColor, fullHealthColor, t);
                barColor.a = 1f;
            }
            else if (percent > 0.3f)
            {
                // ����� �� ������ (30~50%)
                float t = (percent - 0.3f) / 0.2f;
                barColor = Color.Lerp(lowHealthColor, midHealthColor, t);
                barColor.a = 1f;
            }
            else
            {
                // 30% ����: ������ ����, ������
                barColor = lowHealthColor;
                // ���İ��� �����Լ��� �ε巴�� ��ȭ (0.4~1.0)
                float blink = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
                barColor.a = blink;
            }

            curveBar.color = barColor;
        }
    }
}
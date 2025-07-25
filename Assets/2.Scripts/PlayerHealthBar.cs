using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform playerBody; // XR Origin �Ǵ� Origin ���� �� ������Ʈ (Inspector �Ҵ�)
    public PlayerHealth playerHealth;
    public Image curveBar;

    public Vector3 localOffset = new Vector3(0, 1.0f, 0.2f); // �㸮~���� ����, ������ �ణ

    // ����/������ ������ ����
    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);

    public float blinkAlphaMin = 0.4f;
    public float blinkAlphaMax = 1.0f;
    public float blinkSpeed = 2.5f;

    void LateUpdate()
    {
        if (playerBody != null)
        {
            // Origin�� �������� ������ ����
            transform.position = playerBody.TransformPoint(localOffset);

            // �׻� ī�޶�(�Ӹ�)�� �ٶ󺸰�
            if (Camera.main != null)
            {
                Vector3 dir = (transform.position - Camera.main.transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up) * Quaternion.Euler(90, 0, 0);
            }
        }

        // (�Ʒ� �κ��� ����)
        if (playerHealth != null && curveBar != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            float percent = Mathf.Clamp01(current / max);

            curveBar.fillAmount = percent;

            Color barColor;
            if (percent > 0.5f)
            {
                float t = (percent - 0.5f) / 0.5f;
                barColor = Color.Lerp(midHealthColor, fullHealthColor, t);
                barColor.a = 1f;
            }
            else if (percent > 0.3f)
            {
                float t = (percent - 0.3f) / 0.2f;
                barColor = Color.Lerp(lowHealthColor, midHealthColor, t);
                barColor.a = 1f;
            }
            else
            {
                barColor = lowHealthColor;
                float blink = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
                barColor.a = blink;
            }

            curveBar.color = barColor;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform playerBody; // XR Origin �Ǵ� Origin ���� �� ������Ʈ (Inspector �Ҵ�)
    public PlayerHealth playerHealth;
    public Image curveBar;

    public Vector3 localOffset = new Vector3(0, 1.0f, 0.2f);

    public Color fullHealthColor = Color.green;
    public Color midHealthColor = Color.yellow;
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);

    public float blinkAlphaMin = 0.4f;
    public float blinkAlphaMax = 1.0f;
    public float blinkSpeed = 2.5f;

    void LateUpdate()
    {
        // 1. ��ġ ������Ʈ
        if (playerBody != null)
            transform.position = playerBody.TransformPoint(localOffset);

        // 2. ī�޶��� Yaw�� ���󰡼� �ｺ�ٰ� Ƣ�� �ʰ� Billboard ������� ȸ��
        if (Camera.main != null)
        {
            // ī�޶��� forward���� y���� 0���� (���� ���⸸)
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            if (camForward.sqrMagnitude < 0.001f)
                camForward = Vector3.forward; // fallback

            Quaternion lookRot = Quaternion.LookRotation(camForward, Vector3.up);
            transform.rotation = lookRot;

            // �̹����� ���ų� ���������� �Ʒ�ó�� ȸ�� �߰�
            transform.Rotate(90, 0, -180); 
        }

        // 3. ü�� �� UI ����
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

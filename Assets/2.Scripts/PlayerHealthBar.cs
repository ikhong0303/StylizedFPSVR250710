using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Transform playerHead;
    public PlayerHealth playerHealth;
    public Image curveBar;

    public float distance = 1f;
    public float yourOffset = -1.5f; // 플레이어 머리 위로 올리는 높이

    // 색상 지정
    public Color fullHealthColor = Color.green;                  // 100% (녹색)
    public Color midHealthColor = Color.yellow;                  // 50% (노란색)
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);    // 0% (빨간색)

    // 깜빡임 설정
    public float blinkAlphaMin = 0.4f; // 최소 알파값
    public float blinkAlphaMax = 1.0f; // 최대 알파값
    public float blinkSpeed = 2.5f;    // 1초당 깜빡임 속도

    void LateUpdate()
    {
        // 위치 보정
        Vector3 camPos = playerHead.position;
        Vector3 forward = playerHead.forward; forward.y = 0f; forward.Normalize();
        Vector3 barPos = camPos + forward * distance + Vector3.up * yourOffset;
        transform.position = barPos;

        transform.rotation = Quaternion.LookRotation(forward, Vector3.up) * Quaternion.Euler(90, 0, 0);

        // 체력 연동 + 색상 그라데이션 + 깜빡임
        if (playerHealth != null && curveBar != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            float percent = Mathf.Clamp01(current / max);

            curveBar.fillAmount = percent;

            Color barColor;

            if (percent > 0.5f)
            {
                // 녹색 → 노란색 (50~100%)
                float t = (percent - 0.5f) / 0.5f;
                barColor = Color.Lerp(midHealthColor, fullHealthColor, t);
                barColor.a = 1f;
            }
            else if (percent > 0.3f)
            {
                // 노란색 → 빨간색 (30~50%)
                float t = (percent - 0.3f) / 0.2f;
                barColor = Color.Lerp(lowHealthColor, midHealthColor, t);
                barColor.a = 1f;
            }
            else
            {
                // 30% 이하: 빨간색 고정, 깜빡임
                barColor = lowHealthColor;
                // 알파값을 사인함수로 부드럽게 변화 (0.4~1.0)
                float blink = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
                barColor.a = blink;
            }

            curveBar.color = barColor;
        }
    }
}
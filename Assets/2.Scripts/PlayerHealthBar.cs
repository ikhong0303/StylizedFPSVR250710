using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 손목 위에 떠다니는 체력바. (양쪽에서 가운데로 닫히는 형태)
/// 좌우 90도 이미지(leftBar, rightBar)를 각각 제어하며,
/// 반투명 배경 이미지(backgroundImage)는 시각적 깊이감 연출용.
///

/// 체력바 위치, 회전, 색상, 깜빡임 등 세부 조정은 주석 참고!
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    [Header("위치 지정")]
    public Transform playerBody;    // XR Origin, 혹은 플레이어 본체(Inspector에서 직접 drag)
    public Vector3 localOffset = new Vector3(0, 1.0f, 0.2f); // 플레이어 기준 헬스바 위치(조정 가능)

    [Header("체력 데이터")]
    public PlayerHealth playerHealth; // 체력 참조(외부에서 할당, 혹은 GetComponent)

    [Header("이미지 분할 바 (좌/우 90도 각각)")]
    public Image leftBar;      // 왼쪽 90도(반원) 이미지 (Inspector에서 drag)
    public Image rightBar;     // 오른쪽 90도(반원) 이미지 (Inspector에서 drag)
    public Image backgroundImage; // (선택) 반투명 배경 이미지(깊이감 연출, 항상 아래에 오도록!)

    [Header("색상/효과 옵션")]
    public Color fullHealthColor = Color.green;                       // 체력 100% 색상
    public Color midHealthColor = Color.yellow;                       // 체력 50% 색상
    public Color lowHealthColor = new Color(1f, 0.25f, 0.1f);         // 체력 30% 이하 색상

    [Header("깜빡임 효과(위험시)")]
    [Range(0f, 1f)] public float blinkAlphaMin = 0.1f;   // 깜빡임시 최소 투명도
    [Range(0f, 1f)] public float blinkAlphaMax = 1.0f;   // 깜빡임시 최대 투명도
    public float blinkSpeed = 10f;                       // 깜빡임 속도 (높을수록 빨라짐)

    void LateUpdate()
    {
        // 1. 헬스바 위치: 플레이어 기준 Offset 적용 (팔, 머리 위 등 원하는 곳에 맞춰 조정)
        if (playerBody != null)
            transform.position = playerBody.TransformPoint(localOffset);

        // 2. 헬스바 회전: 카메라 방향만 따라가게(수평 billboard)
        if (Camera.main != null)
        {
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0; // 수평 방향만 반영(roll, pitch 무시)
            if (camForward.sqrMagnitude < 0.001f)
                camForward = Vector3.forward; // 혹시 카메라가 완전히 위/아래를 본다면 fallback

            Quaternion lookRot = Quaternion.LookRotation(camForward, Vector3.up);
            transform.rotation = lookRot;

            // 이미지가 눕거나 뒤집힌다면 이 회전값을 조절
            // (대부분의 경우 90, 0, -180 조합이 맞음, 직접 수정 가능!)
            transform.Rotate(90, 0, -180);
        }

        // 3. 체력 바 이미지 fillAmount/색상 등 갱신
        if (playerHealth != null && leftBar != null && rightBar != null)
        {
            float current = playerHealth.GetCurrentHealth();
            float max = playerHealth.GetMaxHealth();
            float percent = Mathf.Clamp01(current / max);

            // 양쪽에서 중앙으로 닫히는 효과:
            // fillAmount 1이면 모두 찬 상태, 0이면 완전 소진
            leftBar.fillAmount = percent;      // 좌측 fillAmount 조정(0~1)
            rightBar.fillAmount = percent;     // 우측 fillAmount 조정(0~1)

            // 체력 구간별 색상 조정 (필요시 구간 및 색상 커스텀)
            Color barColor;
            if (percent > 0.7f)
            {
                // 70%~100% 구간: full → mid 사이 색상
                float t = (percent - 0.7f) / 0.3f; // (0.7~1.0 → 0~1)
                barColor = Color.Lerp(midHealthColor, fullHealthColor, t);
                barColor.a = 1f;
            }
            else if (percent > 0.4f)
            {
                // 40%~70% 구간: low → mid 사이 색상
                float t = (percent - 0.4f) / 0.3f; // (0.4~0.7 → 0~1)
                barColor = Color.Lerp(lowHealthColor, midHealthColor, t);
                barColor.a = 1f;
            }
            else
            {
                // 0~40%: 위험, 저체력(깜빡임 적용)
                barColor = lowHealthColor;
                float blink = Mathf.Lerp(blinkAlphaMin, blinkAlphaMax, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
                barColor.a = blink;
            }
            leftBar.color = barColor;
            rightBar.color = barColor;

            // (참고) 만약 좌우 bar 색을 다르게 주고 싶으면 각각 따로 color 연산!
        }

        // 4. 반투명 배경 이미지는 Inspector에서 직접 설정해도 되지만,
        //     동적으로 색상/알파를 바꿀 경우 아래처럼!
        // if (backgroundImage != null)
        //     backgroundImage.color = new Color(0.15f, 0.15f, 0.15f, 0.5f); // (r,g,b,alpha) 원하는 값으로 조정
    }
}

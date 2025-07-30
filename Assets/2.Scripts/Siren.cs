using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Siren : MonoBehaviour
{
    [Header("Socket Interactors")]
    public XRSocketInteractor leftHolster;
    public XRSocketInteractor rightHolster;

    [Header("사이렌 사운드")]
    public AudioSource sirenAudioSource;

    [Header("경광등 관련")]
    public Transform emergencyLight;   // 경광등 전체(회전용, 빈 오브젝트로 만들어 SpotLight 2개 하위에 배치)
    public Light spotLightA;           // 첫 번째 SpotLight
    public Light spotLightB;           // 두 번째 SpotLight
    public float lightRotateSpeed = 200f;

    [Header("추가 깜빡임 Light")]
    public Light extraBlinkingLight;   // Inspector에서 연결 (예: 천장 조명 등)
    public float blinkingMin = 0f;     // 깜빡임 최소 Intensity
    public float blinkingMax = 10f;    // 깜빡임 최대 Intensity
    public float blinkSpeed = 4f;      // 깜빡임 속도 (Inspector에서 조절 가능)

    private bool leftFilled = false;
    private bool rightFilled = false;
    private bool sirenStarted = false;

    public bool BothHolstered => leftFilled && rightFilled;

    void OnEnable()
    {
        leftHolster.selectEntered.AddListener(OnLeftSocketFilled);
        leftHolster.selectExited.AddListener(OnLeftSocketEmptied);

        rightHolster.selectEntered.AddListener(OnRightSocketFilled);
        rightHolster.selectExited.AddListener(OnRightSocketEmptied);
    }

    void OnDisable()
    {
        leftHolster.selectEntered.RemoveListener(OnLeftSocketFilled);
        leftHolster.selectExited.RemoveListener(OnLeftSocketEmptied);

        rightHolster.selectEntered.RemoveListener(OnRightSocketFilled);
        rightHolster.selectExited.RemoveListener(OnRightSocketEmptied);
    }

    void OnLeftSocketFilled(SelectEnterEventArgs args)
    {
        leftFilled = true;
        CheckBothFilled();
    }

    void OnLeftSocketEmptied(SelectExitEventArgs args)
    {
        leftFilled = false;
        ResetSiren();
    }

    void OnRightSocketFilled(SelectEnterEventArgs args)
    {
        rightFilled = true;
        CheckBothFilled();
    }

    void OnRightSocketEmptied(SelectExitEventArgs args)
    {
        rightFilled = false;
        ResetSiren();
    }

    void CheckBothFilled()
    {
        if (leftFilled && rightFilled && !sirenStarted)
        {
            sirenStarted = true;
            // 사이렌 소리 시작
            if (sirenAudioSource != null && !sirenAudioSource.isPlaying)
                sirenAudioSource.Play();

            // SpotLight 켜기
            if (spotLightA != null) spotLightA.enabled = true;
            if (spotLightB != null) spotLightB.enabled = true;

            // Extra Light 깜빡임 시작 (intensity를 0으로 초기화)
            if (extraBlinkingLight != null)
                extraBlinkingLight.intensity = blinkingMin;
        }
    }

    void ResetSiren()
    {
        sirenStarted = false;
        // 사이렌 정지
        if (sirenAudioSource != null && sirenAudioSource.isPlaying)
            sirenAudioSource.Stop();

        // SpotLight 끄기
        if (spotLightA != null) spotLightA.enabled = false;
        if (spotLightB != null) spotLightB.enabled = false;

        // Extra Light도 intensity 0 & 꺼두기
        if (extraBlinkingLight != null)
            extraBlinkingLight.intensity = blinkingMin;
    }

    void Update()
    {
        // 경광등 회전
        if (sirenStarted && emergencyLight != null)
            emergencyLight.Rotate(Vector3.up, lightRotateSpeed * Time.deltaTime);

        // 추가 깜빡임 라이트 intensity 조절
        if (extraBlinkingLight != null)
        {
            if (sirenStarted)
            {
                // 사인 곡선으로 intensity를 min~max 사이로 부드럽게 변화
                float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f; // 0~1
                extraBlinkingLight.intensity = Mathf.Lerp(blinkingMin, blinkingMax, t);
            }
            else
            {
                extraBlinkingLight.intensity = blinkingMin;
            }
        }
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Siren : MonoBehaviour
{
    [Header("Socket Interactors")]
    public XRSocketInteractor leftHolster;
    public XRSocketInteractor rightHolster;

    [Header("���̷� ����")]
    public AudioSource sirenAudioSource;

    [Header("�汤�� ����")]
    public Transform emergencyLight;   // �汤�� ��ü(ȸ����, �� ������Ʈ�� ����� SpotLight 2�� ������ ��ġ)
    public Light spotLightA;           // ù ��° SpotLight
    public Light spotLightB;           // �� ��° SpotLight
    public float lightRotateSpeed = 200f;

    [Header("�߰� ������ Light")]
    public Light extraBlinkingLight;   // Inspector���� ���� (��: õ�� ���� ��)
    public float blinkingMin = 0f;     // ������ �ּ� Intensity
    public float blinkingMax = 10f;    // ������ �ִ� Intensity
    public float blinkSpeed = 4f;      // ������ �ӵ� (Inspector���� ���� ����)

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
            // ���̷� �Ҹ� ����
            if (sirenAudioSource != null && !sirenAudioSource.isPlaying)
                sirenAudioSource.Play();

            // SpotLight �ѱ�
            if (spotLightA != null) spotLightA.enabled = true;
            if (spotLightB != null) spotLightB.enabled = true;

            // Extra Light ������ ���� (intensity�� 0���� �ʱ�ȭ)
            if (extraBlinkingLight != null)
                extraBlinkingLight.intensity = blinkingMin;
        }
    }

    void ResetSiren()
    {
        sirenStarted = false;
        // ���̷� ����
        if (sirenAudioSource != null && sirenAudioSource.isPlaying)
            sirenAudioSource.Stop();

        // SpotLight ����
        if (spotLightA != null) spotLightA.enabled = false;
        if (spotLightB != null) spotLightB.enabled = false;

        // Extra Light�� intensity 0 & ���α�
        if (extraBlinkingLight != null)
            extraBlinkingLight.intensity = blinkingMin;
    }

    void Update()
    {
        // �汤�� ȸ��
        if (sirenStarted && emergencyLight != null)
            emergencyLight.Rotate(Vector3.up, lightRotateSpeed * Time.deltaTime);

        // �߰� ������ ����Ʈ intensity ����
        if (extraBlinkingLight != null)
        {
            if (sirenStarted)
            {
                // ���� ����� intensity�� min~max ���̷� �ε巴�� ��ȭ
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

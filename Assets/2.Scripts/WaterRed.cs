#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WaterRed : MonoBehaviour
{
    [Header("Target Material (UberStylizedWater)")]
    public Material waterMaterial;

    [Header("Red Effect Settings")]
    public Color shallowTarget = new Color(1, 0.2f, 0.2f, 1);
    public Color deepTarget = new Color(0.8f, 0, 0, 1);
    public Color foamTarget = new Color(1, 0.2f, 0.2f, 1);
    public Color intersectionTarget = new Color(1, 0.2f, 0.2f, 1);
    public float lerpDuration = 2f;

    [Header("테스트용 (Inspector 체크박스)")]
    public bool triggerRed = false;

    // 내부 상태
    private Color shallowInit, deepInit, foamInit, intersectionInit;
    private float strengthInit = 0f;
    private bool initialized = false;
    private bool isLerping = false;
    private float elapsed = 0f;

    private Color fromShallow, fromDeep, fromFoam, fromIntersection;
    private Color toShallow, toDeep, toFoam, toIntersection;
    private float fromStrength;
    private float toStrength;

    private bool isPlaying = false;

    void Awake()
    {
        CacheInitialValues();
    }

    void Start()
    {
        CacheInitialValues();
    }

    void CacheInitialValues()
    {
        if (waterMaterial && !initialized)
        {
            shallowInit = waterMaterial.GetColor("_Color_Shallow");
            deepInit = waterMaterial.GetColor("_Color_Deep");
            foamInit = waterMaterial.GetColor("_SurfFoam_Color");
            intersectionInit = waterMaterial.GetColor("_InterSec_Color");
            strengthInit = waterMaterial.GetFloat("_SurfaceDistortion_Strength");
            initialized = true;
        }
    }

    void FixedUpdate()
    {
        if (!initialized) CacheInitialValues();

        if (triggerRed)
        {
            Play();
            triggerRed = false;
        }

        if (isLerping)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);

            waterMaterial.SetColor("_Color_Shallow", Color.Lerp(fromShallow, toShallow, t));
            waterMaterial.SetColor("_Color_Deep", Color.Lerp(fromDeep, toDeep, t));
            waterMaterial.SetColor("_SurfFoam_Color", Color.Lerp(fromFoam, toFoam, t));
            waterMaterial.SetColor("_InterSec_Color", Color.Lerp(fromIntersection, toIntersection, t));
            waterMaterial.SetFloat("_SurfaceDistortion_Strength", Mathf.Lerp(fromStrength, toStrength, t));

            if (t >= 1f)
                isLerping = false;
        }
    }

    void StartLerp(
        Color startShallow, Color startDeep, Color startFoam, Color startIntersection,
        Color endShallow, Color endDeep, Color endFoam, Color endIntersection,
        float startStrength, float endStrength)
    {
        fromShallow = startShallow;
        fromDeep = startDeep;
        fromFoam = startFoam;
        fromIntersection = startIntersection;

        toShallow = endShallow;
        toDeep = endDeep;
        toFoam = endFoam;
        toIntersection = endIntersection;

        fromStrength = startStrength;
        toStrength = endStrength;

        elapsed = 0f;
        isLerping = true;
    }

    // 빨간색으로 자연스럽게 변화 시작 + Distortion Strength 보간
    public void Play()
    {
        if (!isPlaying)
        {
            CacheInitialValues();
            StartLerp(
                waterMaterial.GetColor("_Color_Shallow"),
                waterMaterial.GetColor("_Color_Deep"),
                waterMaterial.GetColor("_SurfFoam_Color"),
                waterMaterial.GetColor("_InterSec_Color"),
                shallowTarget,
                deepTarget,
                foamTarget,
                intersectionTarget,
                waterMaterial.GetFloat("_SurfaceDistortion_Strength"),
                6f
            );
            isPlaying = true;
        }
    }

    // 원래 색상과 Strength로 자연스럽게 복원
    public void Stop()
    {
        if (isPlaying)
        {
            StartLerp(
                waterMaterial.GetColor("_Color_Shallow"),
                waterMaterial.GetColor("_Color_Deep"),
                waterMaterial.GetColor("_SurfFoam_Color"),
                waterMaterial.GetColor("_InterSec_Color"),
                shallowInit,
                deepInit,
                foamInit,
                intersectionInit,
                waterMaterial.GetFloat("_SurfaceDistortion_Strength"),
                strengthInit
            );
            isPlaying = false;
        }
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            RestoreOriginalValuesImmediate();
        }
    }
#endif

    private void RestoreOriginalValuesImmediate()
    {
        if (waterMaterial == null) return;

        waterMaterial.SetColor("_Color_Shallow", shallowInit);
        waterMaterial.SetColor("_Color_Deep", deepInit);
        waterMaterial.SetColor("_SurfFoam_Color", foamInit);
        waterMaterial.SetColor("_InterSec_Color", intersectionInit);
        waterMaterial.SetFloat("_SurfaceDistortion_Strength", strengthInit);

        isPlaying = false;
        isLerping = false;
    }
}

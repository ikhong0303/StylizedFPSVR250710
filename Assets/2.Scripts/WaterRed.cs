using UnityEngine;

public class WaterRed : MonoBehaviour
{
    [Header("Target Material (UberStylizedWater)")]
    public Material waterMaterial;

    [Header("Red Effect Settings")]
    public Color shallowTarget = new Color(1, 0.2f, 0.2f, 1);
    public Color deepTarget = new Color(0.8f, 0, 0, 1);
    public Color foamTarget = new Color(1, 0.2f, 0.2f, 1);
    public Color intersectionTarget = new Color(1, 0.2f, 0.2f, 1); // Intersection용
    public float lerpDuration = 2f;

    [Header("테스트용(Inspector 체크박스)")]
    public bool triggerRed = false;
    public bool triggerRestore = false;

    // 내부 상태
    private Color shallowInit, deepInit, foamInit, intersectionInit;
    private bool initialized = false;
    private bool isLerping = false;
    private float elapsed = 0f;
    private Color fromShallow, fromDeep, fromFoam, fromIntersection;
    private Color toShallow, toDeep, toFoam, toIntersection;

    void Awake()
    {
        CacheInitialColors();
    }
    void Start()
    {
        CacheInitialColors();
    }

    void CacheInitialColors()
    {
        if (waterMaterial && !initialized)
        {
            shallowInit = waterMaterial.GetColor("_Color_Shallow");
            deepInit = waterMaterial.GetColor("_Color_Deep");
            foamInit = waterMaterial.GetColor("_SurfFoam_Color");
            intersectionInit = waterMaterial.GetColor("_InterSec_Color");
            initialized = true;
        }
    }

    void Update()
    {
        if (!initialized) CacheInitialColors();

        if (triggerRed)
        {
            StartLerp(
                waterMaterial.GetColor("_Color_Shallow"),
                waterMaterial.GetColor("_Color_Deep"),
                waterMaterial.GetColor("_SurfFoam_Color"),
                waterMaterial.GetColor("_InterSec_Color"),
                shallowTarget,
                deepTarget,
                foamTarget,
                intersectionTarget
            );
            triggerRed = false;
        }
        if (triggerRestore)
        {
            StartLerp(
                waterMaterial.GetColor("_Color_Shallow"),
                waterMaterial.GetColor("_Color_Deep"),
                waterMaterial.GetColor("_SurfFoam_Color"),
                waterMaterial.GetColor("_InterSec_Color"),
                shallowInit,
                deepInit,
                foamInit,
                intersectionInit
            );
            triggerRestore = false;
        }

        if (isLerping)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);
            waterMaterial.SetColor("_Color_Shallow", Color.Lerp(fromShallow, toShallow, t));
            waterMaterial.SetColor("_Color_Deep", Color.Lerp(fromDeep, toDeep, t));
            waterMaterial.SetColor("_SurfFoam_Color", Color.Lerp(fromFoam, toFoam, t));
            waterMaterial.SetColor("_InterSec_Color", Color.Lerp(fromIntersection, toIntersection, t));
            if (t >= 1f)
                isLerping = false;
        }
    }

    void StartLerp(
        Color startShallow, Color startDeep, Color startFoam, Color startIntersection,
        Color endShallow, Color endDeep, Color endFoam, Color endIntersection)
    {
        fromShallow = startShallow;
        fromDeep = startDeep;
        fromFoam = startFoam;
        fromIntersection = startIntersection;

        toShallow = endShallow;
        toDeep = endDeep;
        toFoam = endFoam;
        toIntersection = endIntersection;

        elapsed = 0f;
        isLerping = true;
    }
}

using UnityEngine;
using System.Collections;

public class LabLight : MonoBehaviour
{
    public Light targetLight;         // Inspector에서 Point Light를 드래그!
    public float minIntensity = 0f;   // 최소 값
    public float maxIntensity = 1f;   // 최대 값
    public float duration = 0f;       // 오르내리는데 걸리는 시간

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        StartCoroutine(IntensityBlinkCoroutine());
    }

    IEnumerator IntensityBlinkCoroutine()
    {
        while (true)
        {
            // 1. 점점 밝아짐 (0 → 1)
            yield return StartCoroutine(LerpIntensity(minIntensity, maxIntensity, duration));
            yield return new WaitForSeconds(0.2f); // 밝아진 후 잠시 대기
            // 2. 점점 어두워짐 (1 → 0)
            yield return StartCoroutine(LerpIntensity(maxIntensity, minIntensity, duration));
            yield return new WaitForSeconds(0.1f); // 밝아진 후 잠시 대기
            // 1. 점점 밝아짐 (0 → 1)
            yield return StartCoroutine(LerpIntensity(minIntensity, maxIntensity, duration));
            yield return new WaitForSeconds(0.4f); // 밝아진 후 잠시 대기
            // 2. 점점 어두워짐 (1 → 0)
            yield return StartCoroutine(LerpIntensity(maxIntensity, minIntensity, duration));
            yield return new WaitForSeconds(0.2f); // 밝아진 후 잠시 대기
            yield return StartCoroutine(LerpIntensity(minIntensity, maxIntensity, duration));
            yield return new WaitForSeconds(1f); // 밝아진 후 잠시 대기
            // 2. 점점 어두워짐 (1 → 0)
            yield return StartCoroutine(LerpIntensity(maxIntensity, minIntensity, duration));
            yield return new WaitForSeconds(1f); // 밝아진 후 잠시 대기


        }
    }

    IEnumerator LerpIntensity(float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            float t = elapsed / time;
            targetLight.intensity = Mathf.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        targetLight.intensity = to; // 마지막 값 보정
    }
}

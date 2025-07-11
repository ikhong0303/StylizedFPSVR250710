using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitEffect : MonoBehaviour
{
    [SerializeField] private Image hitImage; // 인스펙터에서 할당

    private Coroutine hitRoutine;

    // 데미지 받으면 이 함수 호출!
    public void ShowHitEffect(float duration = 0.3f, float alpha = 0.5f)
    {
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitEffectRoutine(duration, alpha));
    }

    private IEnumerator HitEffectRoutine(float duration, float alpha)
    {
        // 바로 빨갛게!
        hitImage.color = new Color(1, 0, 0, alpha);

        float t = 0;
        Color start = hitImage.color;
        Color end = new Color(1, 0, 0, 0);

        while (t < duration)
        {
            hitImage.color = Color.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        hitImage.color = end;
    }
}

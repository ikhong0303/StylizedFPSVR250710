using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHitEffect : MonoBehaviour
{
    [SerializeField] private Image hitImage; // �ν����Ϳ��� �Ҵ�

    private Coroutine hitRoutine;

    // ������ ������ �� �Լ� ȣ��!
    public void ShowHitEffect(float duration = 0.3f, float alpha = 0.5f)
    {
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);

        hitRoutine = StartCoroutine(HitEffectRoutine(duration, alpha));
    }

    private IEnumerator HitEffectRoutine(float duration, float alpha)
    {
        // �ٷ� ������!
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

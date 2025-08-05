using System.Collections;
using UnityEngine;

public class BossFlashEffect : MonoBehaviour
{
    [Header("적용할 스키닝 메쉬 렌더러 (Boss 모델의 바디)")]
    [SerializeField] private SkinnedMeshRenderer bodyRenderer;

    [Header("플래시 색상")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (bodyRenderer != null)
            originalColor = bodyRenderer.material.color;
    }

    public void Flash()
    {
        if (bodyRenderer == null) return;

        // 중복 플래시 방지
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        Material mat = bodyRenderer.material; // 인스턴스화 보장
        mat.color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        mat.color = originalColor;
        flashCoroutine = null;
    }
}

using System.Collections;
using UnityEngine;

public class FishFlashEffect : MonoBehaviour
{
    [Header("피격 컬러 연출")]
    [SerializeField] private Renderer fishRenderer; // MeshRenderer 또는 SkinnedMeshRenderer
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.15f;

    private Color[] originalColors;
    private Material[] materials;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (fishRenderer == null)
            fishRenderer = GetComponentInChildren<Renderer>();

        if (fishRenderer != null)
        {
            materials = fishRenderer.materials;
            originalColors = new Color[materials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                originalColors[i] = materials[i].color;
            }
        }
    }

    public void Flash()
    {
        if (materials == null || materials.Length == 0) return;

        // 중복 코루틴 방지
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 모든 머티리얼을 hitColor로
        for (int i = 0; i < materials.Length; i++)
            materials[i].color = hitColor;

        yield return new WaitForSeconds(flashDuration);

        // 모든 머티리얼 원래 컬러로 복구
        for (int i = 0; i < materials.Length; i++)
            materials[i].color = originalColors[i];

        flashCoroutine = null;
    }
}

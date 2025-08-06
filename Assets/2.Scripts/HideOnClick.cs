using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class HideOnClick : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject targetObj;

    [Header("타이틀 페이드용")]
    [SerializeField] private Image titleImage;               // 타이틀 이미지
    [SerializeField] private TMP_Text pressStartText;        // Press Start TMP_Text
    [SerializeField] private float fadeDuration = 3f;

    public event Action OnHide;

    private Coroutine blinkCoroutine;

    void Awake()
    {
        if (button != null)
            button.onClick.AddListener(HideTarget);
    }

    void Start()
    {
        // 타이틀 이미지는 투명하게 시작
        if (titleImage != null)
        {
            var col = titleImage.color;
            col.a = 0f;
            titleImage.color = col;
            StartCoroutine(FadeInTitle());
        }

        // Press Start도 투명하게 시작
        if (pressStartText != null)
        {
            var col = pressStartText.color;
            col.a = 0f;
            pressStartText.color = col;
        }
    }

    IEnumerator FadeInTitle()
    {
        BgmManager.Instance.PlayBGM("Bgm04"); // BGM 재생
        yield return new WaitForSeconds(5f);

        float t = 0;
        var col = titleImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            titleImage.color = new Color(col.r, col.g, col.b, a);
            yield return null;
        }
        titleImage.color = new Color(col.r, col.g, col.b, 1f);

        // 타이틀이 다 나오면 Press Start 깜빡이 시작
        if (pressStartText != null)
            blinkCoroutine = StartCoroutine(BlinkPressStart());
    }

    IEnumerator BlinkPressStart()
    {
        yield return new WaitForSeconds(1.5f);


        float blinkInterval = 0.6f; // 깜빡임 간격

        var col = pressStartText.color;
        while (true)
        {
            // 보이게
            pressStartText.color = new Color(col.r, col.g, col.b, 1f);
            yield return new WaitForSeconds(blinkInterval);

            // 안보이게
            pressStartText.color = new Color(col.r, col.g, col.b, 0f);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    void HideTarget()
    {
        // 깜빡이 코루틴 중지
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        if (targetObj != null)
            targetObj.SetActive(false);

        BgmManager.Instance.StopBGM();


        OnHide?.Invoke();
    }
}

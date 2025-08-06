using UnityEngine;
using System.Collections;
using MikeNspired.XRIStarterKit;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StageManager3 : MonoBehaviour
{
    [SerializeField] private FishDeath fishDeath;
    [SerializeField] private GameObject missionPopupPanel;
    [SerializeField] private GameObject naviLine;
    [SerializeField] private WaterRed waterRed;
    [SerializeField] private GameObject CloudEffect;
    [SerializeField] private ParticleSystem particleSystemTarget;
    [SerializeField] GameObject bossObject;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Image blackImage; // Inspector에서 연결

    [Header("비디오 재생 세팅")]
    public VideoPlayer videoPlayer;   // Inspector에서 할당 (VideoManager의 VideoPlayer)
    public VideoClip videoClip;       // Inspector에서 할당 (Assets의 mp4)
    public GameObject videoQuad;      // Inspector에서 할당 (영상 출력할 Quad)
    private bool hasPlayed = false;   // 한 번만 재생 (중복 방지)


    private void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            Transform slots = gm.transform.Find("Slots");
            if (slots != null)
            {
                Transform mission = slots.Find("Mission");
                if (mission != null)
                {
                    missionPopupPanel = mission.gameObject;
                }
            }
        }

        if (fishDeath != null)
            fishDeath.OnSinkStart += OnFishSinkStart;
        if (bossHealth != null)
            bossHealth.OnDeath += OnBossDeath;

        StartCoroutine(StageFlow());
    }

    private IEnumerator StageFlow()
    {

        yield return new WaitForSeconds(3f);
        BgmManager.Instance.PlayBGM("Bgm05");
        StartCoroutine(FadeToAlpha(0f, 4f));
        yield return new WaitForSeconds(3f);

        if (missionPopupPanel != null)
            missionPopupPanel.SetActive(true);
        if (naviLine != null)
            naviLine.SetActive(true);

        yield return new WaitForSeconds(2f);

        AudioManager3.Instance.PlayNarration("GardenNarr01");
    }


    public IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        if (blackImage == null) yield break;

        Color color = blackImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            blackImage.color = color;
            yield return null;
        }
        // 마지막 값 보정
        color.a = targetAlpha;
        blackImage.color = color;
    }
    private void OnFishSinkStart()
    {
        StartCoroutine(PlayDeathSFXSequence());
    }

    private IEnumerator PlayDeathSFXSequence()
    {
        waterRed.triggerRed = true;

        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Wind");
        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Clood");
        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Thunder");


        CloudEffect.SetActive(true);
        yield return new WaitForSeconds(5f);
        AudioManager3.Instance.PlayNarration("GardenNarr03");
        BgmManager.Instance.PlayBGM("Bgm06");
        yield return new WaitForSeconds(11.8f);

        bossObject.SetActive(true);
        yield return new WaitForSeconds(12f);

        var main = particleSystemTarget.main;
        Color c = main.startColor.color;
        c.a = 0.15f;
        main.startColor = c;


    }

    private void OnBossDeath()
    {
        StartCoroutine(BossDeathSequence());
    }

    private IEnumerator BossDeathSequence()
    {
        BgmManager.Instance.StopBGM();

        yield return new WaitForSeconds(8f);
        AudioManager3.Instance.PlayNarration("GardenNarr02");
        yield return new WaitForSeconds(8f);
        StartCoroutine(FadeToAlpha2(1f, 4f));
        yield return new WaitForSeconds(5f);


        if (videoClip != null && videoPlayer != null)
        {
            videoPlayer.clip = videoClip;
            // 영상 출력 Quad를 보이게 (비활성화 상태였다면)
            if (videoQuad != null) videoQuad.SetActive(true);
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;  
        }

        yield break; 
    }

    public IEnumerator FadeToAlpha2(float targetAlpha, float duration)
    {
        if (blackImage == null) yield break;

        Color color = blackImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            blackImage.color = color;
            yield return null;
        }
        // 마지막 값 보정
        color.a = targetAlpha;
        blackImage.color = color;
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        // 영상 Quad 숨김 (필요시)
        if (videoQuad != null) videoQuad.SetActive(false);

    }

    private void OnDestroy()
    {
        if (fishDeath != null)
            fishDeath.OnSinkStart -= OnFishSinkStart;
        if (bossHealth != null)
            bossHealth.OnDeath -= OnBossDeath;
    }
}

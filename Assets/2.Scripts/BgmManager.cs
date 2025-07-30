using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 싱글톤 BgmManager
/// - BGM 데이터를 ID로 관리(Inspector에서 리스트로 추가/편집)
/// - PlayBGM("ID")로 재생, 자연스러운 페이드 지원
/// - StopBGM()로 정지, 페이드아웃 지원
/// - 씬 전환마다 교체하거나, DontDestroyOnLoad로 유지해서 글로벌 관리 가능
/// </summary>
public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

    [System.Serializable]
    public class BGMData
    {
        public string bgmID;                 // 인스펙터에서 식별/검색용 ID
        public AudioClip clip;               // 실제 BGM 오디오 파일
        [Range(0f, 1f)] public float volume = 1f; // 곡별 볼륨(개별 조절)
    }

    [Header("BGM 목록(Inspector에서 추가/정렬)")]
    public List<BGMData> bgmList = new List<BGMData>(); // 편집기로 리스트 관리
    private Dictionary<string, BGMData> bgmDict;        // 런타임 빠른 검색용

    [Header("BGM 전용 AudioSource")]
    public AudioSource bgmSource;   // 배경음만 담당하는 AudioSource

    [Header("페이드 옵션")]
    public float fadeDuration = 1.0f; // 페이드 in/out 소요 시간(초)

    private Coroutine fadeCoroutine;   // 중복 Fade 방지용 코루틴 핸들

    // 싱글톤/딕셔너리 세팅
    void Awake()
    {
        // 싱글톤 패턴(중복 방지)
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // 필요하면 DontDestroyOnLoad(gameObject); 도 여기서!

        // 런타임 ID 검색용 딕셔너리 세팅
        bgmDict = new Dictionary<string, BGMData>();
        foreach (var b in bgmList)
            bgmDict[b.bgmID] = b;
    }

    /// <summary>
    /// ID로 BGM을 재생. 이미 재생 중이면 교체, fade로 자연스러운 전환 지원
    /// </summary>
    /// <param name="id">BGM ID</param>
    /// <param name="fade">true면 페이드 전환, false면 즉시</param>
    public void PlayBGM(string id, bool fade = true)
    {
        if (bgmDict.TryGetValue(id, out var data) && data.clip != null)
        {
            if (fade)
            {
                // 기존 Fade가 진행 중이면 정지
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeToBGM(data.clip, data.volume));
            }
            else
            {
                bgmSource.clip = data.clip;
                bgmSource.volume = data.volume;
                bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"[BGMManager] ID '{id}' not found!");
        }
    }

    /// <summary>
    /// 페이드 아웃 → 새 곡으로 페이드 인
    /// </summary>
    IEnumerator FadeToBGM(AudioClip newClip, float newVolume)
    {
        // 페이드 아웃
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        // 페이드 인
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, newVolume, t / fadeDuration);
            yield return null;
        }
        bgmSource.volume = newVolume;
    }

    /// <summary>
    /// BGM 정지(페이드 가능)
    /// </summary>
    public void StopBGM(bool fade = true)
    {
        if (fade)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAndStop());
        }
        else
        {
            bgmSource.Stop();
        }
    }

    /// <summary>
    /// 페이드아웃 후 완전히 정지
    /// </summary>
    IEnumerator FadeOutAndStop()
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }
        bgmSource.Stop();
    }
}

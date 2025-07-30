using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// �̱��� BgmManager
/// - BGM �����͸� ID�� ����(Inspector���� ����Ʈ�� �߰�/����)
/// - PlayBGM("ID")�� ���, �ڿ������� ���̵� ����
/// - StopBGM()�� ����, ���̵�ƿ� ����
/// - �� ��ȯ���� ��ü�ϰų�, DontDestroyOnLoad�� �����ؼ� �۷ι� ���� ����
/// </summary>
public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

    [System.Serializable]
    public class BGMData
    {
        public string bgmID;                 // �ν����Ϳ��� �ĺ�/�˻��� ID
        public AudioClip clip;               // ���� BGM ����� ����
        [Range(0f, 1f)] public float volume = 1f; // � ����(���� ����)
    }

    [Header("BGM ���(Inspector���� �߰�/����)")]
    public List<BGMData> bgmList = new List<BGMData>(); // ������� ����Ʈ ����
    private Dictionary<string, BGMData> bgmDict;        // ��Ÿ�� ���� �˻���

    [Header("BGM ���� AudioSource")]
    public AudioSource bgmSource;   // ������� ����ϴ� AudioSource

    [Header("���̵� �ɼ�")]
    public float fadeDuration = 1.0f; // ���̵� in/out �ҿ� �ð�(��)

    private Coroutine fadeCoroutine;   // �ߺ� Fade ������ �ڷ�ƾ �ڵ�

    // �̱���/��ųʸ� ����
    void Awake()
    {
        // �̱��� ����(�ߺ� ����)
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // �ʿ��ϸ� DontDestroyOnLoad(gameObject); �� ���⼭!

        // ��Ÿ�� ID �˻��� ��ųʸ� ����
        bgmDict = new Dictionary<string, BGMData>();
        foreach (var b in bgmList)
            bgmDict[b.bgmID] = b;
    }

    /// <summary>
    /// ID�� BGM�� ���. �̹� ��� ���̸� ��ü, fade�� �ڿ������� ��ȯ ����
    /// </summary>
    /// <param name="id">BGM ID</param>
    /// <param name="fade">true�� ���̵� ��ȯ, false�� ���</param>
    public void PlayBGM(string id, bool fade = true)
    {
        if (bgmDict.TryGetValue(id, out var data) && data.clip != null)
        {
            if (fade)
            {
                // ���� Fade�� ���� ���̸� ����
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
    /// ���̵� �ƿ� �� �� ������ ���̵� ��
    /// </summary>
    IEnumerator FadeToBGM(AudioClip newClip, float newVolume)
    {
        // ���̵� �ƿ�
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

        // ���̵� ��
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, newVolume, t / fadeDuration);
            yield return null;
        }
        bgmSource.volume = newVolume;
    }

    /// <summary>
    /// BGM ����(���̵� ����)
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
    /// ���̵�ƿ� �� ������ ����
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

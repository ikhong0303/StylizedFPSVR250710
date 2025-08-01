using UnityEngine;
using System.Collections.Generic;

public class AudioManager3 : MonoBehaviour
{
    public static AudioManager3 Instance { get; private set; }

    [System.Serializable]
    public class SoundData
    {
        public string soundID;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("효과음 목록")]
    public List<SoundData> sfxList = new List<SoundData>();
    [Header("나레이션 목록")]
    public List<SoundData> narrationList = new List<SoundData>();

    private Dictionary<string, SoundData> sfxDict;
    private Dictionary<string, SoundData> narrationDict;

    // 여러 AudioSource를 리스트로 관리 (SFX 전용)
    public List<AudioSource> sfxSources = new List<AudioSource>();

    // 나레이션용 AudioSource는 단일 유지
    public AudioSource narrationSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sfxDict = new Dictionary<string, SoundData>();
            foreach (var s in sfxList)
                sfxDict[s.soundID] = s;

            narrationDict = new Dictionary<string, SoundData>();
            foreach (var n in narrationList)
                narrationDict[n.soundID] = n;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string id)
    {
        if (sfxDict.TryGetValue(id, out var data) && data.clip != null)
        {
            AudioSource availableSource = null;

            // 재생 중이지 않은 AudioSource 찾기
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                {
                    availableSource = source;
                    break;
                }
            }

            if (availableSource != null)
            {
                availableSource.PlayOneShot(data.clip, data.volume);
            }
            else
            {
                Debug.LogWarning("[AudioManager3] 모든 SFX AudioSource가 사용 중입니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager3] SFX '{id}' not found!");
        }
    }

    public void PlayNarration(string id)
    {
        if (narrationDict.TryGetValue(id, out var data) && data.clip != null)
        {
            if (narrationSource.isPlaying)
                narrationSource.Stop();
            narrationSource.clip = data.clip;
            narrationSource.volume = data.volume;
            narrationSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager3] Narration '{id}' not found!");
        }
    }

    public bool IsNarrationPlaying() => narrationSource.isPlaying;
}

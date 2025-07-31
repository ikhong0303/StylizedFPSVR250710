using UnityEngine;
using System.Collections.Generic;

public class AudioManager2 : MonoBehaviour
{
    public static AudioManager2 Instance { get; private set; }

    [System.Serializable]
    public class SoundData
    {
        public string soundID;      // ID (예: "narration_stage2_01", "sfx2_door", ...)
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("효과음 목록")]
    public List<SoundData> sfxList = new List<SoundData>();
    [Header("나레이션 목록")]
    public List<SoundData> narrationList = new List<SoundData>();

    private Dictionary<string, SoundData> sfxDict;
    private Dictionary<string, SoundData> narrationDict;

    public AudioSource sfxSource;         // 효과음 재생용
    public AudioSource narrationSource;   // 나레이션 전용

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxDict = new Dictionary<string, SoundData>();
        foreach (var s in sfxList)
            sfxDict[s.soundID] = s;

        narrationDict = new Dictionary<string, SoundData>();
        foreach (var n in narrationList)
            narrationDict[n.soundID] = n;
    }

    // 효과음
    public void PlaySFX(string id)
    {
        if (sfxDict.TryGetValue(id, out var data) && data.clip != null)
        {
            sfxSource.PlayOneShot(data.clip, data.volume);
        }
        else
        {
            Debug.LogWarning($"[AudioManager2] SFX '{id}' not found!");
        }
    }

    // 나레이션 (중복 방지)
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
            Debug.LogWarning($"[AudioManager2] Narration '{id}' not found!");
        }
    }

    // 나레이션 재생 여부
    public bool IsNarrationPlaying() => narrationSource.isPlaying;
}

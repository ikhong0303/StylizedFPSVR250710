using UnityEngine;
using System.Collections.Generic;

public class AudioManager2 : MonoBehaviour
{
    public static AudioManager2 Instance { get; private set; }

    [System.Serializable]
    public class SoundData
    {
        public string soundID;      // ID (��: "narration_stage2_01", "sfx2_door", ...)
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("ȿ���� ���")]
    public List<SoundData> sfxList = new List<SoundData>();
    [Header("�����̼� ���")]
    public List<SoundData> narrationList = new List<SoundData>();

    private Dictionary<string, SoundData> sfxDict;
    private Dictionary<string, SoundData> narrationDict;

    public AudioSource sfxSource;         // ȿ���� �����
    public AudioSource narrationSource;   // �����̼� ����

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

    // ȿ����
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

    // �����̼� (�ߺ� ����)
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

    // �����̼� ��� ����
    public bool IsNarrationPlaying() => narrationSource.isPlaying;
}

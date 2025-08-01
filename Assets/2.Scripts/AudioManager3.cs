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

    [Header("ȿ���� ���")]
    public List<SoundData> sfxList = new List<SoundData>();
    [Header("�����̼� ���")]
    public List<SoundData> narrationList = new List<SoundData>();

    private Dictionary<string, SoundData> sfxDict;
    private Dictionary<string, SoundData> narrationDict;

    // ���� AudioSource�� ����Ʈ�� ���� (SFX ����)
    public List<AudioSource> sfxSources = new List<AudioSource>();

    // �����̼ǿ� AudioSource�� ���� ����
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

            // ��� ������ ���� AudioSource ã��
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
                Debug.LogWarning("[AudioManager3] ��� SFX AudioSource�� ��� ���Դϴ�.");
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

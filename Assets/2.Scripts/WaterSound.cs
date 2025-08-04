using UnityEngine;

/// <summary>
/// Bullet 태그 오브젝트가 트리거에 닿으면 사운드 재생.
/// AudioSource는 Inspector에서 연결.
/// </summary>
public class WaterSound : MonoBehaviour
{
    [Tooltip("재생할 사운드 AudioSource (Inspector에서 연결)")]
    public AudioSource soundSource;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("Bullet이 닿음! 물소리 재생");
            if (soundSource != null)
            {
                soundSource.Play();
            }
            else
            {
                Debug.LogWarning("WaterSound: soundSource가 연결되어 있지 않습니다!");
            }
        }
    }
}

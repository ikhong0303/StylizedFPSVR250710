using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤

    public GameObject player; // 플레이어 오브젝트

    void Awake()
    {
        // 싱글톤 패턴: 이미 있으면 자신 삭제
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
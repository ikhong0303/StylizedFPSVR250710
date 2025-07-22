using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱���

    public GameObject player; // �÷��̾� ������Ʈ

    void Awake()
    {
        // �̱��� ����: �̹� ������ �ڽ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ����!
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class Player : MonoBehaviour
{
    void Awake()
    {
        // GameManager�� �ڱ� �ڽ� ���
        GameManager.Instance.player = this.gameObject;
    }
}

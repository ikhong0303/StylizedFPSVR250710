using UnityEngine;

public class Player : MonoBehaviour
{
    void Awake()
    {
        // GameManager에 자기 자신 등록
        GameManager.Instance.player = this.gameObject;
    }
}

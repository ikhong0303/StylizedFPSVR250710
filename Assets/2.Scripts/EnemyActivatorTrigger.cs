using UnityEngine;

public class EnemyActivatorTrigger : MonoBehaviour
{
    [Header("활성화할 적 오브젝트들")]
    public GameObject[] enemiesToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var enemy in enemiesToActivate)
        {
            if (enemy != null) enemy.SetActive(true);
        }
        // 트리거를 다시 사용 못하게 자기 자신 비활성화(옵션)
        gameObject.SetActive(false);
    }
}
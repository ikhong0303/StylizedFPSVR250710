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
    }
}
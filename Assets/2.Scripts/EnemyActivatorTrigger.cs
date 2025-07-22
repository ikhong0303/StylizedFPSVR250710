using UnityEngine;

public class EnemyActivatorTrigger : MonoBehaviour
{
    [Header("Ȱ��ȭ�� �� ������Ʈ��")]
    public GameObject[] enemiesToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        foreach (var enemy in enemiesToActivate)
        {
            if (enemy != null) enemy.SetActive(true);
        }
        // Ʈ���Ÿ� �ٽ� ��� ���ϰ� �ڱ� �ڽ� ��Ȱ��ȭ(�ɼ�)
        gameObject.SetActive(false);
    }
}
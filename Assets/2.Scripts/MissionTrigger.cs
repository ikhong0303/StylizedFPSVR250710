using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public MissionManager missionManager; // Inspector���� �巡��

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            missionManager.NextMission();

            // Ʈ���� ������Ʈ�� ������ ������ ���� (����ȭ)
            Destroy(gameObject);
        }
    }
}
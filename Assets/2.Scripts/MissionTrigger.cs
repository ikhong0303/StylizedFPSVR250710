using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public MissionManager missionManager; // Inspector���� �巡��

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ Ʈ���� �ν�
        {
            missionManager.NextMission();
            // or missionManager.SetMission(���ϴ� �ε���);
        }
    }
}

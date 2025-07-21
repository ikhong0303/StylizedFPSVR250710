using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public MissionManager missionManager; // Inspector에서 드래그

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어만 트리거 인식
        {
            missionManager.NextMission();
            // or missionManager.SetMission(원하는 인덱스);
        }
    }
}

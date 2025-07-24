using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public MissionManager missionManager; // Inspector에서 드래그

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            missionManager.NextMission();

            // 트리거 오브젝트를 씬에서 완전히 제거 (최적화)
            Destroy(gameObject);
        }
    }
}
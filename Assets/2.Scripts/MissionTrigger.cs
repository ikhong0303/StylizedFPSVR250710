using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    private MissionManager missionManager;

    void Awake()
    {
        // GameManager에서 직접 찾아올 수도 있고,
        // 씬 내에서 바로 찾을 수도 있음
        // 1. GameManager.Instance.player가 MissionManager를 들고 있다면:
        // missionManager = GameManager.Instance.player.GetComponent<MissionManager>();

        // 2. 아니면 그냥 씬 전체에서 찾아도 됨 (싱글톤이면 OK)
        missionManager = Object.FindFirstObjectByType<MissionManager>();

        if (missionManager == null)
            Debug.LogError("[MissionTrigger] MissionManager를 찾을 수 없음!");
    }

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

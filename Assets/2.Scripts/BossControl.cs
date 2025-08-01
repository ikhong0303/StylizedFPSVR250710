using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    public Transform player;           // 플레이어 Transform 참조
    private NavMeshAgent agent;        // NavMeshAgent 참조

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}

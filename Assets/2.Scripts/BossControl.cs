using UnityEngine;
using UnityEngine.AI;

public class BossControl : MonoBehaviour
{
    public Transform player;           // �÷��̾� Transform ����
    private NavMeshAgent agent;        // NavMeshAgent ����

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

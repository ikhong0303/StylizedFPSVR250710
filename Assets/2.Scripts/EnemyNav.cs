using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IDamageable
{
    void TakeDamage(float damage);
}

// EnemyNav: NavMeshAgent�� �̿��� ��ǥ�������� �̵��ϰ�, ���� �� ���� �ֱ�� �Ѿ��� �߻��ϴ� �� AI ��ũ��Ʈ
public class EnemyNav : MonoBehaviour, IDamageable
{
    NavMeshAgent navMeshAgent; // �׺�޽� ������Ʈ ������Ʈ
    public GameObject player; // �÷��̾� ������Ʈ
    float distance; // ��ǥ�������� �Ÿ�
    public float targetDistance = 2f; // ��ǥ���� ���� ���� �Ÿ�
    public Transform standingPosition; // ��ǥ����(���� �̵��� ��ġ)
    public Animator anim;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 5; // �ִ� ü��
    private float currentHealth;                 // ���� ü��
    private bool hasAimed = false;
    private bool isDead = false;   // ��� ���θ� �����ϴ� ����


    [Header("References")]
    public Animator Deatanimator; // ���� �ִϸ��̼ǿ� (���û���)
    public GameObject deathEffect; // �׾��� �� ����Ʈ ������ (���û���)

    // ���� �ܼ��� �Ѿ� ������Ʈ Ǯ
    private List<GameObject> bulletPool = new List<GameObject>();

    // ���� �� ��� �ð� ������ Ÿ�̸�
    private float arrivedTimer = 0f;
    public GameObject bulletPrefab; // �Ѿ� ������
    public Transform firePoint;     // �Ѿ��� �߻�� ��ġ(�ѱ� ��)
    public float bulletForce = 500f; // AddForce�� ����� ��

    // ������Ʈ �ʱ�ȭ
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // IDamageable �������̽� ����
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // �ǰ� �ִϸ��̼� (����)
        if (Deatanimator != null)
        {
            Deatanimator.SetTrigger("Hit");
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // ���� �ִϸ��̼� ����
        if (Deatanimator != null)
        {
            Deatanimator.SetTrigger("Die");
        }

        // ����Ʈ ����
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ���� �ð� �� ���� (�ִϸ��̼ǿ� ������)
        Destroy(gameObject, 5f);
    }


    // ���� �����Ӹ��� ȣ��
    private void FixedUpdate()
    {
        // ��ǥ������ ���� ��ġ�� �Ÿ� ���
        distance = Vector3.Distance(standingPosition.position, transform.position);
        anim.SetFloat("speed", navMeshAgent.velocity.magnitude);
        if (navMeshAgent.isStopped == true)
        {   
            // �������� �� �÷��̾ �ٶ�
            Vector3 lookPos = player.transform.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);

            if (!hasAimed)
            {
      
                hasAimed = true;

            }
        }

        // ��ǥ������ �������� �ʾ����� �̵�
        if (distance > targetDistance)
        {
            navMeshAgent.isStopped = false;
            Movement();
            arrivedTimer = 0f; // �̵� �߿��� Ÿ�̸� �ʱ�ȭ
            anim.SetBool("Aiming", false);

        }
        else // ��ǥ������ �������� ��
        {
            navMeshAgent.isStopped = true;
            anim.SetBool("Aiming",true);
    

            arrivedTimer += Time.fixedDeltaTime;
            if (arrivedTimer >= 2f) // 2�ʸ��� ����
            {
                Debug.Log("��ǥ������ �������� 2�� ���");
                arrivedTimer = 0f;

                // Ǯ���� ��Ȱ��ȭ�� �Ѿ� ã��
                GameObject bullet = null;
                foreach (var b in bulletPool)
                {
                    if (!b.activeInHierarchy)
                    {
                        bullet = b;
                        break;
                    }
                }
                // ��Ȱ��ȭ�� �Ѿ��� ������ ���� �����ؼ� Ǯ�� �߰�
                if (bullet == null)
                {
                    bullet = Instantiate(bulletPrefab);
                    bulletPool.Add(bullet);
                }

                // �Ѿ� ��ġ/ȸ�� ���� �� Ȱ��ȭ
                bullet.transform.position = firePoint.position;
                bullet.transform.rotation = firePoint.rotation;
                bullet.SetActive(true);


            }
        }
    }

    // NavMeshAgent�� �̿��� ��ǥ�������� �̵�
    void Movement()
    {
        navMeshAgent.SetDestination(standingPosition.position);
    }
}

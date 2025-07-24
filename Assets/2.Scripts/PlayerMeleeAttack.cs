using MikeNspired.XRIStarterKit;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("���� ���� ����")]
    public float attackDamage = 15f;
    public float attackCooldown = 0.5f; // ��Ÿ ����(��)
    public LayerMask targetLayer;       // ���� ��� Ÿ��
    public AudioClip hitSound;
    public GameObject hitEffectPrefab;

    [Header("��ġ �ӵ� ����")]
    public float minAttackSpeed = 2.0f; // �� �ӵ� �̻��� ���� ������
    private Vector3 lastPosition;

    private float lastAttackTime = -100f;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // ��ġ�� �׻� �����صд� (OnTriggerEnter��)
        lastPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ��Ÿ�� üũ
        if (Time.time - lastAttackTime < attackCooldown) return;

        // ���� ��� üũ (IDamageable + LayerMask)
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        // �ӵ� üũ (���� �����Ӱ��� �Ÿ� ��ȭ)
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        if (speed < minAttackSpeed) return;

        // ������ ����
        damageable.TakeDamage(attackDamage, gameObject);
        lastAttackTime = Time.time;

        // ȿ����
        if (hitSound && Camera.main)
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);

        // ����Ʈ(��ƼŬ ��)
        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
    }
}
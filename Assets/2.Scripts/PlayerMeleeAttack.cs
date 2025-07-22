using MikeNspired.XRIStarterKit;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // XRBaseController�� ����

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("���� ���� ����")]
    public float attackDamage = 15f;
    public float attackCooldown = 0.5f; // ��Ÿ ����(��)
    public LayerMask targetLayer;       // ���� ��� Ÿ��
    public AudioClip hitSound;
    public GameObject hitEffectPrefab;

    [Header("XR Haptic (����)")]
    public XRBaseController controller; // �� ��(��/������) XRBaseController ����
    public float hapticAmplitude = 0.5f;
    public float hapticDuration = 0.1f;

    private float lastAttackTime = -100f;

    private void OnTriggerEnter(Collider other)
    {
        // ��Ÿ�� üũ
        if (Time.time - lastAttackTime < attackCooldown) return;

        // ���� ��� üũ (IDamageable + LayerMask)
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        // ������ ����
        damageable.TakeDamage(attackDamage, gameObject);
        lastAttackTime = Time.time;

        // ȿ����
        if (hitSound && Camera.main) // AudioSource�� �տ� ������, ī�޶󿡼� ���
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 1f);

        // ����Ʈ(��ƼŬ ��)
        if (hitEffectPrefab)
            Instantiate(hitEffectPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);

        // ����
        if (controller != null)
            controller.SendHapticImpulse(hapticAmplitude, hapticDuration);
    }
}

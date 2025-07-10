using UnityEngine;
using UnityEngine.Serialization;

namespace MikeNspired.XRIStarterKit
{
    // �� ��ũ��Ʈ�� 'Ÿ��'�� ���� �´� ����(��: ������ �߾�/�����ڸ� ��)�� �����ϴ� �� ����
    public class TargetPoints : MonoBehaviour, IDamageable
    {
        // �¾��� �� �߻��ϴ� �̺�Ʈ (�θ� Target���� �� �̺�Ʈ�� ��� ������)
        public UnityEventFloat onHit;

        // ���� �̸��� damageMultiplier���� ����. �¾��� �� �� ����(�Ǵ� ������)
        [FormerlySerializedAs("damageMultiplier")]
        public float points = 1;

        // �¾��� �� ����� ���� ����Ʈ
        public AudioRandomize hitSoundEffect;

        // ���� �� Ÿ���� �������� ���� �� �ִ� ��������
        public bool canTakeDamage;

        // �������� �޴� �������̽� �Լ� (�Ѿ� ��� ȣ���)
        public void TakeDamage(float damage, GameObject damager)
        {
            if (!canTakeDamage) return;           // ������ ���� �� ���� ���¸� ����
            if (hitSoundEffect)
                hitSoundEffect.Play();            // ȿ���� ���
            onHit.Invoke(points);                 // ����(or ������) ����
        }

        // �������� �������� �ʰ� �ܼ��� Ÿ���� �¾Ҵٴ� ��ȣ�� �� �� ���
        public void Hit()
        {
            if (!canTakeDamage) return;
            if (hitSoundEffect)
                hitSoundEffect.Play();
            onHit.Invoke(points);
        }

        // �ܺο��� Ÿ���� Ȱ��ȭ�� �� ȣ��
        public void Activate() => canTakeDamage = true;

        // �ܺο��� Ÿ���� ��Ȱ��ȭ�� �� ȣ��
        public void Deactivate() => canTakeDamage = false;
    }
}
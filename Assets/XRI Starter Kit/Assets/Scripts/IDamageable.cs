using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// �� �������̽��� '�������� ���� �� �ִ� ���'���� �����˴ϴ�.
    /// ��: �÷��̾�, ��, �ı� ������ ������Ʈ ��.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// �������� �����մϴ�.
        /// </summary>
        /// <param name="damage">���� ������ ��</param>
        /// <param name="damager">������ ��ü (�Ѿ�, �� ��)</param>
        void TakeDamage(float damage, GameObject damager);
    }

    /// <summary>
    /// �� �������̽��� '�浹���� �� � ������ ����Ʈ�� �߻���ų��'�� �����մϴ�.
    /// ��: �Ѿ��� �ݼ�, ����, ���� � �ε����� �� ���� �ٸ� ����Ʈ ���.
    /// </summary>
    public interface IImpactType
    {
        /// <summary>
        /// �浹 �� � ������ ǥ������ ��ȯ�մϴ�.
        /// </summary>
        ImpactType GetImpactType();

        /// <summary>
        /// �浹 �� �� ������Ʈ�� '�ڽ����� �پ�� �ϴ���' ���θ� �����մϴ�.
        /// ��: ȭ���� ���� ������ ��� true
        /// </summary>
        bool ShouldReparent { get; }
    }

    /// <summary>
    /// ǥ���� ������ ������ �������Դϴ�.
    /// �� ���� ���� �ǰ� ����Ʈ, ���� ���� �ٸ��� ������ �� �ֽ��ϴ�.
    /// </summary>
    public enum ImpactType
    {
        Metal,   // �ݼ� ǥ��
        Flesh,   // ���� (��, ���� ��)
        Wood,    // ����
        Neutral  // �⺻�� �Ǵ� Ư��ó�� ���� ǥ��
    }
}
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // �� ��ũ��Ʈ�� ������Ʈ�� � '�浹 Ÿ��'�� ���� �ִ����� �����ϰ� ������
    public class ImpactSurfaceType : MonoBehaviour, IImpactType
    {
        // �� ������Ʈ�� � �浹 Ÿ������ (��: �ݼ�, ����, �� ��)
        [SerializeField] private ImpactType impactType;

        // �ܺο��� �� �浹 Ÿ���� ������ �� �ֵ��� �ϴ� �Լ� (�������̽� ����)
        public ImpactType GetImpactType() => impactType;

        // �浹 �� �� ������Ʈ�� �߻�ü(�Ǵ� ȿ�� ��)�� �θ� �Ǿ�� �ϴ��� ����
        [SerializeField] private bool shouldReparent;

        // �ܺο��� �� ���� ���� �� �ֵ��� �Ӽ����� ���� (�б� ����)
        public bool ShouldReparent => shouldReparent;
    }
}
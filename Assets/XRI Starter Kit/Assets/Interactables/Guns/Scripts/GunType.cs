using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // �� Ŭ������ �ѱ��� Ÿ��(����)�� �����ϱ� ���� ���Ǵ� ScriptableObject�Դϴ�.
    // ��: '����', '����', '����' ���� �ѱ� Ÿ�� ������ �����ϰ� �����ϴ� �� ���˴ϴ�.

    // Unity ������ �󿡼� ���ο� GunType ������ ������ �� �ֵ��� �޴��� �߰��մϴ�.
    // "Assets > Create > ScriptableObject > GunType" �޴��� ����ϴ�.
    [CreateAssetMenu(fileName = "GunType", menuName = "ScriptableObject/GunType")]
    public class GunType : ScriptableObject
    {
        // ���� �� Ŭ������ �ƹ� �����͵� ��� ���� ������,
        // ���߿� ���⿡ �ѱ� �̸�, �߻� �ӵ�, ������ �� �پ��� �Ӽ��� �߰��� �� �ֽ��ϴ�.

        // ���÷� �Ʒ��� ���� ������ ���߿� �߰��� �� �ֽ��ϴ�:
        // public string gunName;
        // public float fireRate;
        // public int damage;

        // ScriptableObject�� �Ϲ� MonoBehaviouró�� ���� �ٴ� ������Ʈ�� �ƴ϶�
        // ������ ������ �ڻ�(asset) ���Ϸ� �����ϸ�, ���� ������Ʈ���� �����Ͽ� ����� �� �ֽ��ϴ�.
    }
}
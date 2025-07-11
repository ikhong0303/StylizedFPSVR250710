using UnityEngine;

public class HolsterZone : MonoBehaviour
{
    public enum HolsterType { Left, Right }
    public HolsterType holsterType; // Inspector���� ���� (��/������ ����)

    private void OnTriggerEnter(Collider other)
    {
        // ���� ������ HolsterableGun ��ũ��Ʈ�� �پ��ִ��� üũ
        HolsterGun gun = other.GetComponent<HolsterGun>();
        if (gun != null)
        {
            gun.EnterHolster(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HolsterGun gun = other.GetComponent<HolsterGun>();
        if (gun != null)
        {
            gun.ExitHolster(this);
        }
    }
}
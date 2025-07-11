using UnityEngine;

public class HolsterZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // HolsterTrigger ������Ʈ(�ڽ�)�� �ݶ��̴��� ���� ��� �θ𿡼� HolsterGun ã��
        var gun = other.GetComponentInParent<HolsterGun>();
        if (gun != null)
        {
            gun.EnterHolster(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var gun = other.GetComponentInParent<HolsterGun>();
        if (gun != null)
        {
            gun.ExitHolster(this);
        }
    }
}
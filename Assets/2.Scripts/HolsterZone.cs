using UnityEngine;

public class HolsterZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // HolsterTrigger 오브젝트(자식)에 콜라이더가 있을 경우 부모에서 HolsterGun 찾기
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
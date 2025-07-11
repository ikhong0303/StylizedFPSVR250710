using UnityEngine;

public class HolsterZone : MonoBehaviour
{
    public enum HolsterType { Left, Right }
    public HolsterType holsterType; // Inspector에서 선택 (왼/오른쪽 구분)

    private void OnTriggerEnter(Collider other)
    {
        // 총이 들어오면 HolsterableGun 스크립트가 붙어있는지 체크
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
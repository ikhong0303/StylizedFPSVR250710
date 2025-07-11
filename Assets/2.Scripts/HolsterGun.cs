using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HolsterGun : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool isInHolster = false;
    private Transform holsterTransform;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    // 홀스터에 진입
    public void EnterHolster(HolsterZone holster)
    {
        holsterTransform = holster.transform;
        if (!grabInteractable.isSelected && !isInHolster)
        {
            Holster();
        }
    }

    // 홀스터에서 나감
    public void ExitHolster(HolsterZone holster)
    {
        if (isInHolster)
        {
            isInHolster = false;
            holsterTransform = null;
        }
    }

    // 실제 홀스터 고정 동작
    private void Holster()
    {
        isInHolster = true;
        transform.SetParent(holsterTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 수납 중 물리 비활성화
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 손에 쥐면 꺼내기
    public void Unholster()
    {
        isInHolster = false;
        transform.SetParent(null);

        // 다시 물리 활성화
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }
    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (isInHolster)
            Unholster();
    }
}
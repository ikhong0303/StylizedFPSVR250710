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

    // Ȧ���Ϳ� �������� �� ȣ���
    public void EnterHolster(HolsterZone holster)
    {
        holsterTransform = holster.transform;
        // �տ� ��� ���� �ʰ�, �̹� Ȧ���� ���� �ƴϸ� ����
        if (!grabInteractable.isSelected && !isInHolster)
        {
            Holster();
        }
    }

    // Ȧ���Ϳ��� ������ ��
    public void ExitHolster(HolsterZone holster)
    {
        if (isInHolster)
        {
            isInHolster = false;
            holsterTransform = null;
        }
    }

    // ���� Ȧ���Ϳ� �ִ� �Լ�
    private void Holster()
    {
        isInHolster = true;
        transform.SetParent(holsterTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 추가! Rigidbody를 Kinematic으로 변경
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;   // 수납 중에는 물리 시뮬레이션 중단
            rb.linearVelocity = Vector3.zero;    // 혹시 남아있던 속도 제거
            rb.angularVelocity = Vector3.zero;
        }
    }

    // ������ �ٽ� ���� ��
    public void Unholster()
    {
        isInHolster = false;
        transform.SetParent(null);

        // 다시 물리 적용
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;    // 손에 쥐면 다시 물리 동작

        // grabInteractable.enabled = true; 등 필요한 기능 추가
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
        // Ȧ���Ϳ��� �������� Unholster ó��
        if (isInHolster)
            Unholster();
    }
}
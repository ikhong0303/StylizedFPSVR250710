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
        // ���� �θ� Ȧ���� Transform���� ���� (��ġ/ȸ�� ����)
        transform.SetParent(holsterTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // XRGrabInteractable ��Ȱ��ȭ (���� �߿��� ������ ���� �� ���� �Ϸ���)
        //grabInteractable.enabled = false;

        // �ݶ��̴� � �ʿ�� �� �� ����
        //GetComponent<Collider>().enabled = false;
    }

    // ������ �ٽ� ���� ��
    public void Unholster()
    {
        isInHolster = false;
        transform.SetParent(null); // �� ��Ʈ�� Ǯ�� (Ȥ�� XR Origin���� �ٿ��� ��)
        //grabInteractable.enabled = true;
        //GetComponent<Collider>().enabled = true;
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
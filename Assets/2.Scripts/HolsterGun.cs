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

    // 홀스터에 진입했을 때 호출됨
    public void EnterHolster(HolsterZone holster)
    {
        holsterTransform = holster.transform;
        // 손에 들고 있지 않고, 이미 홀스터 중이 아니면 수납
        if (!grabInteractable.isSelected && !isInHolster)
        {
            Holster();
        }
    }

    // 홀스터에서 나갔을 때
    public void ExitHolster(HolsterZone holster)
    {
        if (isInHolster)
        {
            isInHolster = false;
            holsterTransform = null;
        }
    }

    // 실제 홀스터에 넣는 함수
    private void Holster()
    {
        isInHolster = true;
        // 총의 부모를 홀스터 Transform으로 변경 (위치/회전 고정)
        transform.SetParent(holsterTransform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // XRGrabInteractable 비활성화 (수납 중에는 손으로 잡을 수 없게 하려면)
        //grabInteractable.enabled = false;

        // 콜라이더 등도 필요시 끌 수 있음
        //GetComponent<Collider>().enabled = false;
    }

    // 손으로 다시 집을 때
    public void Unholster()
    {
        isInHolster = false;
        transform.SetParent(null); // 씬 루트로 풀기 (혹은 XR Origin으로 붙여도 됨)
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
        // 홀스터에서 꺼냈으면 Unholster 처리
        if (isInHolster)
            Unholster();
    }
}
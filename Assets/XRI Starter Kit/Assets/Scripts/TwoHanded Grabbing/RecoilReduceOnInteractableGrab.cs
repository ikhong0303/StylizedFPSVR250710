using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// VR���� ��(ProjectileWeapon)�� "������" �ݵ�(recoil)�� �پ���,
    /// "�տ��� ������" �ٽ� ���� �ݵ����� ���ƿ��� �ϴ� ��ũ��Ʈ!
    /// </summary>
    public class RecoilReduceOnInteractableGrab : MonoBehaviour
    {
        [SerializeField] private ProjectileWeapon projectileWeapon = null; // �ݵ� ������ �� ��ũ��Ʈ
        [SerializeField] private XRGrabInteractable interactable = null;   // ���� �� �ִ� XR ���ͷ��ͺ�
        [SerializeField] private float recoilReduction = .6f;              // ���� �� �ݵ� ������(60% ����)
        [SerializeField] private float recoilRotationReduction = .8f;      // ȸ�� �ݵ� ������(80% ����)
        private float startingRecoil, startingRotationRecoil;              // ���� �ݵ��� ����

        // ���� ����(�ʱ�ȭ) �� �ڵ� ����
        private void Start()
        {
            OnValidate(); // interactable �ڵ� ����
            // ���� ���� �ݵ�/ȸ���ݵ��� ����
            startingRecoil = projectileWeapon.recoilAmount;
            startingRotationRecoil = projectileWeapon.recoilRotation;

            // XRGrabInteractable�� ����Ǿ� ������
            if (!interactable) return;
            // "������ ��" �̺�Ʈ ��� �� �ݵ� ���̱�
            interactable.selectEntered.AddListener(x => ReduceProjectileWeaponRecoil());
            // "������ ��" �̺�Ʈ ��� �� �ݵ� ���󺹱�
            interactable.selectExited.AddListener(x => ReturnProjectileWeaponRecoil());
        }

        // Inspector ���� �ٲ� ������ ���� (interactable�� ������ �ڵ� ����)
        private void OnValidate()
        {
            if (!interactable)
                interactable = GetComponent<XRGrabInteractable>();
        }

        // ����� �� ���� �� �� �ݵ� ���̱�!
        public void ReduceProjectileWeaponRecoil()
        {
            projectileWeapon.recoilAmount *= 1 - recoilReduction;         // ��: ������ 40%��
            projectileWeapon.recoilRotation *= 1 - recoilRotationReduction; // ��: ������ 20%��
        }

        // ������ �� ���� �� �ݵ� ������� ����!
        public void ReturnProjectileWeaponRecoil()
        {
            projectileWeapon.recoilAmount = startingRecoil;
            projectileWeapon.recoilRotation = startingRotationRecoil;
        }
    }
}

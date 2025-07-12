using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// VR에서 총(ProjectileWeapon)을 "집으면" 반동(recoil)이 줄어들고,
    /// "손에서 놓으면" 다시 원래 반동으로 돌아오게 하는 스크립트!
    /// </summary>
    public class RecoilReduceOnInteractableGrab : MonoBehaviour
    {
        [SerializeField] private ProjectileWeapon projectileWeapon = null; // 반동 조절할 총 스크립트
        [SerializeField] private XRGrabInteractable interactable = null;   // 잡을 수 있는 XR 인터렉터블
        [SerializeField] private float recoilReduction = .6f;              // 집을 때 반동 감소율(60% 줄임)
        [SerializeField] private float recoilRotationReduction = .8f;      // 회전 반동 감소율(80% 줄임)
        private float startingRecoil, startingRotationRecoil;              // 원래 반동값 저장

        // 게임 시작(초기화) 시 자동 실행
        private void Start()
        {
            OnValidate(); // interactable 자동 연결
            // 총의 원래 반동/회전반동값 저장
            startingRecoil = projectileWeapon.recoilAmount;
            startingRotationRecoil = projectileWeapon.recoilRotation;

            // XRGrabInteractable이 연결되어 있으면
            if (!interactable) return;
            // "집었을 때" 이벤트 등록 → 반동 줄이기
            interactable.selectEntered.AddListener(x => ReduceProjectileWeaponRecoil());
            // "놓았을 때" 이벤트 등록 → 반동 원상복구
            interactable.selectExited.AddListener(x => ReturnProjectileWeaponRecoil());
        }

        // Inspector 값이 바뀔 때마다 실행 (interactable이 없으면 자동 연결)
        private void OnValidate()
        {
            if (!interactable)
                interactable = GetComponent<XRGrabInteractable>();
        }

        // 잡았을 때 실행 → 총 반동 줄이기!
        public void ReduceProjectileWeaponRecoil()
        {
            projectileWeapon.recoilAmount *= 1 - recoilReduction;         // 예: 원래의 40%로
            projectileWeapon.recoilRotation *= 1 - recoilRotationReduction; // 예: 원래의 20%로
        }

        // 놓았을 때 실행 → 반동 원래대로 복구!
        public void ReturnProjectileWeaponRecoil()
        {
            projectileWeapon.recoilAmount = startingRecoil;
            projectileWeapon.recoilRotation = startingRotationRecoil;
        }
    }
}

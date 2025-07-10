// Author MikeNspired.
// Modified to fix event listener issues

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 이 스크립트는 탄창을 총기에 장착하거나 분리하는 기능을 담당합니다.
    /// XRGrabInteractable을 이용해 총기를 잡고 있을 때만 탄창 장착이 가능하고,
    /// 장착 시 정해진 애니메이션 경로로 위치가 조정되며, 사운드도 재생됩니다.
    /// </summary>
    public class MagazineAttachPoint : MonoBehaviour
    {
        [SerializeField] private Transform start, end; // 탄창이 들어올 때 애니메이션 경로의 시작/끝 지점
        [SerializeField] private float alignAnimationLength = 0.05f;   // 정렬 애니메이션 길이
        [SerializeField] private float insertAnimationLength = 0.1f;   // 삽입 애니메이션 길이
        [SerializeField] private AudioSource loadAudio, unloadAudio;   // 장착/제거 사운드
        [SerializeField] private GunType gunType = null;               // 이 총기에서 허용하는 탄창 타입
        [SerializeField] private Magazine startingMagazine = null;     // 게임 시작 시 장착된 탄창
        [SerializeField] private new Collider collider = null;         // 총에 붙은 트리거용 콜라이더
        [SerializeField] private bool removeByGrabbing = true;         // 손으로 탄창 제거 가능한지 여부

        private XRGrabInteractable xrGrabInteractable; // 총기를 잡기 위한 인터랙터
        private XRInteractionManager interactionManager; // XR 인터랙션 매니저
        private Magazine magazine;          // 현재 장착 중인 탄창
        private bool ammoIsAttached;        // 현재 탄창이 장착되어 있는지 여부
        private bool isBeingGrabbed;        // 총기가 잡혀 있는지 여부

        public Magazine Magazine => magazine;
        public GunType GunType => gunType;

        private void Awake()
        {
            OnValidate();

            // 총기 잡거나 놓을 때 콜백 연결
            xrGrabInteractable.selectEntered.AddListener(_ => SetMagazineGrabbableState());
            xrGrabInteractable.selectExited.AddListener(_ => SetMagazineGrabbableState());

            collider.gameObject.SetActive(false); // 시작 시 콜라이더 비활성화
            if (startingMagazine) CreateStartingMagazine(); // 시작 탄창 생성
        }

        // XR 상호작용 이벤트 후에 상태 조정 (한 프레임 후에 실행)
        private void SetMagazineGrabbableState()
        {
            CancelInvoke();
            Invoke(nameof(MakeMagazineGrabbable), Time.deltaTime);
            Invoke(nameof(EnableOrDisableAttachCollider), Time.deltaTime);
        }

        // XR로 총을 잡고 있을 때만 콜라이더 활성화
        private void EnableOrDisableAttachCollider()
        {
            collider.gameObject.SetActive(xrGrabInteractable.isSelected);
        }

        // 탄창을 손으로 뺄 수 있는지 여부를 판단해 콜라이더 활성/비활성화
        private void MakeMagazineGrabbable()
        {
            if (!magazine) return;

            isBeingGrabbed = xrGrabInteractable.isSelected;

            if (removeByGrabbing && isBeingGrabbed)
                magazine.EnableCollider(); // 손으로 뺄 수 있게
            else
                magazine.DisableCollider(); // 붙어있게
        }

        // 컴포넌트 자동 할당
        private void OnValidate()
        {
            if (!xrGrabInteractable)
                xrGrabInteractable = GetComponentInParent<XRGrabInteractable>();
            if (!interactionManager)
                interactionManager = FindFirstObjectByType<XRInteractionManager>();
        }

        // 시작 탄창 생성
        private void CreateStartingMagazine()
        {
            if (magazine) return;
            SetupNewMagazine(Instantiate(startingMagazine, end.position, end.rotation, transform));
            magazine.DisableCollider();
        }

        // 트리거 충돌 시 탄창이 맞는 종류이면 장착 처리
        private void OnTriggerEnter(Collider other)
        {
            if (ammoIsAttached) return;

            var collidedMagazine = other.attachedRigidbody?.GetComponent<Magazine>();
            if (collidedMagazine && collidedMagazine.GunType == gunType && CheckIfBothGrabbed(collidedMagazine))
            {
                if (collidedMagazine.CurrentAmmo <= 0) return;
                ReleaseItemFromHand(collidedMagazine); // 손에서 떼고
                SetupNewMagazine(collidedMagazine);    // 장착하고
                StartCoroutine(StartAnimation(other.attachedRigidbody.transform)); // 애니메이션 실행
            }
        }

        // 새 탄창을 현재 총에 장착함
        private void SetupNewMagazine(Magazine mag)
        {
            magazine = mag;
            var interactable = magazine.GetComponent<XRGrabInteractable>();
            interactable.selectEntered.AddListener(OnMagazineGrabbed); // 제거 감지용 리스너
            magazine.SetupForGunAttachment(); // 물리 세팅 (고정 등)
            magazine.transform.parent = transform;
            ammoIsAttached = true;
        }

        // 탄창이 잡혔을 때 (즉 제거될 때) 실행
        private void OnMagazineGrabbed(SelectEnterEventArgs args)
        {
            AmmoRemoved();
        }

        // 총과 탄창이 모두 손에 잡혀있는지 체크
        private bool CheckIfBothGrabbed(Magazine magazine) => isBeingGrabbed && magazine.IsBeingGrabbed();

        // 탄창을 강제로 손에서 떼어냄
        private void ReleaseItemFromHand(Magazine collidedMagazine)
        {
            var interactor = collidedMagazine.GetComponent<XRGrabInteractable>().firstInteractorSelecting;
            interactionManager.SelectExit(interactor, collidedMagazine.GetComponent<XRGrabInteractable>());
        }

        // 탄창 제거 처리
        private void AmmoRemoved()
        {
            StopAllCoroutines();
            CancelInvoke();

            if (magazine != null)
            {
                var interactable = magazine.GetComponent<XRGrabInteractable>();
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed); // 리스너 제거
            }

            magazine = null;
            unloadAudio.Play();
            Invoke(nameof(PreventAutoAttach), 0.15f); // 중복 장착 방지용 딜레이
        }

        // 장착 중복 방지용 플래그 해제
        private void PreventAutoAttach()
        {
            ammoIsAttached = false;
        }

        // 탄창을 애니메이션으로 부드럽게 장착
        private IEnumerator StartAnimation(Transform ammo)
        {
            yield return AnimateTransform(ammo, start.localPosition, start.localRotation, alignAnimationLength);
            loadAudio.Play();
            yield return AnimateTransform(ammo, end.localPosition, end.localRotation, insertAnimationLength);
        }

        // 탄창을 분리함
        public void EjectMagazine()
        {
            if (magazine == null) return;
            StopAllCoroutines();
            StartCoroutine(EjectMagazineAnimation(magazine.transform));
        }

        // 탄창 제거 애니메이션 (시작 위치까지 이동 후 detach)
        private IEnumerator EjectMagazineAnimation(Transform ammo)
        {
            unloadAudio.Play();
            yield return AnimateTransform(ammo, start.localPosition, start.localRotation, insertAnimationLength);

            if (magazine != null)
            {
                var interactable = magazine.GetComponent<XRGrabInteractable>();
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed);
                magazine.ResetToGrabbableObject();
                magazine = null;
            }

            ammoIsAttached = false;
            collider.gameObject.SetActive(true);
        }

        // 주어진 시간 동안 오브젝트 위치 및 회전 보간 이동
        private IEnumerator AnimateTransform(Transform target, Vector3 targetPosition, Quaternion targetRotation, float duration)
        {
            float timer = 0;
            var startPosition = target.localPosition;
            var startRotation = target.localRotation;

            while (timer < duration)
            {
                float progress = timer / duration;
                target.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                target.localRotation = Quaternion.Lerp(startRotation, targetRotation, progress);
                timer += Time.deltaTime;
                yield return null;
            }

            target.localPosition = targetPosition;
            target.localRotation = targetRotation;
        }

        // 씬에서 제거될 때 리스너 정리
        private void OnDestroy()
        {
            if (magazine != null && magazine.TryGetComponent(out XRGrabInteractable interactable))
            {
                interactable.selectEntered.RemoveListener(OnMagazineGrabbed);
            }
        }
    }
}
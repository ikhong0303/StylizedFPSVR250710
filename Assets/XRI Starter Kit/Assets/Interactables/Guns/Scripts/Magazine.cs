using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// 이 클래스는 탄창 오브젝트를 제어하는 스크립트입니다.
    /// XR Grab 시스템과 연동되어 직접 손으로 잡거나 총에 장착 가능하며,
    /// 탄 수를 관리하고 콜라이더 상태를 조정하는 역할을 합니다.
    /// </summary>
    public class Magazine : MonoBehaviour, IReturnMovedColliders
    {
        public int MaxAmmo = 10;         // 최대 탄약 수
        public int CurrentAmmo = 10;     // 현재 남은 탄약 수
        private bool isBeingGrabbed = false; // 현재 잡혀 있는지 여부

        [SerializeField] private GunType gunType = null;           // 이 탄창이 사용되는 총기 유형 (ScriptableObject)
        [SerializeField] private GameObject ammoModels = null;     // 탄약이 남아있을 때 보여줄 비주얼 모델
        [FormerlySerializedAs("collider")]
        [SerializeField] private Collider magazineCollider = null; // 충돌 판정용 콜라이더
        [SerializeField] private Rigidbody rigidBody = null;       // 물리 처리용 리지드바디

        private XRGrabInteractable grabInteractable;   // XR 상호작용용 인터랙터
        private Vector3 startingColliderPosition;      // 초기 콜라이더 위치
        private Rigidbody rb;

        // 외부에서 현재 잡혀 있는지 확인할 수 있는 속성
        public bool IsBeingGrabbed() => isBeingGrabbed;

        // 외부에서 GunType에 접근할 수 있는 속성
        public GunType GunType => gunType;

        private void Awake()
        {
            // 컴포넌트 자동 할당
            grabInteractable = GetComponent<XRGrabInteractable>();
            magazineCollider = magazineCollider ? magazineCollider : GetComponentInChildren<Collider>();
            rigidBody = rigidBody ? rigidBody : GetComponentInChildren<Rigidbody>();

            // 초기 콜라이더 위치 저장
            startingColliderPosition = magazineCollider.transform.localPosition;

            // XR 이벤트 등록
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            // XR 상호작용 이벤트 연결
            grabInteractable.selectEntered.AddListener(_ => OnGrab());
            grabInteractable.selectExited.AddListener(_ => OnRelease());
        }

        // 탄창을 잡았을 때 실행됨
        private void OnGrab()
        {
            isBeingGrabbed = true;
            rigidBody.isKinematic = true; // 물리 반응 제거 (고정)
        }

        // 탄창을 놓았을 때 실행됨
        private void OnRelease()
        {
            isBeingGrabbed = false;
            ResetToGrabbableObject(); // 상태 초기화
        }

        private void OnEnable()
        {
            // 비활성화 후 다시 활성화될 때 콜라이더 위치 초기화
            magazineCollider.transform.localPosition = startingColliderPosition;
        }

        /// <summary>
        /// 콜라이더를 부드럽게 원래 위치로 되돌리고 비활성화함 (장착 시 사용)
        /// </summary>
        public void DisableCollider()
        {
            if (!gameObject.activeInHierarchy) return;
            StartCoroutine(PhysicsHelper.MoveAndDisableCollider(magazineCollider, startingColliderPosition));
        }

        /// <summary>
        /// 콜라이더를 활성화하고 위치 복원
        /// </summary>
        public void EnableCollider()
        {
            ReturnMovedColliders();
            magazineCollider.enabled = true;
            EnableDistanceGrabbing(true); // 원거리 잡기 가능하게 설정
        }

        /// <summary>
        /// 탄창을 다시 잡을 수 있도록 상태 초기화
        /// </summary>
        public void ResetToGrabbableObject()
        {
            EnableCollider();
            rigidBody.isKinematic = false;
            transform.parent = null; // 부모에서 분리 (총에서 분리됨)
        }

        /// <summary>
        /// 탄창을 총기에 장착할 때 세팅 (중력 O, 리지드바디 고정, 트리거화 등)
        /// </summary>
        public void SetupForGunAttachment()
        {
            rigidBody.isKinematic = true;
            rigidBody.useGravity = true;
            EnableDistanceGrabbing(false); // 원거리 잡기 비활성화
        }

        /// <summary>
        /// 원거리 XR 잡기 기능 on/off
        /// </summary>
        private void EnableDistanceGrabbing(bool state)
        {
            if (TryGetComponent(out InteractableItemData itemData))
                itemData.canDistanceGrab = state;
        }

        /// <summary>
        /// 탄약을 한 발 사용. 탄약이 있으면 true 반환, 없으면 false
        /// </summary>
        public bool UseAmmo()
        {
            if (CurrentAmmo <= 0)
                return false;

            CurrentAmmo--;

            // 탄약이 0이면 탄약 모델 숨김
            if (CurrentAmmo <= 0 && ammoModels != null)
                ammoModels.SetActive(false);

            return true;
        }

        /// <summary>
        /// 콜라이더를 원래 위치로 되돌리고 코루틴 중단
        /// </summary>
        public void ReturnMovedColliders()
        {
            StopAllCoroutines();
            magazineCollider.transform.localPosition = startingColliderPosition;
        }
    }
}
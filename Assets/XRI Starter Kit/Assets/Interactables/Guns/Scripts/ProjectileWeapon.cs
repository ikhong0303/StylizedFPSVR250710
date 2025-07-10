// Author MikeNspired

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Unity.Mathematics.math; // 수학 유틸리티 함수(remap 등) 사용

namespace MikeNspired.XRIStarterKit
{
    // ProjectileWeapon: VR에서 사용하는 총기 발사 및 반동, 탄창, 이펙트 제어
    public class ProjectileWeapon : MonoBehaviour
    {
        [Header("필수 참조")]
        [SerializeField] private Transform firePoint;            // 총알이 발사되는 위치
        [SerializeField] private Rigidbody projectilePrefab;     // 발사될 총알 프리팹
        [SerializeField] private ParticleSystem cartridgeEjection; // 탄피 이펙트
        [SerializeField] private AudioSource fireAudio;          // 발사 사운드
        [SerializeField] private AudioSource outOfAmmoAudio;     // 탄약 없음 사운드
        [SerializeField] private MatchTransform bulletFlash;     // 총구 섬광
        [SerializeField] private GunCocking gunCocking;          // 장전 상태 제어 스크립트

        [Header("설정")]
        public MagazineAttachPoint magazineAttach = null; // 탄창 시스템
        public float recoilAmount = -0.03f;                // 반동 이동 거리
        public float recoilRotation = 1;                   // 반동 회전
        public float recoilTime = 0.06f;                   // 반동 지속 시간
        public int bulletsPerShot = 1;                     // 한 번에 발사할 총알 수
        public float bulletSpreadAngle = 1;                // 총알 퍼짐 각도
        public float bulletSpeed = 150;                    // 총알 속도
        public bool infiniteAmmo = false;                  // 무한 탄약 여부
        public float hapticDuration = 0.1f;                // 햅틱 지속 시간
        public float hapticStrength = 0.5f;                // 햅틱 강도

        [Header("자동 발사")]
        public float fireSpeed = 0.25f;    // 자동 발사 속도 (초당 발사 간격)
        public bool automaticFiring = false; // 자동 발사 여부

        private XRGrabInteractable interactable;  // XR 인터랙션용 컴포넌트
        private XRBaseInteractor controller;      // 잡고 있는 컨트롤러
        private Collider[] gunColliders;          // 총기의 콜라이더 목록
        private bool gunCocked, isFiring;         // 장전 여부, 트리거 눌림 여부
        private float fireTimer;                  // 발사 간격 타이머

        // 이벤트
        public UnityEvent BulletFiredEvent, OutOfAmmoEvent, FiredLastBulletEvent;

        void Awake()
        {
            OnValidate(); // 컴포넌트 자동 연결

            // 트리거 작동시 발사
            interactable.activated.AddListener(_ => TryFire(true));
            interactable.deactivated.AddListener(_ => TryFire(false));

            // 잡았을 때 반동 설정, 놓으면 반동 정리
            interactable.selectEntered.AddListener(SetupRecoilVariables);
            interactable.selectExited.AddListener(DestroyRecoilTracker);

            if (gunCocking)
                gunCocking.GunCockedEvent.AddListener(() => gunCocked = true);
        }

        void OnValidate()
        {
            // 필요한 컴포넌트가 빠져 있으면 자동으로 찾기
            if (!gunCocking) gunCocking = GetComponentInChildren<GunCocking>();
            if (!interactable) interactable = GetComponent<XRGrabInteractable>();
        }

        // RecoilUpdate를 렌더 전에 등록
        void OnEnable() => Application.onBeforeRender += RecoilUpdate;
        void OnDisable() => Application.onBeforeRender -= RecoilUpdate;

        void Update()
        {
            if (!automaticFiring) return;

            // 자동 발사 모드: 트리거를 계속 누르고 있을 때 일정 간격으로 발사
            if (isFiring && fireTimer >= fireSpeed)
            {
                FireGun();
                fireTimer = 0f;
            }
            fireTimer += Time.deltaTime;
        }

        private void TryFire(bool state)
        {
            isFiring = state;

            // 자동 발사가 아닐 경우, 누를 때마다 1회 발사
            if (state && !automaticFiring) FireGun();
        }

        // 실제 발사 처리
        public void FireGun()
        {
            if (bulletsPerShot < 1) return;

            // 탄약 없거나 장전 안 되어 있으면 발사 안 됨
            if (magazineAttach && !infiniteAmmo && (CheckIfGunCocked() || !magazineAttach.Magazine || !magazineAttach.Magazine.UseAmmo()))
            {
                OutOfAmmoEvent.Invoke();
                outOfAmmoAudio.PlayOneShot(outOfAmmoAudio.clip);
                gunCocked = false;
                return;
            }

            // GunCocking이 있을 경우 장전 상태 확인
            if (gunCocking && !gunCocked)
            {
                OutOfAmmoEvent.Invoke();
                outOfAmmoAudio.PlayOneShot(outOfAmmoAudio.clip);
                return;
            }

            // 여러 발 발사 (샷건, 산탄총 등)
            for (int i = 0; i < bulletsPerShot; i++)
            {
                // 퍼짐 적용한 발사 방향 계산
                Vector3 shotDirection = Vector3.Slerp(
                    firePoint.forward,
                    UnityEngine.Random.insideUnitSphere,
                    bulletSpreadAngle / 180f
                );

                var bullet = Instantiate(projectilePrefab);
                IgnoreColliders(bullet); // 총알과 총기 충돌 무시

                bullet.transform.SetPositionAndRotation(
                    firePoint.position, Quaternion.LookRotation(shotDirection)
                );
                bullet.AddForce(bullet.transform.forward * bulletSpeed, ForceMode.VelocityChange);

                // 햅틱 진동
                controller.GetComponentInParent<HapticImpulsePlayer>().SendHapticImpulse(hapticStrength, hapticDuration);

                BulletFiredEvent.Invoke(); // 발사 이벤트

                StopAllCoroutines();
                StartRecoil(); // 반동 실행
            }

            // 마지막 탄약 발사했을 경우
            if (magazineAttach && magazineAttach.Magazine && magazineAttach.Magazine.CurrentAmmo == 0)
                FiredLastBulletEvent.Invoke();

            // 총구 섬광
            if (bulletFlash)
            {
                var flash = Instantiate(bulletFlash);
                flash.transform.position = firePoint.position;
                flash.positionToMatch = firePoint;
            }

            // 사운드 및 이펙트
            fireAudio?.PlayOneShot(fireAudio.clip);
            if (cartridgeEjection)
                cartridgeEjection.Play();
        }

        // 총알이 총과 충돌하지 않도록 설정
        private void IgnoreColliders(Component bullet)
        {
            gunColliders = GetComponentsInChildren<Collider>(true);
            var bulletCollider = bullet.GetComponentInChildren<Collider>();
            foreach (var c in gunColliders)
                Physics.IgnoreCollision(c, bulletCollider);
        }

        private bool CheckIfGunCocked() => gunCocking && !gunCocked;

        #region 반동 시스템 (Recoil)

        private Transform recoilTracker;
        private Quaternion startingRotation;
        private Vector3 endOfRecoilPosition;
        private Quaternion endOfRecoilRotation;
        private float timer;
        private bool isRecoiling;
        private Vector3 controllerToAttachDelta;

        // 총을 잡았을 때 반동 트래커 설정
        private void SetupRecoilVariables(SelectEnterEventArgs args)
        {
            controller = args.interactorObject as XRBaseInteractor;
            StartCoroutine(SetupRecoil(interactable.attachEaseInTime));
        }

        // 놓았을 때 반동 리셋
        private void DestroyRecoilTracker(SelectExitEventArgs args)
        {
            StopAllCoroutines();
            if (recoilTracker) Destroy(recoilTracker.gameObject);
            isRecoiling = false;
        }

        // 반동용 트래커 생성
        private IEnumerator SetupRecoil(float interactableAttachEaseInTime)
        {
            var handReference = controller.GetComponentInParent<HandReference>();
            if (!handReference) yield break;

            recoilTracker = new GameObject($"{name} Recoil Tracker").transform;
            recoilTracker.parent = controller.attachTransform;
            yield return null;
        }

        private void StartRecoil()
        {
            if (!recoilTracker) StartCoroutine(SetupRecoil(1));

            recoilTracker.localRotation = startingRotation;
            recoilTracker.localPosition = Vector3.zero;
            startingRotation = transform.localRotation;

            timer = 0f;
            controllerToAttachDelta = transform.position - recoilTracker.position;
            isRecoiling = true;
        }

        // 프레임별 반동 계산 (BeforeRender에 등록됨)
        [BeforeRenderOrder(101)]
        private void RecoilUpdate()
        {
            if (!isRecoiling) return;

            if (timer < recoilTime / 2f)
            {
                // 총을 뒤로 밀고, 위로 살짝 회전
                if (Math.Abs(recoilAmount) > 0.001f)
                {
                    recoilTracker.position += transform.forward * recoilAmount * Time.deltaTime;
                    transform.position = recoilTracker.position + controllerToAttachDelta;
                }

                if (Math.Abs(recoilRotation) > 0.001f)
                    transform.Rotate(Vector3.right, -recoilRotation * Time.deltaTime, Space.Self);

                endOfRecoilPosition = recoilTracker.localPosition;
                endOfRecoilRotation = transform.localRotation;
            }
            else
            {
                // 원래 위치로 되돌아가기
                float t = remap(recoilTime / 2f, recoilTime, 0f, 1f, timer);
                recoilTracker.localPosition = Vector3.Lerp(endOfRecoilPosition, Vector3.zero, t);
                var newRotation = Quaternion.Lerp(endOfRecoilRotation, startingRotation, t);

                transform.position = recoilTracker.position + controllerToAttachDelta;
                transform.localRotation = newRotation;
            }

            timer += Time.deltaTime;
            if (timer > recoilTime)
                isRecoiling = false;
        }

        #endregion
    }
}
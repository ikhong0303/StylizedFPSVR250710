// Author MikeNspired

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using static Unity.Mathematics.math; // 수학 유틸리티 함수(remap 등) 사용
using TMPro;

namespace MikeNspired.XRIStarterKit
{
    // ProjectileWeapon: VR에서 사용하는 총기 발사 및 반동, 탄창, 이펙트 제어
    public class ProjectileWeapon : MonoBehaviour
    {
        [Header("필수")]
        [SerializeField] private Transform firePoint;              // 총알이 나가는 위치
        [SerializeField] private Rigidbody projectilePrefab;       // 발사될 총알 프리팹
        [SerializeField] private AudioSource fireAudio;            // 발사 사운드
        [SerializeField] private ParticleSystem cartridgeEjection; // 탄피 이펙트
        [SerializeField] private MatchTransform bulletFlash;       // 총구 섬광
        [SerializeField] private Transform cylinderTransform;      // 실린더(회전용 부모)
        [SerializeField] private List<GameObject> bullets;         // 실린더 안에 보이는 총알 오브젝트(6개 등)


        [Header("설정")]
        [SerializeField] private float bulletSpeed = 150f;         // 총알 속도
        [SerializeField] private float cylinderAngle = 60f;        // 실린더 회전 각도
        [SerializeField] private float cylinderRotateDuration = 0.2f;   // ★ 부드러운 회전 지속시간
        [SerializeField] private TextMeshProUGUI ammoTextUI;       // UI용 TMP

        public Transform aimPointer;     // 조준점 오브젝트(스피어, 플랫 스프라이트 등)
        public float aimMaxDistance = 100f; // 최대 표시 거리

        public float recoilAmount = -0.03f;                        // 반동 이동 거리
        public float recoilRotation = 1;                           // 반동 회전
        public float recoilTime = 0.06f;                           // 반동 지속 시간
        public float hapticDuration = 0.1f;                        // 햅틱 지속 시간
        public float hapticStrength = 0.5f;                        // 햅틱 강도

        [Header("재장전 (Tilt Reload)")]
        [SerializeField] private float reloadAngle = 60f;      // 아래로 얼마나 기울이면 재장전되는지 (degree)
        [SerializeField] private float reloadCooldown = 1f;    // 재장전 연타 방지 쿨타임(초)

        // XR
        private XRGrabInteractable interactable;
        private XRBaseInteractor controller;
        private Collider[] gunColliders;

        // 상태
        private int currentBulletIndex = 0;
        private bool isRecoiling = false;
        private bool isCylinderRotating = false;   // 회전 중엔 발사 금지
        private Transform recoilTracker;
        private Quaternion startingRotation;
        private Vector3 endOfRecoilPosition;
        private Quaternion endOfRecoilRotation;
        private float timer;
        private Vector3 controllerToAttachDelta;
        private float lastReloadTime = -99f;                   // 마지막 재장전 시점

        // 이벤트
        public UnityEvent BulletFiredEvent, OutOfAmmoEvent, FiredLastBulletEvent;

        private void Awake()
        {
            interactable = GetComponent<XRGrabInteractable>();
            interactable.activated.AddListener(_ => FireGun());
            interactable.selectEntered.AddListener(SetupRecoilVariables);
            interactable.selectExited.AddListener(DestroyRecoilTracker);
            UpdateAmmoTextUI();

        }

        private void FixedUpdate()
        {
            if (interactable.isSelected && controller != null)
            {
                var gunForward = firePoint.forward.normalized;
                float dot = Vector3.Dot(gunForward, Vector3.down); // 아래=+1, 위=-1

                // 140도 기준이면 0.766f, 아래든 위든 상관없이 장전
                if (Mathf.Abs(dot) > 0.766f && Time.time - lastReloadTime > reloadCooldown)
                {
                    Reload();
                    lastReloadTime = Time.time;
                }
            }

            RaycastHit hit;
            Vector3 targetPos;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, aimMaxDistance))
                targetPos = hit.point;
            else
                targetPos = firePoint.position + firePoint.forward * aimMaxDistance;

            aimPointer.position = targetPos;

        }

        public void FireGun()
        {
            if (isCylinderRotating) return; // 회전 중에는 중복 발사 금지
            if (currentBulletIndex >= bullets.Count)
            {
                OutOfAmmoEvent?.Invoke();
                Debug.Log("총알 없음! 재장전 필요");
                return;
            }

            // 총알 비활성화
            bullets[currentBulletIndex].SetActive(false);

            // 발사체 생성 및 힘 적용
            var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            bullet.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);

            // 사운드, 이펙트
            fireAudio?.PlayOneShot(fireAudio.clip);
            cartridgeEjection?.Play();

            // 총구 플래쉬
            if (bulletFlash)
            {
                var flash = Instantiate(bulletFlash);
                flash.transform.position = firePoint.position;
                flash.positionToMatch = firePoint;
            }

            // 햅틱(진동)
            if (controller)
            {
                var haptic = controller.GetComponentInParent<HapticImpulsePlayer>();
                haptic?.SendHapticImpulse(hapticStrength, hapticDuration);
            }

            BulletFiredEvent?.Invoke();

            //// 실린더 회전
            //if (cylinderTransform)
            //    cylinderTransform.localRotation *= Quaternion.Euler(0, 0, -cylinderAngle);

            // ▶ 실린더 회전을 부드럽게!
            if (cylinderTransform)
            {
                float targetAngle = -(currentBulletIndex + 1) * cylinderAngle;
                StartCoroutine(RotateCylinderSmooth(targetAngle, cylinderRotateDuration));
            }

            // 인덱스 증가
            currentBulletIndex++;

            // 마지막 총알이면 이벤트
            if (currentBulletIndex == bullets.Count)
                FiredLastBulletEvent?.Invoke();

            // 반동
            //StopAllCoroutines();
            StartRecoil();
            UpdateAmmoTextUI();
            
        }

        // ★ 실린더(총알 그룹) 부드러운 회전 코루틴
        private IEnumerator RotateCylinderSmooth(float targetAngle, float duration)
        {
            isCylinderRotating = true;

            float startAngle = cylinderTransform.localEulerAngles.z;
            // 각도 차이 계산(0~360 기준으로 보정)
            float endAngle = targetAngle;
            if (endAngle - startAngle > 180f) startAngle += 360f;
            if (startAngle - endAngle > 180f) endAngle += 360f;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float angle = Mathf.Lerp(startAngle, endAngle, elapsed / duration);
                cylinderTransform.localEulerAngles = new Vector3(0, 0, angle);
                elapsed += Time.deltaTime;
                yield return null;
            }
            cylinderTransform.localEulerAngles = new Vector3(0, 0, endAngle);

            isCylinderRotating = false;
        }

        public void Reload()
        {
            foreach (var bullet in bullets)
                bullet.SetActive(true);
            if (cylinderTransform)
                cylinderTransform.localRotation = Quaternion.identity;
            currentBulletIndex = 0;
            UpdateAmmoTextUI();
        }

        private void UpdateAmmoTextUI()
        {
            if (ammoTextUI != null)
            {
                int remaining = bullets.Count - currentBulletIndex;
                ammoTextUI.text = remaining.ToString();
            }
        }



        // ==== 아래는 반동 코드(원본 유지) ====
        private void SetupRecoilVariables(SelectEnterEventArgs args)
        {
            controller = args.interactorObject as XRBaseInteractor;
            StartCoroutine(SetupRecoil(interactable.attachEaseInTime));
        }
        private void DestroyRecoilTracker(SelectExitEventArgs args)
        {
            StopAllCoroutines();
            if (recoilTracker) Destroy(recoilTracker.gameObject);
            isRecoiling = false;
        }
        private System.Collections.IEnumerator SetupRecoil(float interactableAttachEaseInTime)
        {
            if (controller == null) yield break;
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
        private void OnEnable() => Application.onBeforeRender += RecoilUpdate;
        private void OnDisable() => Application.onBeforeRender -= RecoilUpdate;
        [UnityEngine.BeforeRenderOrder(101)]
        private void RecoilUpdate()
        {
            if (!isRecoiling) return;
            if (timer < recoilTime / 2f)
            {
                if (Mathf.Abs(recoilAmount) > 0.001f)
                {
                    recoilTracker.position += transform.forward * recoilAmount * Time.deltaTime;
                    transform.position = recoilTracker.position + controllerToAttachDelta;
                }
                if (Mathf.Abs(recoilRotation) > 0.001f)
                    transform.Rotate(Vector3.right, -recoilRotation * Time.deltaTime, Space.Self);

                endOfRecoilPosition = recoilTracker.localPosition;
                endOfRecoilRotation = transform.localRotation;
            }
            else
            {
                float t = Mathf.InverseLerp(recoilTime / 2f, recoilTime, timer);
                recoilTracker.localPosition = Vector3.Lerp(endOfRecoilPosition, Vector3.zero, t);
                var newRotation = Quaternion.Lerp(endOfRecoilRotation, startingRotation, t);
                transform.position = recoilTracker.position + controllerToAttachDelta;
                transform.localRotation = newRotation;
            }
            timer += Time.deltaTime;
            if (timer > recoilTime)
                isRecoiling = false;
        }
    }
}
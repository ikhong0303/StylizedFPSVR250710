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
    public class ProjectileWeapon : MonoBehaviour
    {
        [Header("필수")]
        [SerializeField] private Transform bulletFirePoint;        // ★ 총알이 나가는 위치
        [SerializeField] private Transform laserFirePoint;         // ★ 레이저 시작 위치
        [SerializeField] private Rigidbody projectilePrefab;       // 발사될 총알 프리팹
        [SerializeField] private AudioSource fireAudio;            // 발사 사운드
        [SerializeField] private ParticleSystem cartridgeEjection; // 탄피 이펙트
        [SerializeField] private MatchTransform bulletFlash;       // 총구 섬광
        [SerializeField] private Transform cylinderTransform;      // 실린더(회전용 부모)
        [SerializeField] private List<GameObject> bullets;         // 실린더 안에 보이는 총알 오브젝트(6개 등)

        [Header("설정")]
        [SerializeField] private float bulletSpeed = 150f;         // 총알 속도
        [SerializeField] private float cylinderAngle = 60f;        // 실린더 회전 각도
        [SerializeField] private float cylinderRotateDuration = 0.2f;   // 부드러운 회전 지속시간
        [SerializeField] private TextMeshProUGUI ammoTextUI;       // UI용 TMP

        [Header("Laser Pointer")]
        [SerializeField] private GameObject laserPrefab;           // Emission 머티리얼 원기둥 프리팹
        [SerializeField] private float laserMaxDistance = 20f;     // 최대 거리 (Inspector에서 조절)
        private GameObject activeLaser;

        public float recoilAmount = -0.03f;                        // 반동 이동 거리
        public float recoilRotation = 1;                           // 반동 회전
        public float recoilTime = 0.06f;                           // 반동 지속 시간
        public float hapticDuration = 0.1f;                        // 햅틱 지속 시간
        public float hapticStrength = 0.5f;                        // 햅틱 강도

        [Header("재장전 (Tilt Reload)")]
        [SerializeField] private float reloadAngle = 60f;          // 아래로 얼마나 기울이면 재장전되는지 (degree)
        [SerializeField] private float reloadCooldown = 1f;        // 재장전 연타 방지 쿨타임(초)

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

            if (ammoTextUI != null)
                ammoTextUI.enabled = false;

            // ★ 반드시 추가! ★
            if (laserPrefab != null && activeLaser == null)
                activeLaser = Instantiate(laserPrefab, laserFirePoint.position, Quaternion.identity, transform);
            if (activeLaser != null)
                activeLaser.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (interactable.isSelected && controller != null)
            {
                var gunForward = bulletFirePoint.forward.normalized;
                float dot = Vector3.Dot(gunForward, Vector3.down); // 아래=+1, 위=-1

                if (Mathf.Abs(dot) > 0.766f && Time.time - lastReloadTime > reloadCooldown)
                {
                    Reload();
                    lastReloadTime = Time.time;
                }
            }

            UpdateLaserPointer(); // 에임포인터 관련 코드 완전 제거!
        }

        private void UpdateLaserPointer()
        {
            XRBaseInteractor currInteractor = null;
            if (interactable != null && interactable.interactorsSelecting.Count > 0)
                currInteractor = (XRBaseInteractor)interactable.interactorsSelecting[0];

            bool inSocket = currInteractor is XRSocketInteractor;

            if (!interactable.isSelected || inSocket)
            {
                if (activeLaser != null) activeLaser.SetActive(false);
                return;
            }

            float maxDist = laserMaxDistance;

            RaycastHit hit;
            Vector3 start = laserFirePoint.position;
            Vector3 dir = laserFirePoint.forward;
            bool hasHit = Physics.Raycast(start, dir, out hit, maxDist);

            float dist = hasHit ? hit.distance : maxDist;

            if (activeLaser != null)
            {
                // Cylinder: Y축 위, scale.y = 길이/2
                activeLaser.transform.position = start + dir * (dist * 0.5f);
                activeLaser.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90, 0, 0);
                Vector3 scale = activeLaser.transform.localScale;
                scale.y = dist * 0.5f;
                activeLaser.transform.localScale = scale;
                activeLaser.SetActive(true);
            }
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
            var bullet = Instantiate(projectilePrefab, bulletFirePoint.position, bulletFirePoint.rotation);
            bullet.AddForce(bulletFirePoint.forward * bulletSpeed, ForceMode.VelocityChange);

            // 사운드, 이펙트
            fireAudio?.PlayOneShot(fireAudio.clip);
            cartridgeEjection?.Play();

            // 총구 플래쉬
            if (bulletFlash)
            {
                var flash = Instantiate(bulletFlash);
                flash.transform.position = bulletFirePoint.position;
                flash.positionToMatch = bulletFirePoint;
            }

            // 햅틱(진동)
            if (controller)
            {
                var haptic = controller.GetComponentInParent<HapticImpulsePlayer>();
                haptic?.SendHapticImpulse(hapticStrength, hapticDuration);
            }

            BulletFiredEvent?.Invoke();

            // 실린더 회전
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
            StartRecoil();
            UpdateAmmoTextUI();
        }

        private IEnumerator RotateCylinderSmooth(float targetAngle, float duration)
        {
            isCylinderRotating = true;

            float startAngle = cylinderTransform.localEulerAngles.z;
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

        private void SetupRecoilVariables(SelectEnterEventArgs args)
        {
            controller = args.interactorObject as XRBaseInteractor;
            StartCoroutine(SetupRecoil(interactable.attachEaseInTime));

            if (controller is XRSocketInteractor)
            {
                if (ammoTextUI != null) ammoTextUI.enabled = false;
                if (activeLaser != null) activeLaser.SetActive(false);
                return;
            }

            if (ammoTextUI != null) ammoTextUI.enabled = true;
            if (activeLaser != null) activeLaser.SetActive(true);
        }

        private void DestroyRecoilTracker(SelectExitEventArgs args)
        {
            StopAllCoroutines();
            if (recoilTracker) Destroy(recoilTracker.gameObject);
            isRecoiling = false;

            if (ammoTextUI != null)
                ammoTextUI.enabled = false;
            if (activeLaser != null)
                activeLaser.SetActive(false);
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

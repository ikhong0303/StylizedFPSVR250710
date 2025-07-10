using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace MikeNspired.XRIStarterKit
{
    public class AmmoBackPack : MonoBehaviour
    {
        // 왼손과 오른손 컨트롤러를 지정 (직접 잡을 수 있는 상호작용기)
        public XRDirectInteractor leftHand, rightHand;

        // 복제해서 생성할 탄창 프리팹 2종
        [SerializeField] private XRGrabInteractable magazine, magazine2;

        // 각각의 탄창이 사용될 총기 타입
        [SerializeField] private GunType gunType1, gunType2;

        private XRInteractionManager interactionManager; // 아이템을 손에 쥐게 하기 위한 관리자
        private XRSimpleInteractable simpleInteractable; // 이 오브젝트가 클릭되거나 터치될 수 있도록 하는 컴포넌트

        // 시작 시 실행됨
        private void Start()
        {
            OnValidate();

            // 이 오브젝트가 작동되었을 때(터치 등) CheckControllerGrip 함수 실행
            simpleInteractable.activated.AddListener(CheckControllerGrip);
        }

        // 에디터에서 자동으로 필드 할당하는 용도 (안전장치)
        private void OnValidate()
        {
            if (!simpleInteractable)
                simpleInteractable = GetComponent<XRSimpleInteractable>();
        }

        // 백팩을 터치했을 때 실행되는 함수
        private void CheckControllerGrip(ActivateEventArgs args)
        {
            // 누가(어떤 컨트롤러가) 터치했는지 가져옴
            var controller = args.interactorObject as XRBaseInteractor;
            if (controller == null) return;

            // 그 손이 이미 뭔가를 들고 있다면 아무것도 하지 않음
            if (!IsControllerHoldingObject(controller))
                TryGrabAmmo(controller); // 손이 비었으면 탄창을 쥐게 함
        }

        // 해당 손이 이미 어떤 오브젝트를 잡고 있는지 확인
        private bool IsControllerHoldingObject(XRBaseInteractor controller)
        {
            var directInteractor = controller as XRDirectInteractor;
            return directInteractor != null && directInteractor.interactablesSelected.Count > 0;
        }

        // 탄창을 손에 쥐게 시도하는 함수
        private void TryGrabAmmo(XRBaseInteractor interactor)
        {
            // 누른 손이 왼손이면 → currentInteractor = 왼손 / 다른 손 = 오른손
            XRBaseInteractor currentInteractor = interactor == leftHand ? interactor : rightHand;
            XRBaseInteractor handHoldingWeapon = interactor == leftHand ? rightHand : leftHand;

            // 다른 손이 무기를 들고 있는지 확인
            if (handHoldingWeapon == null || handHoldingWeapon.interactablesSelected.Count == 0) return;
            if (currentInteractor.interactablesSelected.Count > 0) return; // 현재 손도 비어 있어야 함

            // 무기를 들고 있는 손에서 총기 타입 확인
            var gunType = handHoldingWeapon.interactablesSelected[0]
                .transform.GetComponentInChildren<MagazineAttachPoint>()?.GunType;
            if (gunType == null) return;

            // 총기 타입에 따라 탄창 프리팹 결정
            XRGrabInteractable newMagazine = gunType == gunType1 ? Instantiate(magazine) : Instantiate(magazine2);

            // 탄창의 위치를 현재 손 위치로 이동시킴
            newMagazine.transform.position = currentInteractor.transform.position;
            newMagazine.transform.forward = currentInteractor.transform.forward;

            // 탄창을 손에 자동으로 쥐게 함
            StartCoroutine(GrabItem(currentInteractor, newMagazine));
        }

        // 일정 시간 후 탄창을 손에 쥐게 처리하는 코루틴 (물리 엔진과 충돌 방지)
        private IEnumerator GrabItem(XRBaseInteractor currentInteractor, XRGrabInteractable newMagazine)
        {
            // 물리 프레임 한 번 대기
            yield return new WaitForFixedUpdate();

            // 그 사이에 손이 다른 걸 잡으면 취소
            if (currentInteractor.interactablesSelected.Count > 0) yield break;

            // 인터랙션 매니저를 통해 해당 손이 탄창을 잡게 함
            interactionManager.SelectEnter(currentInteractor, (IXRSelectInteractable)newMagazine);
        }
    }
}
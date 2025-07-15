using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace MikeNspired.XRIStarterKit
{
    // 병(Glass Bottle) 오브젝트에 붙는 스크립트.
    // 맞으면 깨지고, 파편과 이펙트/사운드/물체 상태 전환 등을 처리한다.
    public class Bottle : MonoBehaviour, IDamageable
    {
        // [필수] 깨질 때 물 튀기는 이펙트
        public ParticleSystem particleSystemSplash;

        // [필수] 깨진 병(파편) 프리팹
        public GameObject SmashedObject;

        // [필수] 병 안의 액체(깨지면 꺼짐)
        public GameObject Liquid;

        // [필수] 병의 3D 메쉬 (깨지면 꺼짐)
        public GameObject Mesh;

        // [설정값] 파편이 튈 때 힘
        public float glassExplodeForce = 500;

        // [설정값] 파편이 위로 튀는 정도
        public float explodeUpwardModifier = 1.5f;

        // [사운드]
        AudioSource m_AudioSource;

        // [이벤트] 병이 맞았을 때(float damage)를 외부로 알림
        public UnityEventFloat onHit;

        // 오브젝트가 켜질 때(활성화될 때) 파티클 이펙트 정지
        void OnEnable()
        {
            if (particleSystemSplash)
                particleSystemSplash.Stop();
        }

        // --- IDamageable 인터페이스 구현부 ---
        // 총알, 근접타격 등으로 피해를 받으면 이 함수가 호출됨
        public void TakeDamage(float damage, GameObject damager)
        {
            // 1. 맞았다는 이벤트(데미지 값 포함) 알리기
            onHit.Invoke(damage);

            // 2. 랜덤 사운드 재생 (병 깨지는 소리)
            GetComponent<AudioRandomize>().Play();

            // 3. 파티클(물 튀김) 이펙트 실행 (부모에서 분리해서)
            particleSystemSplash.transform.parent = null;
            particleSystemSplash.gameObject.SetActive(true);
            particleSystemSplash.Play();

            // 4. 깨진 병 파편 표시, 액체와 원래 병은 꺼버림
            SmashedObject.SetActive(true);   // 파편 on
            Liquid.SetActive(false);         // 액체 off
            Mesh.SetActive(false);           // 병 메쉬 off

            // 5. 파편에 물리 충격 가하기 (카메라 기준으로 방향 계산)
            Rigidbody[] rbs = SmashedObject.GetComponentsInChildren<Rigidbody>();
            Transform camera = Camera.main.transform;
            var position = transform.position - camera.position; // 병 → 카메라 방향

            foreach (Rigidbody rb in rbs)
            {
                // 파편에 폭발(튕겨나가는) 힘 적용
                rb.AddExplosionForce(
                    glassExplodeForce,
                    SmashedObject.transform.position - position.normalized * 0.25f, // 폭발 중심: 카메라 기준 살짝 앞으로
                    2.0f,      // 폭발 반경
                    explodeUpwardModifier   // 위로 튀는 힘
                );
            }

            // 6. 파편이 씬의 루트에 남게끔 부모 끊기
            SmashedObject.transform.parent = null;

            // 7. 병 본체(이 스크립트 붙은 오브젝트) 3초 뒤에 파괴
            Destroy(gameObject, 3);
        }
    }
}

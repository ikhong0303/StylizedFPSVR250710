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
            onHit.Invoke(damage);
            GetComponent<AudioRandomize>()?.Play();

            // 파티클
            if (particleSystemSplash != null)
            {
                particleSystemSplash.transform.parent = null;
                particleSystemSplash.gameObject.SetActive(true);
                particleSystemSplash.Play();
            }

            // 파편 활성화
            if (SmashedObject != null)
            {
                SmashedObject.SetActive(true);
                // 파편에 물리 충격
                Rigidbody[] rbs = SmashedObject.GetComponentsInChildren<Rigidbody>();
                Transform camera = Camera.main?.transform;
                Vector3 position = (camera != null) ? transform.position - camera.position : Vector3.forward;
                foreach (Rigidbody rb in rbs)
                    rb.AddExplosionForce(glassExplodeForce, SmashedObject.transform.position - position.normalized * 0.25f, 2.0f, explodeUpwardModifier);

                // 부모 끊기
                SmashedObject.transform.parent = null;
            }
            // 액체/메쉬
            if (Liquid != null)
                Liquid.SetActive(false);
            if (Mesh != null)
                Mesh.SetActive(false);

            Destroy(gameObject, 3);
        }

    }
}

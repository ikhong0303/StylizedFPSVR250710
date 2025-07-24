// Author MikeNspired. 

using System;
using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 충돌 시 데미지를 주고, 적절한 데칼(이펙트)을 생성하는 컴포넌트입니다.
    public class SimpleCollisionDamage : MonoBehaviour
    {
        // 가할 데미지 값
        [SerializeField] protected float damage = 10;
        // 금속, 살점, 나무 표면에 생성할 데칼 프리팹
        [SerializeField] private GameObject metalDecal = null;
        [SerializeField] private GameObject fleshDecal = null;
        [SerializeField] private GameObject woodDecal = null;
        // 충돌 시 오브젝트를 파괴할지 여부, 트리거로도 데미지를 줄지 여부
        [SerializeField] private bool destroyOnCollision = true, triggerDamage = false;

        // 물리 충돌이 발생했을 때 호출됩니다.
        private void OnCollisionEnter(Collision collision)
        {
            // 같은 스크립트가 붙은 리지드바디와는 상호작용하지 않음
            if (collision.rigidbody?.GetComponent<SimpleCollisionDamage>()) return;

            // 충돌한 오브젝트 계층에서 IDamageable을 찾고, 있으면 데미지 부여
            var damageable = collision.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
                Damage(damageable);

            // 충돌 표면에 맞는 데칼 생성
            CheckForImpactDecalType(collision);

            // 설정에 따라 자기 자신 파괴
            if (destroyOnCollision)
                Destroy(gameObject);
        }

        // 트리거 충돌이 발생했을 때 호출됩니다.
        private void OnTriggerEnter(Collider other)
        {
            if (!triggerDamage) return;

            // 트리거 충돌 대상에 데미지 부여
            var damageable = other.transform.GetComponentInParent<IDamageable>();
            if (damageable != null)
                Damage(damageable);

            // 설정에 따라 자기 자신 파괴
            if (destroyOnCollision)
                Destroy(gameObject);
        }

        // IDamageable 인터페이스를 통해 데미지 전달
        protected virtual void Damage(IDamageable damageable) => damageable.TakeDamage(damage, gameObject);

        // 충돌 표면의 타입에 따라 적절한 데칼을 생성
        private void CheckForImpactDecalType(Collision collision)
        {
            var impact = collision.transform.GetComponentInParent<IImpactType>();

            if (impact != null)
            {
                var impactType = impact.GetImpactType();
                var shouldReparent = impact.ShouldReparent;
                switch (impactType)
                {
                    case ImpactType.Flesh:
                        SpawnDecal(collision, fleshDecal, shouldReparent);
                        break;
                    case ImpactType.Metal:
                        SpawnDecal(collision, metalDecal, shouldReparent);
                        break;
                    case ImpactType.Wood:
                        SpawnDecal(collision, woodDecal, shouldReparent);
                        break;
                    case ImpactType.Neutral:
                        SpawnDecal(collision, null, shouldReparent);
                        break;
                    default:
                        SpawnDecal(collision, metalDecal, shouldReparent);
                        break;
                }
            }
            else
                // IImpactType이 없으면 기본적으로 metal 데칼 생성
                SpawnDecal(collision, metalDecal, false);
                SpawnDecal(collision, metalDecal, false);
        }

        // 데칼 프리팹을 충돌 지점에 생성하고, 필요시 부모를 설정
        private static void SpawnDecal(Collision hit, GameObject decalPrefab, bool shouldReparent)
        {
            if (!decalPrefab) return;

            var spawnedDecal = Instantiate(decalPrefab, null, true);

            if (shouldReparent)
                spawnedDecal.transform.SetParent(hit.transform);

            var contact = hit.contacts[0]; //맞은 위치 계산 (굉장히 유명)
            spawnedDecal.transform.position = contact.point;
            spawnedDecal.transform.forward = contact.normal;
        }
    }
}
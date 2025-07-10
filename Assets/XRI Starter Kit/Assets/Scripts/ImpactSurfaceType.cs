using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 이 스크립트는 오브젝트가 어떤 '충돌 타입'을 갖고 있는지를 정의하고 제공함
    public class ImpactSurfaceType : MonoBehaviour, IImpactType
    {
        // 이 오브젝트가 어떤 충돌 타입인지 (예: 금속, 나무, 고무 등)
        [SerializeField] private ImpactType impactType;

        // 외부에서 이 충돌 타입을 가져갈 수 있도록 하는 함수 (인터페이스 구현)
        public ImpactType GetImpactType() => impactType;

        // 충돌 후 이 오브젝트가 발사체(또는 효과 등)의 부모가 되어야 하는지 여부
        [SerializeField] private bool shouldReparent;

        // 외부에서 이 값을 읽을 수 있도록 속성으로 제공 (읽기 전용)
        public bool ShouldReparent => shouldReparent;
    }
}
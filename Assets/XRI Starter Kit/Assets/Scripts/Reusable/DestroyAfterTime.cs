// Author MikeNspired. 

using UnityEngine;

namespace MikeNspired.XRIStarterKit
{
    // 지정된 시간 후에 오브젝트를 파괴하는 컴포넌트입니다.
    public class DestroyAfterTime : MonoBehaviour
    {
        // 오브젝트가 파괴되기까지의 시간(초)
        public float timeTillDestroy = 2f;
        // true로 설정 시, 한 프레임 후에 오브젝트를 파괴합니다.
        [SerializeField] private bool destroyAfterFrame = false;
        // true로 설정 시, Awake 시점에 타이머를 자동으로 시작합니다.
        [SerializeField] private bool startTimerOnAwake = true;

        // Start에서 타이머를 자동으로 시작할지 결정합니다.
        private void Start()
        {
            if (startTimerOnAwake) StartTimerToDestroy();
        }

        // 오브젝트 파괴 타이머를 시작합니다.
        // destroyAfterFrame이 true면 한 프레임 후, 아니면 지정된 시간 후에 파괴합니다.
        public void StartTimerToDestroy() => Invoke(nameof(DestroyThis), !destroyAfterFrame ? timeTillDestroy : Time.deltaTime);

        // 오브젝트를 즉시 파괴합니다.
        private void DestroyThis() => DestroyImmediate(gameObject);
    }
}
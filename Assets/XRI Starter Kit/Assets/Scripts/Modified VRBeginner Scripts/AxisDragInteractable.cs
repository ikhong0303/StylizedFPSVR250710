using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.Events; // UnityEventFloat, UnityEventInt를 사용하기 위해 추가

namespace MikeNspired.XRIStarterKit
{
    /// <summary>
    /// Custom interactable that can be dragged along an axis. 
    /// Can either be continuous or snap to integer steps.
    /// </summary>
    public class AxisDragInteractable : XRBaseInteractable
    {
        [Header("Motion Settings")]
        [Tooltip("The Rigidbody that will be moved. If null, the first Rigidbody in children will be used.")]
        public Rigidbody MovingRigidbody;

        [Tooltip("Local axis along which the object can be dragged.")]
        public Vector3 LocalAxis;

        [Tooltip("Maximum distance along the axis the object can travel.")]
        public float AxisLength;

        [Tooltip("Number of discrete steps. If zero, it behaves like a continuous slider.")]
        public int Steps = 0;

        [Tooltip("If true, snapping to steps only happens when the grip is released.")]
        public bool SnapOnlyOnRelease = true;

        [Header("Return Settings")]
        [Tooltip("If true, the object will return to start when not being grabbed.")]
        public bool ReturnOnFree;
        public float ReturnSpeed = 1f;

        [Header("Audio & Events")]
        public AudioClip SnapAudioClip;
        public AudioSource AudioSource;
        public UnityEventFloat OnDragDistance;
        public UnityEventInt OnDragStep;

        private Vector3 m_EndPoint;
        private Vector3 m_StartPoint;
        private Vector3 m_GrabbedOffset;
        private float m_StepLength;
        private int m_CurrentStep;
        private XRBaseInteractor m_GrabbingInteractor;

        void Start()
        {
            // Normalize the specified local axis
            LocalAxis.Normalize();

            // If AxisLength is negative, flip the axis direction & make length positive
            // 이 부분은 LocalAxis가 항상 "미는" 방향을 가리키도록 하는 데 중요합니다.
            if (AxisLength < 0)
            {
                LocalAxis *= -1;
                AxisLength *= -1;
            }

            // Calculate how far one "step" is
            m_StepLength = Steps == 0 ? 0f : (AxisLength / Steps);

            // Cache start & end points in world space
            m_StartPoint = transform.position;
            m_EndPoint = transform.position + transform.TransformDirection(LocalAxis) * AxisLength;

            // If no rigidbody was specified, try to find one
            if (!MovingRigidbody)
                MovingRigidbody = GetComponentInChildren<Rigidbody>();

            m_CurrentStep = 0;

            // Setup audio source
            if (AudioSource && SnapAudioClip)
                AudioSource.clip = SnapAudioClip;
        }

        void OnValidate()
        {
            if (!AudioSource)
                AudioSource = GetComponent<AudioSource>();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            // Only process if actively held
            if (isSelected)
            {
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
                {
                    var worldAxis = transform.TransformDirection(LocalAxis); // 오브젝트의 로컬 축을 월드 축으로 변환
                    // 인터랙터의 위치에서 오브젝트 위치와 잡은 오프셋을 뺀 후, 월드 축으로의 투영 거리를 계산
                    var distance = m_GrabbingInteractor.transform.position - transform.position - m_GrabbedOffset;
                    var projected = Vector3.Dot(distance, worldAxis); // 드래그 방향으로의 투영 거리

                    // --- 이 부분이 한 방향으로만 밀고 당기지 못하게 하는 핵심 수정 부분 ---
                    // projected 값이 음수(당기는 방향)이면 0으로 고정하여 움직임을 막습니다.
                    projected = Mathf.Max(0, projected);
                    // --- 수정 끝 ---

                    // If we have steps & snap is not only on release, snap continuously
                    if (Steps != 0 && !SnapOnlyOnRelease)
                    {
                        int steps = Mathf.RoundToInt(projected / m_StepLength);
                        projected = steps * m_StepLength;
                    }

                    // Determine the final target point, clamped between start & end
                    // 이제 projected는 항상 0 이상이므로, 시작점에서 양의 방향으로만 이동합니다.
                    Vector3 targetPoint = m_StartPoint + worldAxis * Mathf.Clamp(projected, 0, AxisLength);

                    // If we have discrete steps, fire event when crossing a step boundary
                    if (Steps > 0)
                    {
                        // 현재 위치에서 시작점까지의 거리를 계산할 때, 단순히 magnitude를 사용하면 음수 방향으로의 이동이 없으므로
                        // projected 값을 직접 사용하는 것이 더 정확합니다.
                        var posStep = Mathf.RoundToInt(projected / m_StepLength);
                        if (posStep != m_CurrentStep)
                        {
                            AudioSource?.Play();
                            OnDragStep.Invoke(posStep);
                        }
                        m_CurrentStep = posStep;
                    }

                    // Fire distance event
                    // OnDragDistance 이벤트도 projected 값을 기반으로 호출합니다.
                    OnDragDistance.Invoke(projected);

                    // Move the object or its Rigidbody
                    var move = targetPoint - transform.position;
                    if (MovingRigidbody)
                        MovingRigidbody.MovePosition(MovingRigidbody.position + move);
                    else
                        transform.position += move;
                }
            }
            else
            {
                // If not being grabbed & configured to return, move towards start
                if (ReturnOnFree)
                {
                    var targetPoint = Vector3.MoveTowards(transform.position, m_StartPoint, ReturnSpeed * Time.deltaTime);
                    var move = targetPoint - transform.position;
                    if (MovingRigidbody)
                        MovingRigidbody.MovePosition(MovingRigidbody.position + move);
                    else
                        transform.position += move;
                }
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            // Record offset between the object's position and the grab point
            m_GrabbedOffset = args.interactorObject.transform.position - transform.position;
            m_GrabbingInteractor = args.interactorObject as XRBaseInteractor;
            base.OnSelectEntered(args);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            // If snapping only on release, then snap to the nearest step
            if (SnapOnlyOnRelease && Steps != 0)
            {
                // 현재 위치에서 시작점까지의 실제 이동 거리를 계산합니다.
                // LocalAxis 방향으로의 투영 거리를 사용해야 합니다.
                var currentProjectedDistance = Vector3.Dot(transform.position - m_StartPoint, transform.TransformDirection(LocalAxis));

                int step = Mathf.RoundToInt(currentProjectedDistance / m_StepLength);
                // 스텝 위치도 음수 방향으로는 가지 않도록 0 미만으로 내려가지 않게 합니다.
                step = Mathf.Max(0, step);

                var finalDistance = step * m_StepLength;

                transform.position = m_StartPoint + transform.TransformDirection(LocalAxis) * finalDistance;

                if (step != m_CurrentStep)
                    OnDragStep.Invoke(step);
            }
        }

        void OnDrawGizmosSelected()
        {
            // Draw a line & sphere to illustrate the drag axis in the Editor
            // Gizmos는 현재 위치(transform.position)에서 시작하여 LocalAxis 방향으로 그립니다.
            // LocalAxis가 이미 Start()에서 정규화되고 AxisLength에 따라 방향이 결정되므로 그대로 사용합니다.
            var startGizmoPoint = transform.position;
            var endGizmoPoint = m_StartPoint + transform.TransformDirection(LocalAxis.normalized) * AxisLength; // 실제 시작점과 끝점

            Gizmos.color = Color.green; // 시작점부터 끝점까지의 전체 이동 가능 범위
            Gizmos.DrawLine(m_StartPoint, endGizmoPoint);
            Gizmos.DrawSphere(endGizmoPoint, 0.01f);

            // 현재 오브젝트의 위치를 나타내는 작은 구
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.02f);
        }
    }
}
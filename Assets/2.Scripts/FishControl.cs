using UnityEngine;
using MikeNspired.XRIStarterKit;

public class FishControl : MonoBehaviour
{
    [Header("이동할 위치들(좌표)")]
    public Transform[] waypoints;

    [Header("Y(높이) 고정")]
    public float fixedY = 0.5f;

    [Header("대기 시간")]
    public float idleTimeMin = 1f;
    public float idleTimeMax = 3f;

    [Header("방향 랜덤 변경 주기")]
    public float rotationChangeInterval = 2f;   // 몇 초마다 방향 바꿈
    private float rotationTimer = 0f;

    [Header("방향 변화 최대 각도")]
    public float maxTurnAngle = 60f;    // 60도(한쪽), 총 120도 범위

    private float moveSpeed = 1f;
    private int currentWaypoint = 0;
    private Vector3 targetPos;
    private float idleTimer = 0f;
    private bool isMoving = true;
    private Animator animator;
    private FishHealth fishHealth;
    private Quaternion targetRotation;
    public float rotationLerpSpeed = 3.0f; // 회전 부드러움 속도 (크면 빠름)

    void Awake()
    {
        fishHealth = GetComponent<FishHealth>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("FishControl: waypoints가 필요합니다!");
            enabled = false;
            return;
            rotationTimer = rotationChangeInterval;
        }

        currentWaypoint = 0;
        targetPos = WaypointPos(currentWaypoint);
        SetY();
        isMoving = true;
    }

    void Update()
    {
        SetY();
        UpdateMoveSpeed();

        if (!isMoving)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isMoving = true;
                GotoNextWaypoint();
            }
        }
        rotationTimer -= Time.deltaTime;
        if (rotationTimer <= 0f)
        {
            SetRandomRotation();
            rotationTimer = rotationChangeInterval;
        }
    }

    void SetRandomRotation()
    {
        // 현 이동방향 벡터
        Vector3 moveDir = (targetPos - transform.position).normalized;
        if (moveDir.sqrMagnitude < 0.01f) return; // 이동 안하면 무시

        // 이동방향 → 각도(월드)
        float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

        // ±60도 내에서 랜덤 오프셋
        float randomOffset = Random.Range(-maxTurnAngle, maxTurnAngle);
        float newYaw = targetYaw + randomOffset;

        targetRotation = Quaternion.Euler(0, newYaw, 0); // 목표 회전값만 저장!
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        Vector3 next = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        next.y = fixedY;
        transform.position = next; // Rigidbody 없어도 OK

        if (Vector3.Distance(next, targetPos) < 0.2f)
        {
            isMoving = false;
            idleTimer = Random.Range(idleTimeMin, idleTimeMax);
        }
    }

    void UpdateMoveSpeed()
    {
        if (fishHealth != null)
        {
            float hpRatio = fishHealth.CurrentHealth / fishHealth.MaxHealth;
            if (hpRatio <= 0.5f)
            {
                moveSpeed = 3f;
                if (animator != null) animator.SetBool("isFast", true);
            }
            else
            {
                moveSpeed = 0.5f;
                if (animator != null) animator.SetBool("isFast", false);
            }
        }
    }

    void GotoNextWaypoint()
    {
        // ★ 순차 루프!
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        targetPos = WaypointPos(currentWaypoint);
    }

    Vector3 WaypointPos(int idx)
    {
        var pos = waypoints[idx].position;
        pos.y = fixedY;
        return pos;
    }

    void SetY()
    {
        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;
    }
}

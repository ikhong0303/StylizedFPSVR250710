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

    [Header("이동속도")]
    public float normalSpeed = 0.5f;
    public float fastSpeed = 3f;

    [Header("회전 관련")]
    public float rotationChangeInterval = 2f;    // 몇 초마다 방향 바꿈
    public float rotationLerpSpeed = 4.0f;       // 회전 부드러움 속도
    public float maxTurnAngle = 60f;             // ±60도 (총 120도)

    private int currentWaypoint = 0;
    private Vector3 targetPos;
    private float idleTimer = 0f;
    private bool isMoving = true;
    private Animator animator;
    private FishHealth fishHealth;
    private bool isDead = false;

    // 방향 랜덤 관련
    private float rotationTimer = 0f;
    private Quaternion targetRotation;

    private float moveSpeed = 1f;

    void Awake()
    {
        fishHealth = GetComponent<FishHealth>();
        animator = GetComponent<Animator>();

        if (fishHealth != null)
            fishHealth.OnDeath += OnFishDeath;
    }

    private void OnFishDeath()
    {
        isDead = true;
        isMoving = false;  // 즉시 멈춤
                           // 추가로, 멈춰야 하는 게 있다면 여기서 멈춤 처리!
    }

    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("FishControl: waypoints가 필요합니다!");
            enabled = false;
            return;
        }

        currentWaypoint = 0;
        targetPos = WaypointPos(currentWaypoint);
        SetY();
        isMoving = true;

        // 초기 회전 설정
        targetRotation = transform.rotation;
        rotationTimer = rotationChangeInterval;
    }

    void Update()
    {
        if (isDead) return;

        SetY();
        UpdateMoveSpeed();

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);

        // 일정 주기로 새로운 방향으로 부드럽게 회전
        rotationTimer -= Time.deltaTime;
        if (rotationTimer <= 0f)
        {
            SetRandomRotation();
            rotationTimer = rotationChangeInterval;
        }

        if (!isMoving)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isMoving = true;
                GotoNextWaypoint();
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;
        if (!isMoving) return;

        // transform.position 만 사용
        Vector3 next = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        next.y = fixedY;

        transform.position = next;

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
                moveSpeed = fastSpeed;
                if (animator != null) animator.SetBool("isFast", true);
            }
            else
            {
                moveSpeed = normalSpeed;
                if (animator != null) animator.SetBool("isFast", false);
            }
        }
    }

    void GotoNextWaypoint()
    {
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        targetPos = WaypointPos(currentWaypoint);

        // 도착 후 새로운 목표로 회전도 갱신
        SetRandomRotation();
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

    void SetRandomRotation()
    {
        Vector3 moveDir = (targetPos - transform.position).normalized;
        if (moveDir.sqrMagnitude < 0.01f) return; // 이동 안하면 무시

        // 목표 지점 방향
        float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float randomOffset = Random.Range(-maxTurnAngle, maxTurnAngle);
        float newYaw = targetYaw + randomOffset;

        targetRotation = Quaternion.Euler(0, newYaw, 0);
    }
}

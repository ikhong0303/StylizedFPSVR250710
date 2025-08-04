using UnityEngine;

public class FishControl : MonoBehaviour
{
    [Header("연못(원형) 설정")]
    public Vector3 pondCenter = Vector3.zero;
    public float pondRadius = 4.0f;

    [Header("중앙 섬(원형 금지영역)")]
    public Vector3 islandCenter = Vector3.zero;
    public float islandRadius = 1.0f;

    [Header("Y(높이) 고정")]
    public float fixedY = 0.5f;

    [Header("속도 랜덤")]
    public float moveSpeedMin = 1.0f;
    public float moveSpeedMax = 2.0f;
    public float speedChangeInterval = 3.0f;

    [Header("목표 뽑기 옵션")]
    public int maxPickTry = 10;

    private float moveSpeed;
    private float speedTimer = 0f;
    private Vector3 targetPos;

    private Rigidbody rb;
    private float idleTimer = 0f;
    private bool isMoving = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("FishControl: Rigidbody 필요! (isKinematic = true, Gravity Off)");
    }

    void Start()
    {
        SetRandomSpeed();
        speedTimer = speedChangeInterval;
        PickNewTarget();
        SetY();
    }

    void Update()
    {
        // 속도 랜덤 변화
        speedTimer -= Time.deltaTime;
        if (speedTimer <= 0f)
        {
            SetRandomSpeed();
            speedTimer = speedChangeInterval;
        }

        // Y값 고정
        SetY();

        if (!isMoving)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0f)
            {
                isMoving = true;
                PickNewTarget();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving || rb == null) return;

        Vector3 next = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        next.y = fixedY;
        rb.MovePosition(next);

        // 목표 도착
        if (Vector3.Distance(transform.position, targetPos) < 0.2f)
        {
            isMoving = false;
            idleTimer = Random.Range(1f, 3f);
        }
    }

    void PickNewTarget()
    {
        for (int attempt = 0; attempt < maxPickTry; attempt++)
        {
            // 연못(큰 원) 내에서만 목표 뽑기
            Vector2 circle = Random.insideUnitCircle * pondRadius;
            Vector3 candidate = pondCenter + new Vector3(circle.x, 0, circle.y);

            // 섬(작은 원) 반경 내면 무효
            if (Vector3.Distance(candidate, islandCenter) < islandRadius + 0.3f)
                continue;
            // 너무 가까우면 무효
            if (Vector3.Distance(candidate, transform.position) < 0.6f)
                continue;

            candidate.y = fixedY;
            targetPos = candidate;
            return;
        }
        // 실패 시라도 아무 좌표
        Vector2 backupCircle = Random.insideUnitCircle * pondRadius;
        targetPos = pondCenter + new Vector3(backupCircle.x, 0, backupCircle.y);
        targetPos.y = fixedY;
    }

    void SetRandomSpeed() => moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);

    void SetY()
    {
        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pondCenter, pondRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(islandCenter, islandRadius);
    }
}

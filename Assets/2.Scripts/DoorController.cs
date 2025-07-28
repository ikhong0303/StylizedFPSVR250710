using UnityEngine;

public class DoorController : MonoBehaviour
{
    // 회전문(양쪽)
    public Transform doorLeft;
    public Transform doorRight;
    public float openAngle = 120f;
    public float openDuration = 1f;

    // 슬라이딩문
    [Header("슬라이딩 Door (필요할 때만 사용)")]
    public Transform slidingDoor;
    public Vector3 slideDirection = Vector3.left; // -X축이 기본값
    public float slideDistance = 2f;
    public float slideDuration = 1f;

    // 내부 상태
    private bool isOpening = false;
    private float t = 0f;
    // 회전문용
    private Quaternion leftStartRot, leftEndRot;
    private Quaternion rightStartRot, rightEndRot;
    // 슬라이딩문용
    private Vector3 slideStartPos, slideEndPos;

    // Door 타입 선택
    public enum DoorType { Rotate, Slide }
    public DoorType doorType = DoorType.Rotate;

    public void OpenDoor()
    {
        if (isOpening) return;
        t = 0f;
        isOpening = true;

        if (doorType == DoorType.Rotate)
        {
            if (doorLeft)
            {
                leftStartRot = doorLeft.localRotation;
                leftEndRot = Quaternion.Euler(0f, -openAngle, 0f);
            }
            if (doorRight)
            {
                rightStartRot = doorRight.localRotation;
                rightEndRot = Quaternion.Euler(0f, openAngle, 0f);
            }
        }
        else if (doorType == DoorType.Slide)
        {
            if (slidingDoor)
            {
                slideStartPos = slidingDoor.localPosition;
                slideEndPos = slideStartPos + slideDirection.normalized * slideDistance;
            }
        }
    }

    public void CloseDoor()
    {
        isOpening = false;
        t = 0f;
        if (doorType == DoorType.Rotate)
        {
            if (doorLeft) doorLeft.localRotation = Quaternion.identity;
            if (doorRight) doorRight.localRotation = Quaternion.identity;
        }
        else if (doorType == DoorType.Slide)
        {
            if (slidingDoor) slidingDoor.localPosition = slideStartPos;
        }
    }

    void Update()
    {
        if (!isOpening) return;

        if (doorType == DoorType.Rotate)
        {
            t += Time.deltaTime / openDuration;
            float clampT = Mathf.Clamp01(t);

            if (doorLeft) doorLeft.localRotation = Quaternion.Slerp(leftStartRot, leftEndRot, clampT);
            if (doorRight) doorRight.localRotation = Quaternion.Slerp(rightStartRot, rightEndRot, clampT);

            if (clampT >= 1f)
                isOpening = false;
        }
        else if (doorType == DoorType.Slide)
        {
            t += Time.deltaTime / slideDuration;
            float clampT = Mathf.Clamp01(t);

            if (slidingDoor)
                slidingDoor.localPosition = Vector3.Lerp(slideStartPos, slideEndPos, clampT);

            if (clampT >= 1f)
                isOpening = false;
        }
    }
}

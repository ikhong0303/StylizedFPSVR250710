using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform doorLeft;
    public Transform doorRight;
    public float openAngle = 120f;
    public float openDuration = 1f;

    private bool isOpening = false;
    private float t = 0f;
    private Quaternion leftStartRot, leftEndRot;
    private Quaternion rightStartRot, rightEndRot;

    public void OpenDoor()
    {
        if (isOpening) return;
        leftStartRot = doorLeft.localRotation;
        leftEndRot = Quaternion.Euler(0f, -openAngle, 0f);
        rightStartRot = doorRight.localRotation;
        rightEndRot = Quaternion.Euler(0f, openAngle, 0f);
        t = 0f;
        isOpening = true;
    }

    public void CloseDoor()
    {
        // 문을 즉시 닫음
        doorLeft.localRotation = Quaternion.identity;
        doorRight.localRotation = Quaternion.identity;
        isOpening = false; // 열림 중이더라도 즉시 중단
    }

    void Update()
    {
        if (!isOpening) return;

        t += Time.deltaTime / openDuration;
        if (t > 1f) t = 1f;

        doorLeft.localRotation = Quaternion.Slerp(leftStartRot, leftEndRot, t);
        doorRight.localRotation = Quaternion.Slerp(rightStartRot, rightEndRot, t);

        if (t >= 1f)
            isOpening = false;
    }
}

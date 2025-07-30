using UnityEngine;
using System.Collections;
using GuidanceLine;
using UnityEditor.EditorTools;

public class LabStageManager : MonoBehaviour
{

    [SerializeField] GameObject missionPopupPanel;
    [SerializeField] Siren siren;
    [SerializeField] DoorKnobTrigger doorKnob;
    [SerializeField] GameObject SceneTrigger01;
    [SerializeField] GameObject LabDoor;

    public int leverPulledCount = 0;
    public bool BothLeversPulled => leverPulledCount >= 2;

    public void OnLeverActivated() { leverPulledCount++; }
    public void OnLeverDeactivated() { leverPulledCount--; }

    private void Start()
    {
        StartCoroutine(StageFlow());
    }

    IEnumerator StageFlow()
    {
        // 1. 게임 시작, 3초 대기 → 나레이션1
        yield return new WaitForSeconds(3f);
        BgmManager.Instance.PlayBGM("Bgm01", true);
        AudioManager.Instance.PlayNarration("LabNarr01");
        yield return WaitNarration(); // 나레이션이 끝날 때까지 대기

        // 2. 미션 팝업 띄우고, 2초 대기 → 나레이션2
        yield return new WaitForSeconds(2f);
        missionPopupPanel.SetActive(true);         // 팝업 패널 띄우기
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.PlayNarration("LabNarr02");
        yield return WaitNarration();

        // 3. 홀스터에 총 수납될 때까지 대기 → 경고음/경광등
        yield return new WaitUntil(() => siren.BothHolstered);
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlayNarration("LabNarr03");
        yield return WaitNarration();

        // 4. 문고리 클릭 대기 → 효과음, 3초 후 나레이션4
        yield return new WaitUntil(() => doorKnob.knobTriggered);
        AudioManager.Instance.PlaySFX("LabSfx01");
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlayNarration("LabNarr04");
        yield return WaitNarration();

        // 5. 레버 2개 조작까지 대기 → 문 열림 효과음
        yield return new WaitUntil(() => BothLeversPulled); //
        AudioManager.Instance.PlaySFX("door_open");
        SceneTrigger01.SetActive(true); // 문 열림 트리거 활성화
        yield return RotateDoor(LabDoor, -30f, 1.0f);


    }

    IEnumerator RotateDoor(GameObject door, float yDeltaAngle, float duration)
    {
        if (door == null)
            yield break;

        Quaternion startRot = door.transform.rotation;
        // 목표 각도 계산 (현재 Y + yDeltaAngle)
        Vector3 euler = door.transform.eulerAngles;
        Quaternion targetRot = Quaternion.Euler(
            euler.x,
            euler.y + yDeltaAngle,
            euler.z
        );

        float elapsed = 0f;
        while (elapsed < duration)
        {
            door.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        door.transform.rotation = targetRot; // 마지막엔 정확히 맞추기!
    }


    IEnumerator WaitNarration()
    {
        yield return new WaitUntil(() => !AudioManager.Instance.IsNarrationPlaying());
    }

}

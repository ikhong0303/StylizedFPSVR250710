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
        // 1. ���� ����, 3�� ��� �� �����̼�1
        yield return new WaitForSeconds(3f);
        BgmManager.Instance.PlayBGM("Bgm01", true);
        AudioManager.Instance.PlayNarration("LabNarr01");
        yield return WaitNarration(); // �����̼��� ���� ������ ���

        // 2. �̼� �˾� ����, 2�� ��� �� �����̼�2
        yield return new WaitForSeconds(2f);
        missionPopupPanel.SetActive(true);         // �˾� �г� ����
        yield return new WaitForSeconds(2f);
        AudioManager.Instance.PlayNarration("LabNarr02");
        yield return WaitNarration();

        // 3. Ȧ���Ϳ� �� ������ ������ ��� �� �����/�汤��
        yield return new WaitUntil(() => siren.BothHolstered);
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlayNarration("LabNarr03");
        yield return WaitNarration();

        // 4. ���� Ŭ�� ��� �� ȿ����, 3�� �� �����̼�4
        yield return new WaitUntil(() => doorKnob.knobTriggered);
        AudioManager.Instance.PlaySFX("LabSfx01");
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlayNarration("LabNarr04");
        yield return WaitNarration();

        // 5. ���� 2�� ���۱��� ��� �� �� ���� ȿ����
        yield return new WaitUntil(() => BothLeversPulled); //
        AudioManager.Instance.PlaySFX("door_open");
        SceneTrigger01.SetActive(true); // �� ���� Ʈ���� Ȱ��ȭ
        yield return RotateDoor(LabDoor, -30f, 1.0f);


    }

    IEnumerator RotateDoor(GameObject door, float yDeltaAngle, float duration)
    {
        if (door == null)
            yield break;

        Quaternion startRot = door.transform.rotation;
        // ��ǥ ���� ��� (���� Y + yDeltaAngle)
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
        door.transform.rotation = targetRot; // �������� ��Ȯ�� ���߱�!
    }


    IEnumerator WaitNarration()
    {
        yield return new WaitUntil(() => !AudioManager.Instance.IsNarrationPlaying());
    }

}

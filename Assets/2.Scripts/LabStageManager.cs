using UnityEngine;
using System.Collections;
using GuidanceLine;
using UnityEditor.EditorTools;
using MikeNspired.XRIStarterKit.ChrisNolet;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class LabStageManager : MonoBehaviour
{

    [SerializeField] GameObject missionPopupPanel;
    [SerializeField] GameObject tutoPopupPanel;
    [SerializeField] Siren siren;
    [SerializeField] DoorKnobTrigger doorKnob;
    [SerializeField] GameObject SceneTrigger01;
    [SerializeField] GameObject LabDoor;
    [SerializeField] private MikeNspired.XRIStarterKit.ChrisNolet.Outline targetOutline;
    [SerializeField] private MikeNspired.XRIStarterKit.ChrisNolet.Outline leftGun;
    [SerializeField] private MikeNspired.XRIStarterKit.ChrisNolet.Outline rightGun;
    [SerializeField] private Image blackImage; // Inspector에서 연결
    public HideOnClick hideOnClick;

    [Header("비디오 재생 세팅")]
    public VideoPlayer videoPlayer;   // Inspector에서 할당 (VideoManager의 VideoPlayer)
    public VideoClip videoClip;       // Inspector에서 할당 (Assets의 mp4)
    public GameObject videoQuad;      // Inspector에서 할당 (영상 출력할 Quad)
    private bool hasPlayed = false;   // 한 번만 재생 (중복 방지)




    public int leverPulledCount = 0;
    public bool BothLeversPulled => leverPulledCount >= 2;

    public void OnLeverActivated() { leverPulledCount++; }
    public void OnLeverDeactivated() { leverPulledCount--; }

    private void Start()
    {
        if (targetOutline != null)
            targetOutline.enabled = false;
        if (leftGun != null)
            leftGun.enabled = false;
        if (rightGun != null)
            rightGun.enabled = false;
        hideOnClick.OnHide += () => StartCoroutine(MyAfterHideCoroutine());


    }


    IEnumerator MyAfterHideCoroutine()
    {
        Debug.Log("오브젝트가 꺼지는 순간, 여기서 뭔가 시작!");
        yield return new WaitForSeconds(1f);
        StartCoroutine(StageFlow());

    }

    IEnumerator StageFlow()
    {
        yield return new WaitForSeconds(3f);

        if (videoClip != null && videoPlayer != null)
        {
            videoPlayer.clip = videoClip;
            // 영상 출력 Quad를 보이게 (비활성화 상태였다면)
            if (videoQuad != null) videoQuad.SetActive(true);
            videoPlayer.Play();
            videoPlayer.loopPointReached += OnVideoEnd;  // ★ 영상 끝나면 이벤트 등록!
        }      
        yield return new WaitForSeconds(88f);
        blackImage.gameObject.SetActive(true); // 검은 배경 이미지 활성화
        StartCoroutine(FadeToAlpha(0f, 4f));
        BgmManager.Instance.PlayBGM("Bgm01", true);

        yield return new WaitForSeconds(3f);

        AudioManager.Instance.PlayNarration("LabNarr01");
        yield return WaitNarration(); // 나레이션이 끝날 때까지 대기

        // 2. 미션 팝업 띄우고, 2초 대기 → 나레이션2
        yield return new WaitForSeconds(2f);
        missionPopupPanel.SetActive(true);         // 팝업 패널 띄우기
        tutoPopupPanel.SetActive(true);         // 팝업 패널 띄우기

        yield return new WaitForSeconds(2f);
        AudioManager.Instance.PlayNarration("LabNarr02");
        if (leftGun != null)
            leftGun.enabled = true;
        if (rightGun != null)
            rightGun.enabled = true;

        yield return WaitNarration();

        // 3. 홀스터에 총 수납될 때까지 대기 → 경고음/경광등
        yield return new WaitUntil(() => siren.BothHolstered);
        if (targetOutline != null)
            targetOutline.enabled = true;
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

    public IEnumerator FadeToAlpha(float targetAlpha, float duration)
    {
        if (blackImage == null) yield break;

        Color color = blackImage.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            blackImage.color = color;
            yield return null;
        }
        // 마지막 값 보정
        color.a = targetAlpha;
        blackImage.color = color;
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

    private void OnVideoEnd(VideoPlayer vp)
    {
        // 영상 Quad 숨김 (필요시)
        if (videoQuad != null) videoQuad.SetActive(false);

    }

    IEnumerator WaitNarration()
    {
        yield return new WaitUntil(() => !AudioManager.Instance.IsNarrationPlaying());
    }

}

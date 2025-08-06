using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoTrigger01 : MonoBehaviour
{
    [Header("비디오 재생 세팅")]
    public VideoPlayer videoPlayer;   // Inspector에서 할당 (VideoManager의 VideoPlayer)
    public VideoClip videoClip;       // Inspector에서 할당 (Assets의 mp4)
    public GameObject videoQuad;      // Inspector에서 할당 (영상 출력할 Quad)

    private bool hasPlayed = false;   // 한 번만 재생 (중복 방지)

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {
            hasPlayed = true;

            if (videoPlayer == null)
                Debug.LogWarning("VideoPlayer가 할당되지 않았습니다!");

            if (videoClip != null && videoPlayer != null)
            {
                videoPlayer.clip = videoClip;
                // 영상 출력 Quad를 보이게 (비활성화 상태였다면)
                if (videoQuad != null) videoQuad.SetActive(true);
                videoPlayer.Play();
                videoPlayer.loopPointReached += OnVideoEnd;  // ★ 영상 끝나면 이벤트 등록!
            }
            else
            {
                Debug.LogWarning("VideoPlayer나 VideoClip이 할당되지 않았습니다!");
            }
        }
    }

    // ★ 영상이 끝나면 자동으로 호출됨!
    private void OnVideoEnd(VideoPlayer vp)
    {
        // 영상 Quad 숨김 (필요시)
        if (videoQuad != null) videoQuad.SetActive(false);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // 씬이 존재하는지 체크 (없으면 0번으로 돌아가게)
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            SceneManager.LoadScene(0); // 마지막 씬이면 첫 씬(0번)으로
    }
}

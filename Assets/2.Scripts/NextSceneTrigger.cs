using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 현재 씬의 build index를 가져와서, 다음 번호로 이동
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            // 씬이 존재하는지 체크 (없으면 0번으로 돌아가게)
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextIndex);
            else
                SceneManager.LoadScene(0); // 마지막 씬이면 첫 씬(0번)으로
        }
    }
}
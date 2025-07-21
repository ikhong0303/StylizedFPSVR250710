using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ���� ���� build index�� �����ͼ�, ���� ��ȣ�� �̵�
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            // ���� �����ϴ��� üũ (������ 0������ ���ư���)
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(nextIndex);
            else
                SceneManager.LoadScene(0); // ������ ���̸� ù ��(0��)����
        }
    }
}
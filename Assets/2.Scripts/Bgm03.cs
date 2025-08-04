using UnityEngine;

public class Bgm03 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // BGM 재생
            BgmManager.Instance.PlayBGM("Bgm03", true);

            // 한 번 트리거 되면 오브젝트 비활성화
            gameObject.SetActive(false);
        }
    }
}
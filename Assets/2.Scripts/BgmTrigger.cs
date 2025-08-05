using UnityEngine;

public class BgmTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BgmManager.Instance.PlayNextBGM();
            gameObject.SetActive(false); // ¶Ç´Â Destroy(gameObject);
        }
    }
}

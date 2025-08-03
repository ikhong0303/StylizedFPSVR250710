using UnityEngine;

public class BulletLiquid : MonoBehaviour
{
    public Material liquidMaterial; // Inspector에 할당 (교체할 머티리얼)

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LiquidZone"))
        {
            Debug.Log("트리거 감지, 머티리얼 교체 시도");
            if (rend != null && liquidMaterial != null)
            {
                rend.material = liquidMaterial;
            }
            else
            {
                Debug.LogWarning("Renderer 또는 Material이 할당되지 않음!");
            }
        }
    }

}

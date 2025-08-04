using UnityEngine;

public class BulletLiquid : MonoBehaviour
{
    public Material liquidMaterial; // Inspector에 할당

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LiquidZone"))
        {
            Debug.Log("트리거 감지, 머티리얼 전체 교체 시도");

            // 하위 모든 Renderer 찾아서 material 교체
            var renderers = GetComponentsInChildren<Renderer>();
            int changedCount = 0;
            foreach (var rend in renderers)
            {
                // Capsule만 바꾸고 싶으면 이름 체크
                if (rend.gameObject.name.Contains("Capsule"))
                {
                    rend.material = liquidMaterial;
                    changedCount++;
                }
            }
            Debug.Log($"총 {changedCount}개의 Capsule의 머티리얼을 교체했습니다.");
        }
    }
}
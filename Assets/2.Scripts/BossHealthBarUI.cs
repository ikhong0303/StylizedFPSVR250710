using MikeNspired.XRIStarterKit;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    public Image fillImage;
    public TMP_Text hpText;
    public Transform targetToLook; // player의 camera transform 등
    public Vector3 offset = Vector3.up * 2.5f; // 머리 위 위치

    private BossHealth bossHealth;

    public void SetTarget(BossHealth health, Transform lookTarget)
    {
        bossHealth = health;
        targetToLook = lookTarget;
        UpdateBar();
    }

    void FixedUpdate()
    {
        // 항상 target을 바라보도록(빌보드)
        if (targetToLook != null)
        {
            transform.LookAt(transform.position + (transform.position - targetToLook.position));
        }

        if (bossHealth != null)
        {
            UpdateBar();
        }
    }


    void UpdateBar()
    {
        float t = bossHealth.CurrentHealth / bossHealth.MaxHealth;
        fillImage.fillAmount = t;
        hpText.text = Mathf.CeilToInt(bossHealth.CurrentHealth) + " / " + Mathf.CeilToInt(bossHealth.MaxHealth);
    }
}

using UnityEngine;
using System.Collections;
using MikeNspired.XRIStarterKit;
using System.Collections.Generic;

public class StageManager3 : MonoBehaviour
{
    [SerializeField] private FishDeath fishDeath;
    [SerializeField] private GameObject missionPopupPanel;
    [SerializeField] private GameObject naviLine;
    [SerializeField] private WaterRed waterRed;
    [SerializeField] private GameObject CloudEffect;
    [SerializeField] private ParticleSystem particleSystemTarget;
    [SerializeField] GameObject bossObject;
    [SerializeField] private BossHealth bossHealth;


    private void Start()
    {
        GameObject gm = GameObject.Find("GameManager");
        if (gm != null)
        {
            Transform slots = gm.transform.Find("Slots");
            if (slots != null)
            {
                Transform mission = slots.Find("Mission");
                if (mission != null)
                {
                    missionPopupPanel = mission.gameObject;
                }
            }
        }

        if (fishDeath != null)
            fishDeath.OnSinkStart += OnFishSinkStart;
        if (bossHealth != null)
            bossHealth.OnDeath += OnBossDeath;

        StartCoroutine(StageFlow());
    }

    private IEnumerator StageFlow()
    {
        BgmManager.Instance.PlayBGM("Bgm05");
        yield return new WaitForSeconds(3f);

        if (missionPopupPanel != null)
            missionPopupPanel.SetActive(true);
        if (naviLine != null)
            naviLine.SetActive(true);

        yield return new WaitForSeconds(2f);

        AudioManager3.Instance.PlayNarration("GardenNarr01");
    }

    private void OnFishSinkStart()
    {
        StartCoroutine(PlayDeathSFXSequence());
    }

    private IEnumerator PlayDeathSFXSequence()
    {
        waterRed.triggerRed = true;

        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Wind");
        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Clood");
        yield return new WaitForSeconds(1f);
        AudioManager3.Instance.PlaySFX("Garden_Thunder");


        CloudEffect.SetActive(true);
        yield return new WaitForSeconds(5f);
        AudioManager3.Instance.PlayNarration("GardenNarr03");
        BgmManager.Instance.PlayBGM("Bgm06");
        yield return new WaitForSeconds(11.8f);

        bossObject.SetActive(true);
        yield return new WaitForSeconds(12f);

        var main = particleSystemTarget.main;
        Color c = main.startColor.color;
        c.a = 0.15f;
        main.startColor = c;


    }

    private void OnBossDeath()
    {
        StartCoroutine(BossDeathSequence());
    }

    private IEnumerator BossDeathSequence()
    {
        Debug.Log("보스 사망! 여기서부터 코루틴 동작.");

        yield return new WaitForSeconds(8f);
        AudioManager3.Instance.PlayNarration("GardenNarr02");
        // 원하는 연출, UI, 이펙트 등...

        yield break; 
    }

    private void OnDestroy()
    {
        if (fishDeath != null)
            fishDeath.OnSinkStart -= OnFishSinkStart;
        if (bossHealth != null)
            bossHealth.OnDeath -= OnBossDeath;
    }
}

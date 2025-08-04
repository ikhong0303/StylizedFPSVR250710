using UnityEngine;
using System.Collections;
using MikeNspired.XRIStarterKit;
using System.Collections.Generic;

public class StageManager3 : MonoBehaviour
{
    [SerializeField] private FishHealth fishHealth;
    [SerializeField] private GameObject missionPopupPanel;
    [SerializeField] private GameObject naviLine;
    [SerializeField] private WaterRed waterRed;
    [SerializeField] private GameObject CloudEffect;
    [SerializeField] private ParticleSystem particleSystemTarget;
    [SerializeField] GameObject bossObject;


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

        if (fishHealth != null)
        {
            fishHealth.OnDeath += OnFishDeath;
        }

        StartCoroutine(StageFlow());
    }

    private IEnumerator StageFlow()
    {
        yield return new WaitForSeconds(3f);

        if (missionPopupPanel != null)
            missionPopupPanel.SetActive(true);
        if (naviLine != null)
            naviLine.SetActive(true);

        yield return new WaitForSeconds(2f);

        AudioManager3.Instance.PlayNarration("GardenNarr01");
    }

    private void OnFishDeath()
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
        yield return new WaitForSeconds(14f);

        bossObject.SetActive(true);
        yield return new WaitForSeconds(12f);

        var main = particleSystemTarget.main;
        Color c = main.startColor.color;
        c.a = 0.04f;
        main.startColor = c;


    }

    private void OnDestroy()
    {
        if (fishHealth != null)
        {
            fishHealth.OnDeath -= OnFishDeath;
        }
    }
}

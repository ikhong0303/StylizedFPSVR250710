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

        yield return new WaitForSeconds(2f);

        AudioManager3.Instance.PlaySFX("Garden_Clood");
        AudioManager3.Instance.PlaySFX("Garden_Wind");
        AudioManager3.Instance.PlaySFX("Garden_Thunder");
    }

    private void OnDestroy()
    {
        if (fishHealth != null)
        {
            fishHealth.OnDeath -= OnFishDeath;
        }
    }
}

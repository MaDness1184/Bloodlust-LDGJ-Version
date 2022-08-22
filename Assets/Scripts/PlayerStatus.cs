using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : EntityStatus
{
    [SerializeField] private int bloodMeter = 150;
    [SerializeField] private Text playerUI;
    [SerializeField] private Text playerUI2;

    private PlayerController playerController;
    private int cachedDay = -1;
    protected override void Start()
    {
        base.Start();
        playerUI.gameObject.SetActive(true);
        playerUI2.gameObject.SetActive(true);
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        var currentDay = (int) GameManager.main.currentDay;

        if(cachedDay != currentDay)
        {
            cachedDay = currentDay;
            bloodMeter -= 50;
            //Debug.Log("Bloodmeter = " + bloodMeter);
        }

        if (bloodMeter <= 0) RecieveDamage(100);

        var builder1 = "";
        builder1 += "Health: " + currentHP;

        /*for (int i = 0; i < currentHP; i++)
            builder1 += '\u2661';*/
        builder1 += "\nBlood: " + bloodMeter;
        /*for (int i = 0; i < currentHP; i++)
            builder1 += '\u2661';*/

        var builder2 = "\nEquiped Weapon: " + playerController.GetCurrentWeapn().name +
            "\n>Damage: " + playerController.GetCurrentWeapn().damage +
            "\n>Range: " + playerController.GetCurrentWeapn().range +
            "\n>Cdr: " + playerController.GetCurrentWeapn().attackCdr;

        /* for (int i = 0; i < bloodMeter / 10; i++)
             builder += '\u2662';*/



        playerUI.text = builder2;
        playerUI2.text = builder1;
    }

    public override void OnDeath()
    {
        Debug.Log("Player died");
        Camera.main.transform.parent = null;
        FadeUI.main.StartFadeCoroutine();
        Destroy(gameObject);
    }

    public void FillBlood(int value)
    {
        bloodMeter += value;
        //Debug.Log("Bloodmeter = " + bloodMeter);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    public GameObject panel;
    public GameObject shade;
    public static int towerlevel;
    public int maxlvl;
    public GameObject Error;

    //order not final, merely placeholder blurbs for now
    private string[] blurbs = {"This is the tower", "This is the slingshot", "this is the arrow", "this is the ice shard", "this is the soul axe", "this is the rocket", "this is the fireball", "Thsi is the shotgun" };

    //each item is a level indication for a weapon/tower with 0 indicating unpurchased and -1 indicating locked
    //listed in the same order as the blurbs
    public static int[] cannonlevels = {1, 1, 1, 1, -1, -1, -1, -1};

    //each row represents prices for a single weapon/tower with the first being the pruchase price and the rest upgrade prices except for the tower which has only upgrade prices
    //listed in the same order as the blurbs
    private int[,] prices = {
                                {0, 300, 600, 900},
                                {0, 200, 200, 300},
                                {0, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300}
                            };

    private int weapon;

    public Text weaponT;
    public Text weaponCostT;
    public Text DescriptionT;
    public Text PurchaseText;
    public Text leveltext;

    public int gold;
    private int price;

    private string[] names = { "Tower", "Slinghsot", "Arrow", "Ice Shard", "Soul Axe", "Rocket", "Fireball", "Shotgun" };

    private bool gameHasStarted = false;
    public static Shop ShopInstance { get; private set; }

    public Text goldText;
    public GameObject Buttons;

    void Start()
    {
        //allow advertisements
        Advertisement.Initialize("3577863", true);

        //create a shopInstance
        if (ShopInstance == null) 
            ShopInstance = this; 

        //display the amount of gold the player has
        goldText.text = "" + gold;

        //Lock the weapons whose cannonsLevels index is -1 (not bought)
        for (int i = 0; i < cannonlevels.Length; i++)
        {
            if (cannonlevels[i] < 0)
            {
                Buttons.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                Buttons.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    //If a bought weapon's icon is pressed, popup the weapon description panel
    public void Select(int indx)
    {
        //keep track of which weapon was opened
        weapon = indx;

        if (cannonlevels[indx] != -1)
        {
            panel.SetActive(true);
            shade.SetActive(true);
            weaponT.text = names[indx];
            DescriptionT.text = blurbs[indx];
            if (cannonlevels[indx] < maxlvl)
                weaponCostT.text = "" + prices[indx, cannonlevels[indx]];

            if (cannonlevels[indx] != 0)
                PurchaseText.text = "Upgrade";
            else
                PurchaseText.text = "Buy";

            if (cannonlevels[indx] < 1)
                leveltext.text = "Level 1";
            else
                leveltext.text = "Level " + cannonlevels[indx];
        }
    }

    //Exit a weapon's popup panel
    public void ExitPanel()
    {
        panel.SetActive(false);
        shade.SetActive(false);
    }

    //Upgrade a weapon
    public void upgrade() {
        
        int weaponlevel = cannonlevels[weapon];
        int weaponPrice = prices[weapon, weaponlevel+1];

        //if you have enough gold, upgrade the weapon
        if (gold >= weaponPrice) {
            gold -= weaponPrice;
            cannonlevels[weapon]++;
        }

        //Otherwise, play error sound + popup
        else {
            Error.SetActive(true);
            Error.transform.GetChild(0).GetComponent<Text>().text = "Not enough gold";
        }
    }

    //player wants to watch an ad for Gold
    public void deployAd()
    {
        StartCoroutine(ShowAdWhenReady());
    }

    //Wait till an ad is retrieved, then show it
    private IEnumerator ShowAdWhenReady()
    {
        if (!Advertisement.IsReady()) {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ShowAdWhenReady());
        }
        else {
            Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });
        }
    }

    //Handle the ad result
    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Player watched ad");
                break;
            case ShowResult.Skipped:
                Debug.Log("Player skipped ad");
                break;
            case ShowResult.Failed:
                Debug.Log("No internet");
                break;
        }
    }

    //Return to the shop
    public void GoBack()
    {
        SceneManager.LoadScene(0);
    }

    //Disable the error popup
    public void DisableError()
    {
        Error.SetActive(false);
    }
}

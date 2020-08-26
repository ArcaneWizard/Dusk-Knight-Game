using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public GameObject panel;
    public GameObject shade;
    public static int towerlevel;
    public int maxlvl;

    //order not final, merely placeholder blurbs for now
    private string[] blurbs = {"This is the tower", "This is the slingshot", "this is the arrow", "this is the ice shard", "this is the soul axe", "this is the rocket", "this is the fireball", "Thsi is the shotgun" };

    //each item is a level indication for a weapon/tower with 0 indicating unpurchased and -1 indicating locked
    //listed in the same order as the blurbs
    public static int[] cannonlevels = {1, 1, 1, 1, -1, -1, -1, -1};

    //each row represents prices for a single weapon/tower with the first being the pruchase price and the rest upgrade prices except for the tower which has only upgrade prices
    //listed in the same order as the blurbs
    private int[,] prices = {
                                {0, 300, 600, 900},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300},
                                {400, 200, 200, 300}
                            };


    public Text weaponT;
    public Text weaponCostT;
    public Text DescriptionT;
    public Text PurchaseText;
    public Text leveltext;

    public Button purchaseButton;
    public Image bought;

    public int jewels;
    private int price;

    private string[] names = { "Tower", "Slinghsot", "Arrow", "Ice Shard", "Soul Axe", "Rocket", "Fireball", "Shotgun" };

    private bool gameHasStarted = false;
    public static Shop ShopInstance { get; private set; }

    public Text jewelText;
    public Image tower;
    public Image CB_Image;
    public GameObject Buttons;

    // Start is called before the first frame update
    void Awake()
    {
        if (ShopInstance == null) { ShopInstance = this; }

        jewels = PlayerPrefs.GetInt("Jewels");
        Advertisement.Initialize("3577863", true);
        gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < cannonlevels.Length; i++)
        {
            if (cannonlevels[i] < 0)
            {
                Buttons.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                Buttons.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    public void Select(int indx)
    {
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

    public void ExitPanel()
    {
        panel.SetActive(false);
        shade.SetActive(false);
    }

    public void ResetUpgrades()
    {
        for (int i = 0; i < names.Length; i++)
        {
            PlayerPrefs.SetInt(names[i], 0);
        }
    }

    void Update()
    {
        PlayerPrefs.SetInt("Jewels", jewels);

        if (jewelText.gameObject != null)
        jewelText.text = jewels.ToString();

        //Change in-game menu buttons as you equip weapons
        for (int i = 0; i < names.Length; i++)
        {
            if (PlayerPrefs.GetInt(names[i]) == 1)
            {
                Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (buttons[j].gameObject.name == names[i] + "!") 
                        buttons[j].gameObject.SetActive(true);
                }
            }
        }

    }

    public void deployAd()
    {
        StartCoroutine(ShowBannerWhenReady());
    }

    private IEnumerator ShowBannerWhenReady()
    {
        if (!Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(ShowBannerWhenReady());
        }
        else
        {
            if (transform.GetChild(0).transform.GetChild(6).gameObject.name == "Back Button")
                transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(false);
            else
                Debug.LogError("Back Button's child position was changed. Please revert your changes.");

            if (transform.GetChild(0).transform.GetChild(2).gameObject.name == "Ad Panel")
                transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
            else
                Debug.LogError("Ad Panel's child position was changed. Please revert your changes.");

            StartCoroutine(turnOnAdPanel());
            Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });
        }
    }

    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Player watched ad");
                transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(true);
                jewels += 50;
                break;
            case ShowResult.Skipped:
                Debug.Log("Player skipped ad");
                transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(true);
                break;
            case ShowResult.Failed:
                Debug.Log("No internet");
                transform.GetChild(0).transform.GetChild(6).gameObject.SetActive(true);
                break;
        }
    }

    private IEnumerator turnOnAdPanel()
    {
        yield return new WaitForSeconds(60f);
        if (transform.GetChild(0).transform.GetChild(2).gameObject.name == "Ad Panel")
            transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
        else
            Debug.LogError("Ad Panel's child position was changed. Please revert your changes.");
    }
}

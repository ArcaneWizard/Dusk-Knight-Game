using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public Text weaponT;
    public Text weaponCostT;
    public Text DescriptionT;

    public Button purchaseButton;
    public Image bought;

    [HideInInspector]
    public int jewels = 600;
    private int price;

    private string[] Selection = {"HpBoost", "Bullets", "Grenade", "Flame", "Potion", "CB", "Arrow", "Tower"};

    private bool gameHasStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        //Reset Playerprefs every game
        /*for (int i = 0; i < Selection.Length; i++)
        {
            PlayerPrefs.SetInt(Selection[i], 0);
        }*/

        jewels = PlayerPrefs.GetInt("Jewels");

        gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        PlayerPrefs.SetInt("Jewels", jewels);

        if (GameObject.FindGameObjectWithTag("Jewels") != null)
        GameObject.FindGameObjectWithTag("Jewels").transform.GetComponent<Text>().text = jewels.ToString();

        for (int i = 0; i < Selection.Length; i++)
        {
            checkWeaponUserIsOn(Selection[i]);
        }

        //Change in-game menu buttons as you equip weapons
        for (int i = 0; i < Selection.Length; i++)
        {
            if (PlayerPrefs.GetInt(Selection[i]) == 1)
            {
                Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
                for (int j = 0; j < buttons.Length; j++)
                {
                    if (buttons[j].gameObject.name == Selection[i] + "!") 
                        buttons[j].gameObject.SetActive(true);
                }
            }
        }

        if (gameHasStarted == false && transform.GetChild(0).gameObject.activeSelf == true)
        {
            gameHasStarted = true;
            //Start equipped with Cannon
            CB();
            PlayerPrefs.SetInt("CB", 1);
        }
    }

    void checkWeaponUserIsOn(string key)
    {
        if (PlayerPrefs.GetString("Weapon") == key)
            updatePurchaseState(key);
    }

    void updatePurchaseState(string key)
    {
        if (PlayerPrefs.GetInt(key) == 0)
        {
            purchaseButton.gameObject.SetActive(true);
            bought.gameObject.SetActive(false);
        }

        else if (PlayerPrefs.GetInt(key) == 1 && key != "Tower" && key != "HpBoost")
        {
            purchaseButton.gameObject.SetActive(false);
            bought.gameObject.SetActive(true);
        }

        else if (key == "Tower")
            towerStates(key);

        else
            hpBoost(key);
    }

    void hpBoost(string key)
    {
        purchaseButton.gameObject.SetActive(true);
        bought.gameObject.SetActive(false);
    }

    void towerStates(string key)
    {
        if (PlayerPrefs.GetInt(key) >= 1 && PlayerPrefs.GetInt(key) <= 2)
        {
            purchaseButton.gameObject.SetActive(true);
            bought.gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt(key) == 3)
        {
            purchaseButton.gameObject.SetActive(false);
            bought.gameObject.SetActive(true);
        }

        //Fort 2 code located below
        if (PlayerPrefs.GetInt("Tower") == 1)
            chooseWeapon("Fort 3", 700, "Strengthen your fort with some heavy, metal beams. 3x stronger than Fort 1!");
        if (PlayerPrefs.GetInt("Tower") >= 2)
            chooseWeapon("Fort 4", 1150, "Strengthen your fort with a mystical protection charm. 4x stronger than Fort 1.");
    }

    public void purchase()
    {
        string key = PlayerPrefs.GetString("Weapon");

        if (jewels >= price)
        {
            jewels -= price;
            PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);

            Debug.Log("Purchase successful");
            Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
            transform.GetComponent<AudioSource>().PlayOneShot(m.purchase);
        }

        else
        {
            Debug.Log("Error. Not enough jewels. <Add error sound>");

            Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
            transform.GetComponent<AudioSource>().PlayOneShot(m.errorPurchase);
        }


    }

    void chooseWeapon(string weapon, int weaponCost, string Description)
    {
        weaponT.text = weapon;
        weaponCostT.text = weaponCost.ToString();
        DescriptionT.text = Description;

        price = weaponCost;        
    }

    private void clearButtonColor()
    {
        GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(0).transform.GetComponent<Image>().color = new Color32(143, 188, 195, 255);
        GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(1).transform.GetComponent<Image>().color = new Color32(143, 188, 195, 255);

        for (int i = 2; i < GameObject.FindGameObjectWithTag("Buttons").transform.childCount; i++)
        {
            GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(i).transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.buttonClick);
    }

    public void openShop()
    {
        openSomething(0);
    }

    public void closeShop()
    {
        closeSomething(0);
    }

    public void openSettings()
    {
        openSomething(1);
    }

    public void closeSettings()
    {
        closeSomething(1);
    }

    public void closeSomething(int child)
    {
        Time.timeScale = 1f;
        transform.GetChild(child).gameObject.SetActive(false);
        for (int i = 0; i <= 3; i++)
            transform.parent.transform.GetChild(i).gameObject.SetActive(true);

        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.buttonClick);
    }
    public void openSomething(int child)
    {
        Time.timeScale = 0f;
        transform.GetChild(child).gameObject.SetActive(true);
        for (int i = 0; i <= 3; i++)
            transform.parent.transform.GetChild(i).gameObject.SetActive(false);

        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.buttonClick);
    }

    public void CB()
    {
        clearButtonColor();
        chooseWeapon("Cannon", 200, "A heavy cannon ball isn't the most pleasant thing to be hit by.");
        GameObject.FindGameObjectWithTag("Respawn").transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "CB");
    }

    public void Arrow()
    {
        clearButtonColor();
        chooseWeapon("Ballista", 500, "A frosty arrow that slices through skin and freezes enemies to the bone. Brrrrrrr");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Arrow");
    }
    public void Potion()
    {
        clearButtonColor();
        chooseWeapon("Witch", 700, "A swirling green potion that intoxicates enemies and eats them from the inside.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Potion");
    }
    public void Flame()
    {
        clearButtonColor();
        chooseWeapon("Flamethrower", 1050, "A fiery flame that burns everything in sight. However, it only extends so far.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Flame");
    }

    public void Grenade()
    {
        clearButtonColor();
        chooseWeapon("Boomer", 1500, "A grenade with splash damage. Catch whole hordes of enemies in a deadly blast.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Grenade");
    }

    public void Bullets()
    {
        clearButtonColor();
        chooseWeapon("Turret", 650, "Ever wanted to drill enemies with streams of bullets from a gatling gun? This is your chance.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Bullets");
    }

    public void HpBoost()
    {
        clearButtonColor();
        chooseWeapon("Health Boost", 400, "Regain 50% of your tower's health through a mystical, life-giving heart.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "HpBoost");
        GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Health>().hpBoost = true;
    }
    public void Tower()
    {        
        clearButtonColor();
        chooseWeapon("Fort 2", 450, "Strengthen your fort with more wood. 2x stronger than Fort 1.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Tower");
    }
}

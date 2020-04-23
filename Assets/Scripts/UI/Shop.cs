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

    private string weapon;
    private string weaponCost;
    private string Description;

    private string[] Selection = {"HpBoost", "Bullets", "Grenade", "Flame", "Potion", "CB", "Arrow", "Tower" };

    // Start is called before the first frame update
    void Start()
    {  
        //Reset Playerprefs every game
        for (int i = 0; i < Selection.Length; i++)
        {
            PlayerPrefs.SetInt(Selection[i], 0);
        }

        //Start equipped with Cannon
        CB();
        PlayerPrefs.SetInt("CB", 1);
    }

    void Update()
    {
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

        else if (PlayerPrefs.GetInt(key) == 1 && key != "Tower")
        {
            purchaseButton.gameObject.SetActive(false);
            bought.gameObject.SetActive(true);
        }

        else
        towerStates(key);
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
            chooseWeapon("Fort 3", 300, "Build up your tower and strengthen its wall against invading hordes. Triple the resistance!");
        if (PlayerPrefs.GetInt("Tower") >= 2)
            chooseWeapon("Fort 4", 400, "Build up your tower and strengthen its wall against invading hordes. Quadruple the armor!");
    }

    public void purchase()
    {
        string key = PlayerPrefs.GetString("Weapon");
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
    }

    void chooseWeapon(string weapon, int weaponCost, string Description)
    {
        weaponT.text = weapon;
        weaponCostT.text = weaponCost.ToString();
        DescriptionT.text = Description;
    }

    private void clearButtonColor()
    {
        GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(0).transform.GetComponent<Image>().color = new Color32(143, 188, 195, 255);
        GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(1).transform.GetComponent<Image>().color = new Color32(143, 188, 195, 255);

        for (int i = 2; i < GameObject.FindGameObjectWithTag("Buttons").transform.childCount; i++)
        {
            GameObject.FindGameObjectWithTag("Buttons").transform.GetChild(i).transform.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }
    }

    public void CB()
    {
        clearButtonColor();
        chooseWeapon("Cannon", 100, "A heavy cannon ball isn't the most pleasant thing to be hit by.");
        GameObject.FindGameObjectWithTag("Respawn").transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "CB");
    }

    public void Arrow()
    {
        clearButtonColor();
        chooseWeapon("Ballista", 100, "A frosty arrow that slices through skin and freezes enemies to the bone.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Arrow");
    }
    public void Potion()
    {
        clearButtonColor();
        chooseWeapon("Witch", 100, "A swirling green potion that intoxicates enemies and slowly kills them from the inside.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Potion");
    }
    public void Flame()
    {
        clearButtonColor();
        chooseWeapon("Flamethrower", 100, "A fiery flame that burns everything in sight. However, it can only extend so far.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Flame");
    }

    public void Grenade()
    {
        clearButtonColor();
        chooseWeapon("Boomer", 100, "Shoots out grenades that do splash damage. No one likes getting caught in its deadly blast.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Grenade");
    }

    public void Bullets()
    {
        clearButtonColor();
        chooseWeapon("Turret", 100, "Ever wanted to drill enemies with streams of bullets from a gatling gun? This is your chance.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Bullets");
    }

    public void HpBoost()
    {
        clearButtonColor();
        chooseWeapon("Health Boost", 100, "Regain 50% of your tower's health through a mystical, life-giving heart.");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "HpBoost");
    }
    public void Tower()
    {        
        clearButtonColor();
        chooseWeapon("Fort 2", 200, "Build up your tower and strengthen its wall against invading hordes. Twice as strong!");
        EventSystem.current.currentSelectedGameObject.transform.GetComponent<Image>().color = new Color32(101, 186, 233, 255);
        PlayerPrefs.SetString("Weapon", "Tower");
    }
}

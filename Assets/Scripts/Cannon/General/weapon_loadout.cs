using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weapon_loadout : MonoBehaviour
{
    public Transform Weapons;

    [Space(10)]
    [Header("The 3 Weapons")]   
    private List<int> weapons = new List<int>{0, 1, 2};
    private int currentWeapon = 0;
    
    [Space(10)]
    [Header("Visuals + Ammo")]    
    public List<float> ammo;
    private float[] ammoCopy;
    private List<Text> ammoText = new List<Text>();
    private List<Image> weaponImage = new List<Image>();
    
    [Space(10)]
    [Header("Sprites")]    
    public Sprite weaponSelected;
    public Sprite weaponNotSelected;
    private shooting shooting;

    void Awake() 
    {
        int position = 0;
        int offset = 0;

        //Based off your weapons loadout, activate the specified weapon tokens in the specified order
        foreach (int weapon in weapons) {
            Weapons.transform.GetChild(weapon+offset).SetSiblingIndex(position);
            Weapons.transform.GetChild(position).gameObject.SetActive(true);

            if (weapon != position)
                ++offset;
                
            ++position;
        }

        //get all the active weapon icons and store them in a list
        foreach (RectTransform icon in Weapons) {
            if (icon.gameObject.activeSelf)
                weaponImage.Add(icon.GetComponent<Image>());
        }

        //get the text that displays the ammo of each weapon icon and store all of them in a list
        foreach (Image i in weaponImage) 
            ammoText.Add(i.gameObject.transform.GetChild(0).transform.GetComponent<Text>());

        //keep a copy of the max ammo counts of each weapon
        ammoCopy = ammo.ToArray();
    }

    void Start() 
    {
        //defining components
        shooting = transform.GetComponent<shooting>();

        //equip the right weapon
        selectWeapon();
        updateAmmo();
    }

    //every shot, update ammo + swap weapons when out of ammo
    public void shotTaken() {

        //use 1 ammo and update the ammo text
        if (ammo[currentWeapon] > 0) {
            ammo[currentWeapon] -= 1;
            updateAmmo();
        }

        //swap to the next weapon when out of ammo + start reloading this weapon for the future
        if (ammo[currentWeapon] <= 0) {
            StartCoroutine(reload(currentWeapon));
            currentWeapon = ++currentWeapon % weapons.Count;
            selectWeapon();
        }        
    }

    //select a new weapon 
    void selectWeapon() {
        
        //change the bullets that you are shooting
        shooting.weaponType = shooting.bullets[weapons[currentWeapon]];
        shooting.equipWeapon();

        //update the weapon icons
        foreach (Image i in weaponImage)
            i.sprite = weaponNotSelected;

        weaponImage[currentWeapon].sprite = weaponSelected;
    }

    //select a certain weapon manually
    public void selectWeaponManually() 
    {
        int cW = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex();
        if (ammo[cW] > 0) {
            currentWeapon = cW;
            selectWeapon();
        }
    }

    //update the ammo text to show the weapons' real ammo
    void updateAmmo() {
        for (int i = 0; i < ammoText.Count; i++) 
            ammoText[i].text = ammo[i].ToString();        
    }

    //reload a weapon
    private IEnumerator reload(int weapon)
     {
        //Show a visual flashing zero to indicate that a weapon is reloading
        for (int i = 1; i <= 12; i++) {
            yield return new WaitForSeconds(0.5f);
            ammoText[weapon].enabled = ammoText[weapon].IsActive() ?  false : true;
        }

        //refill the weapon's ammo stock
        ammo[weapon] = ammoCopy[weapon];
        updateAmmo();
    }
}

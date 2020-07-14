using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class weapon_loadout : MonoBehaviour
{
    [Space(10)]
    [Header("Visuals + Ammo")]    
    public List<float> ammo;
    public List<Text> ammoText;
    public List<Image> weaponImage;
    
    [Space(10)]
    [Header("Sprites")]    
    public Sprite weaponSelected;
    public Sprite weaponNotSelected;
    private shooting shooting;
    
    [Space(10)]
    [Header("The 3 Weapons")]   
    public List<int> weapons;
    private int currentWeapon = 0;

    void Start() {
        shooting = transform.GetComponent<shooting>();

        selectWeapon();
        updateAmmo();
    }

    //every shot, update ammo + swap weapons when out of ammo
    public void shotTaken() {

        //use 1 ammo and update the ammo text
        ammo[currentWeapon] -= 1;
        updateAmmo();

        //swap to the next weapon when out of ammo + start reloading this weapon for the future
        if (ammo[currentWeapon] <= 0) {
            StartCoroutine(reload(currentWeapon));
            currentWeapon = ++currentWeapon % 3;
            selectWeapon();
        }        
    }

    //select a new weapon 
    void selectWeapon() {
        
        //change the bullets that you are shooting
        shooting.weaponType = shooting.bullets[weapons[currentWeapon]];
        shooting.equipWeapon();

        //change the new weapon icon background to gold and the previous one back to blue
        weaponImage[0].sprite = weaponNotSelected;
        weaponImage[1].sprite = weaponNotSelected;
        weaponImage[2].sprite = weaponNotSelected;
        weaponImage[currentWeapon].sprite = weaponSelected;
    }

    //update the ammo text to show the weapons' real ammo
    void updateAmmo() {
        ammoText[0].text = ammo[0].ToString();
        ammoText[1].text = ammo[1].ToString();
        ammoText[2].text = ammo[2].ToString();
    }

    private IEnumerator reload(int weapon) {
        for (int i = 1; i <= 12; i++) {
            yield return new WaitForSeconds(0.5f);
            ammoText[weapon].enabled = ammoText[weapon].IsActive() ?  false : true;
        }

        ammo[weapon] = 25;
        updateAmmo();
    }
}

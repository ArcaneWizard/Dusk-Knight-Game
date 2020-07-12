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
    private List<float> ammoStore;
    
    [Space(10)]
    [Header("Sprites")]    
    public Sprite weaponSelected;
    public Sprite weaponNotSelected;
    private shooting shooting;
    
    [Space(10)]
    [Header("The 3 Weapons")]   
    public List<int> weapons;
    public int currentWeapon = 0;

    void Start() {
        shooting = transform.GetComponent<shooting>();

        selectWeapon(currentWeapon);
        updateAmmo();
    }

    public void shotTaken() {
        ammo[currentWeapon] -= 1;
        updateAmmo();

        if (ammo[currentWeapon] <= 0) {
            StartCoroutine(reload(currentWeapon));
            currentWeapon = ++currentWeapon % 3;
            selectWeapon(currentWeapon);
        }        
    }

    void selectWeapon(int weapon) {
        shooting.weaponType = shooting.bullets[weapons[currentWeapon]];
        shooting.equipWeapon();

        weaponImage[0].sprite = weaponNotSelected;
        weaponImage[1].sprite = weaponNotSelected;
        weaponImage[2].sprite = weaponNotSelected;
        weaponImage[weapon].sprite = weaponSelected;
    }

    void updateAmmo() {
        ammoText[0].text = ammo[0].ToString();
        ammoText[1].text = ammo[1].ToString();
        ammoText[2].text = ammo[2].ToString();
    }

    private IEnumerator reload(int weapon) {
        yield return new WaitForSeconds(3f);
        ammo[weapon] = 25;
    }
}

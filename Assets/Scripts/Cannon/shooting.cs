using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{

    public List<GameObject> ammo;
    public Transform muzzle;
    private float cooldown;
    private float startcooldown=2.0f;
    private int counter = 0;

    GameObject weaponType;

    void Start()
    {
        changeWeapon();
    }

    void changeWeapon()
    {        
        //Only need to change this one line for diff weapons (depending on weapon selected when that's implemented)
        weaponType = GameObject.Find("Grenade");

        for (int i = 0; i < weaponType.transform.childCount; i++)
            ammo.Add(weaponType.transform.GetChild(i).gameObject);

        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position) - transform.position;
            float rot = Mathf.Atan2(touchPosition.y, touchPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot);
            if (cooldown <= 0)
            {
                cooldown = startcooldown;
                Fire(rot);
            }
            else
            {
                cooldown -= Time.deltaTime;
            }
            
        }
        cooldown -= Time.deltaTime;
    }

    void Fire(float rot)
    {
        //Modify the position/rotation code based on weapon type
        if (weaponType == GameObject.Find("Grenade"))
        {
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
        }

        ammo[counter].SetActive(true);
        counter += 1;
        counter %= (weaponType.transform.childCount - 1);
    }
}

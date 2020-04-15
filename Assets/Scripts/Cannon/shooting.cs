using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class shooting : MonoBehaviour
{

    public List<GameObject> ammo;
    public Transform muzzle;
    private float cooldown;
    private float startcooldown;
    private int counter = 0;
    public static float touchPercent;

    GameObject weaponType;

    void Start()
    {
        changeWeapon();
    }

    public void changeWeapon()
    {
        //Only need to change this one line for diff weapons (depending on weapon selected when that's implemented)
        if (transform.gameObject.name == "Launcher")
        {
            weaponType = GameObject.Find("Grenade");
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Gatling")
        {
            weaponType = GameObject.Find("Bullet");
            startcooldown = 0.3f;
        }

        if (transform.gameObject.name == "Gunpowder Cannon")
        {
            weaponType = GameObject.FindGameObjectWithTag("CB");
            startcooldown = 0.8f;
        }
        
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
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position) - transform.position;
            float rot = Mathf.Atan2(touchPosition.y, touchPosition.x) * Mathf.Rad2Deg;
            if(weaponType == GameObject.Find("Grenade"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot);
            if (weaponType == GameObject.FindGameObjectWithTag("CB"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot);
            if (weaponType == GameObject.Find("Bullet"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot-90);

            Vector2 vectorFromTouch = touch.position - new Vector2(Screen.width/2f, Screen.height/2f);            
            touchPercent = (vectorFromTouch/new Vector2(Screen.width, Screen.height)).magnitude;

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

        if (weaponType == GameObject.Find("Bullet"))
        {
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<bullet>().oneHit = false;
            StartCoroutine(Flash());
        }
        if (weaponType == GameObject.Find("Cannon Ball"))
        {
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<cannon_ball>().oneHit = false;
            ammo[counter].transform.GetComponent<cannon_ball>().oneLaunch = false;
        }


        ammo[counter].SetActive(true);
        counter += 1;
        counter %= (weaponType.transform.childCount);
    }

    private IEnumerator Flash()
    {
        float r = UnityEngine.Random.Range(0.08f, 0.15f);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(r);
        transform.GetChild(2).gameObject.SetActive(false);

    }
}

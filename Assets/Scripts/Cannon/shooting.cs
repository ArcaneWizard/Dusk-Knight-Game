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
    public bool loaded = false;
    public bool playsound = false;

    GameObject weaponType;

    void Start()
    {
        changeWeapon();
        gameObject.AddComponent<AudioSource>();
    }

    public void changeWeapon()
    {
        //Only need to change this one line for diff weapons (depending on weapon selected when that's implemented)
        if (transform.gameObject.name == "Launcher")
        {
            weaponType = GameObject.Find("Grenade");
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Potion Cannon")
        {
            weaponType = GameObject.FindGameObjectWithTag("Potion");
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Gatling")
        {
            weaponType = GameObject.Find("Bullet");
            startcooldown = 0.25f;
        }

        if (transform.gameObject.name == "Gunpowder Cannon")
        {
            weaponType = GameObject.FindGameObjectWithTag("CB");
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Ice Arrow Cannon")
        {
            weaponType = GameObject.FindGameObjectWithTag("Arrow");
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


            Vector2 vectorFromTouch = touch.position - new Vector2(Screen.width/2f, Screen.height/2f);            
            touchPercent = (vectorFromTouch/new Vector2(Screen.width, Screen.height)).magnitude;

            if (cooldown <= 0)
            {
                cooldown = startcooldown;
                StartCoroutine(checkForWeaponChangeOrFire(rot, touchPosition));
            }
            else
            {
                cooldown -= Time.deltaTime;
            }
            
        }
        cooldown -= Time.deltaTime;
            
        if (weaponType == GameObject.FindGameObjectWithTag("Arrow") && loaded == false)
        {
            ammo[counter].gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = false;
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            ammo[counter].transform.GetComponent<PolygonCollider2D>().enabled = false;
            loaded = true;
            StartCoroutine(delayAppearance());
        }
    }

    private IEnumerator delayAppearance()
    {
        yield return new WaitForSeconds(0.01f);
        int flip = 0;
        Transform arrowHead = GameObject.Find("Head").transform.GetChild(4).transform;
        if (arrowHead.rotation.y != 0)
            flip = -1;
        else
            flip = 1;
        ammo[counter].transform.rotation = Quaternion.Euler(0f, (flip - 1) * 90, arrowHead.rotation.eulerAngles.z - 90);
        ammo[counter].transform.position = arrowHead.GetChild(0).position;
        ammo[counter].SetActive(true);
    }

    private IEnumerator checkForWeaponChangeOrFire(float rot, Vector3 touchPosition)
    {
        yield return new WaitForSeconds(0.01f);
        if (GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Select_Weapon>().weaponChange == false)
        {
            if (weaponType == GameObject.Find("Grenade"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot);

            if (weaponType == GameObject.FindGameObjectWithTag("CB"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot);

            if (weaponType == GameObject.Find("Bullet"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot - 90);

            if (weaponType == GameObject.FindGameObjectWithTag("Potion"))
                transform.rotation = Quaternion.Euler(0f, 0f, rot+180);

            if (weaponType == GameObject.FindGameObjectWithTag("Arrow"))
                transform.rotation = Quaternion.Euler(0f, (touchPosition.x / Mathf.Abs(touchPosition.x) - 1) * 90, (touchPosition.x / Mathf.Abs(touchPosition.x)) * rot + 90 * (touchPosition.x / Mathf.Abs(touchPosition.x) - 1));

            //if (transform.gameObject.name == "Flamethrower")
              //  transform.rotation = Quaternion.Euler(0f, 0f, rot);

            Vector3 eulerAngle = transform.localRotation.eulerAngles;
            float angle = eulerAngle.z % 360;
            if (angle < 0)
                angle += 360;

            if (angle > 90 && angle < 270 && transform.gameObject.name != "Gatling")
               transform.localRotation = Quaternion.Euler(eulerAngle.x, eulerAngle.y - 180f, -eulerAngle.z - 180f);

            Fire(rot);
        }
    }

    void Fire(float rot)
    {
        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        

        //Modify the position/rotation code based on weapon type
        if (weaponType == GameObject.Find("Grenade"))
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.cannonShot, 0.3f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<grenade>().oneLaunch = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == GameObject.Find("Bullet"))
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.gatling, 0.3f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<bullet>().oneHit = false;
            StartCoroutine(Flash());
        }

        if (weaponType == GameObject.FindGameObjectWithTag("CB"))
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.cannonShot, 0.4f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<cannon_ball>().oneHit = false;
            ammo[counter].transform.GetComponent<cannon_ball>().oneLaunch = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == GameObject.FindGameObjectWithTag("Potion"))
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.potionshot, 0.3f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<potion>().oneLaunch = false;
            ammo[counter].transform.GetComponent<potion>().oneHit = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == GameObject.FindGameObjectWithTag("Arrow"))
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.arrowshot, 0.22f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot-90);
            ammo[counter].transform.GetComponent<arrow>().oneLaunch = false;
            ammo[counter].transform.GetComponent<arrow>().oneHit = false;
            ammo[counter].transform.GetComponent<PolygonCollider2D>().enabled = true;
            ammo[counter].transform.GetComponent<Rigidbody2D>().gravityScale = 2;
            StartCoroutine(Load());
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

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0.2f);
        loaded = false;
    }

}

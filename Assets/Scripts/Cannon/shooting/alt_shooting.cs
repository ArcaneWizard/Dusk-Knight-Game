﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class alt_shooting : MonoBehaviour
{

    public List<GameObject> ammo;
    public Transform muzzle;
    public Camera camera;
    public GameObject Aim_Arrow;
    public GameObject Charge_Arrow;
    private LineRenderer lr;
    private LineRenderer lr_2;
    public float arrowInitLength = 28f;
    public float maxArrowLength = 60f;
    public float arrowGrowRate = 2f;
    public float aimArrWidth = 1.2f;
    public float chargeArrWidth = 1.2f;
    public float arrowWidthGrowRate = 0.1f;
    private float chargeArrowLength;
    public float minSwipeToShoot = 0.1f;

    private float cooldown;
    private float startcooldown;
    private int counter = 0;
    public static float touchPercent;
    public bool loaded = false;
    public bool playsound = false;
    private Vector3 initPosition;
    private Vector3 endPosition;
    private Vector3 initPosition2;
    private Vector3 endPosition2;

    GameObject weaponType;

    public GameObject grenade;
    public GameObject potion;
    public GameObject cannonBall;
    public GameObject bullet;
    public GameObject arrow;

    public Select_Weapon select_Weapon;
    public Transform ballista;
    public Manage_Sounds manage_Sounds;

    void Start()
    {
        changeWeapon();
        gameObject.AddComponent<AudioSource>();
    }

    //swap out the weapon you have with a different one
    public void changeWeapon()
    {
        //Only need to change this one line for diff weapons (depending on weapon selected when that's implemented)
        if (transform.gameObject.name == "Launcher")
        {
            weaponType = grenade;
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Potion Cannon")
        {
            weaponType = potion;
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Gatling")
        {
            weaponType = bullet;
            startcooldown = 0.25f;
        }

        if (transform.gameObject.name == "Gunpowder Cannon")
        {
            weaponType = cannonBall;
            startcooldown = 0.8f;
        }

        if (transform.gameObject.name == "Ice Arrow Cannon")
        {
            weaponType = arrow;
            startcooldown = 0.8f;

        }

        for (int i = 0; i < weaponType.transform.childCount; i++)
            ammo.Add(weaponType.transform.GetChild(i).gameObject);

        cooldown = 0;
    }

    //-------------------------------------------------------------------------------
    //------------------------FINGER DRAG + HOLD CONTROLS----------------------------
    //-------------------------------------------------------------------------------

    void Update()
    {
        float rotation, magnitude;

        //When one finger is on the screen
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began) {
                //setup aim arrow 
                if (lr == null) 
                    lr = Aim_Arrow.GetComponent<LineRenderer>();                

                lr.enabled = true;
                lr.widthMultiplier = aimArrWidth;
                lr.positionCount = 2;                

                //set anchor point of aim arrow
                initPosition = new Vector3(touch.position.x ,touch.position.y, 6);
                lr.SetPosition(0, camera.ScreenToWorldPoint(initPosition));
                lr.SetPosition(1, camera.ScreenToWorldPoint(initPosition));
           
                //setup charge arrow
                if (lr_2 == null) 
                    lr_2 = Charge_Arrow.GetComponent<LineRenderer>();     

                lr_2.enabled = true;
                lr_2.widthMultiplier = chargeArrWidth;
                lr_2.positionCount = 2;                
                
                //set anchor point of charge arrow
                lr_2.SetPosition(0, camera.ScreenToWorldPoint(initPosition));
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(initPosition));
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                //Change endpoint of the aim arrow vector to where you move your finger
                endPosition = new Vector3(touch.position.x, touch.position.y, 6);
                if ((endPosition - initPosition).magnitude > maxArrowLength)
                    endPosition = initPosition + (endPosition-initPosition).normalized * maxArrowLength;
        
                float aimArrowLength = (endPosition-initPosition).magnitude;
                lr.SetPosition(1, camera.ScreenToWorldPoint(endPosition));
                
                //Set the length of the charge arrow
                endPosition2 = new Vector3(touch.position.x, touch.position.y, 6);
                if (chargeArrowLength > maxArrowLength)
                    chargeArrowLength = maxArrowLength;
                if (lr_2.widthMultiplier > aimArrWidth)
                    lr_2.widthMultiplier = aimArrWidth;

                endPosition2 = initPosition + (endPosition2-initPosition).normalized * chargeArrowLength;
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(endPosition2));
                
                //increase the charge arrow's length over time when you're fully stretched out
                if (aimArrowLength >= maxArrowLength-2 && chargeArrowLength > 0) {
                    chargeArrowLength += Time.deltaTime * arrowGrowRate;
                    lr_2.widthMultiplier += Time.deltaTime * arrowWidthGrowRate;
                }

                //set an initial charge arrow length when you start charging
                else if (aimArrowLength >= maxArrowLength-2 && chargeArrowLength == 0) {
                    chargeArrowLength = arrowInitLength;
                    lr_2.widthMultiplier = chargeArrWidth;
                }

                //set the charge arrow length to 0 when not fully stretched out
                else 
                    chargeArrowLength = 0;

                //Rotate gun while aiming
                float rot = Mathf.Atan2(initPosition.y - endPosition.y, initPosition.x - endPosition.x) * Mathf.Rad2Deg;
                if ((endPosition - initPosition).magnitude > minSwipeToShoot)
                   changeRotation(rot, endPosition);
            }

            if (touch.phase == TouchPhase.Ended) {
                //arrows disappear when you lift your finger
                lr.enabled = false;
                lr_2.enabled = false;

                //set important values that your cannon requires
                rotation = Mathf.Atan2(initPosition.y - endPosition.y, initPosition.x - endPosition.x) * Mathf.Rad2Deg;
                magnitude = (endPosition - initPosition).magnitude;
                touchPercent = magnitude / Screen.width;

                //if the player every dragged out the arrow, fire
                if (magnitude > minSwipeToShoot)
                   StartCoroutine(checkForWeaponChangeOrFire(rotation, endPosition));
            }
        }                
        
        /*//____LAYERED CHARGE ARROW____

        Vector3 initPosition2 = new Vector3(0, 0, 0);
        Vector3 endPosition2 = new Vector3(0, 0, 0);  

        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            
            if (touch2.phase == TouchPhase.Began) {
                //Enable the line renderer
                if (lr_2 == null) 
                    lr_2 = Charge_Arrow.GetComponent<LineRenderer>();                

                lr_2.enabled = true;
                lr_2.widthMultiplier = 1.2f;
                lr_2.positionCount = 2;                

                initPosition2 = new Vector3(touch.position.x, touch.position.y, 6);
                lr_2.SetPosition(0, camera.ScreenToWorldPoint(initPosition2));
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(initPosition2));

                //set its initial length to a fixed value
                arrowLength = arrowInitLength;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                //update the line's end point as the finger moves or remains still
                Vector3 touchPos = touch.position;
                endPosition2 = initPosition2 + (touchPos-initPosition2).normalized * arrowLength;
                endPosition2 = new Vector3(endPosition2.x, endPosition2.y, 6);
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(endPosition2));
                
                //increase the arrow's length over time
                arrowLength += Time.deltaTime * arrowGrowRate;
            }

            if (touch.phase == TouchPhase.Ended) {
                //make the line disappear upon release
                lr_2.enabled = false;
            }
        }
*/
        
        //____Ballista needs to load each arrow well before shooting
        if (weaponType == arrow && loaded == false)
        {
            ammo[counter].gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = false;
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            ammo[counter].transform.GetComponent<PolygonCollider2D>().enabled = false;
            loaded = true;
            StartCoroutine(delayAppearance());
        }
    }
    
    //-------------------------------------------------------------------------------
    //--------------------------ROTATE AND FIRE WEAPON-------------------------------
    //-------------------------------------------------------------------------------
   
    //Set weapon rotation and fire if button wasn't pressed
    private IEnumerator checkForWeaponChangeOrFire(float rot, Vector3 touchPosition)
    {
        yield return new WaitForSeconds(0.01f);
        if (select_Weapon.weaponChange == false)
        {
            changeRotation(rot, touchPosition);
            Fire(rot);
        }
    }

    //Set weapon rotation
    private void changeRotation(float rot, Vector3 touchPosition) {
        
            if (weaponType == grenade)
                transform.rotation = Quaternion.Euler(0f, 0f, rot);

            if (weaponType == cannonBall)
                transform.rotation = Quaternion.Euler(0f, 0f, rot);

            if (weaponType == bullet)
                transform.rotation = Quaternion.Euler(0f, 0f, rot - 90);

            if (weaponType == potion)
                transform.rotation = Quaternion.Euler(0f, 0f, rot+180);

            if (weaponType == arrow)
                transform.rotation = Quaternion.Euler(0f, (touchPosition.x / Mathf.Abs(touchPosition.x) - 1) * 90, (touchPosition.x / Mathf.Abs(touchPosition.x)) * rot + 90 * (touchPosition.x / Mathf.Abs(touchPosition.x) - 1));

            Vector3 eulerAngle = transform.localRotation.eulerAngles;
            float angle = eulerAngle.z % 360;
            if (angle < 0)
                angle += 360;

            if (angle > 90 && angle < 270 && transform.gameObject.name != "Gatling")
               transform.localRotation = Quaternion.Euler(eulerAngle.x, eulerAngle.y - 180f, -eulerAngle.z - 180f);
    }

    //Fire the weapon
    void Fire(float rot)
    {
        Manage_Sounds m = manage_Sounds;

        //Modify the position/rotation code based on weapon type
        if (weaponType == grenade)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.cannonShot, 0.8f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<grenade>().oneLaunch = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == bullet)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.gatling, 0.6f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<bullet>().oneHit = false;
            StartCoroutine(Flash());
        }

        if (weaponType == cannonBall)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.cannonShot, 0.8f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<cannon_ball>().oneLaunch = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == potion)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.potionshot, 0.9f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
            ammo[counter].transform.GetComponent<potion>().oneLaunch = false;
            ammo[counter].transform.GetComponent<potion>().oneHit = false;
            ammo[counter].transform.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < ammo[counter].transform.childCount; i++)
                ammo[counter].transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = false;
        }

        if (weaponType == arrow)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.arrowshot, 0.35f * Manage_Sounds.soundMultiplier);
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

    //-------------------------------------------------------------------------------
    //------------------------------SHORT SUB-METHODS--------------------------------
    //----------(Small methods that server to make code more readable)---------------
    //-------------------------------------------------------------------------------

    //Flash effect for the gatling gun sub-method
    private IEnumerator Flash()
    {
        float r = UnityEngine.Random.Range(0.08f, 0.15f);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(r);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    //Load ice arrow into the ballista sub-method
    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0.2f);
        loaded = false;
    }
    
    //Loading ice arrow in the ballista sub-method v2
    private IEnumerator delayAppearance()
    {
        yield return new WaitForSeconds(0.01f);
        int flip = 0;
        Transform arrowHead = ballista;
        if (arrowHead.rotation.y != 0)
            flip = -1;
        else
            flip = 1;
        ammo[counter].transform.rotation = Quaternion.Euler(0f, (flip - 1) * 90, arrowHead.rotation.eulerAngles.z - 90);
        ammo[counter].transform.position = arrowHead.GetChild(0).position;
        ammo[counter].SetActive(true);
    }
}

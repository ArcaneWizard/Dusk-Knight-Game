using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class shooting : MonoBehaviour
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
    public float zOffset = 0f;

    private int counter = 0;
    private float rot;
    public static float touchPercent;
    public bool loaded = false;
    public bool playsound = false;
    private Vector3 initPosition;
    private Vector3 endPosition;
    private Vector3 initPosition2;
    private Vector3 endPosition2;
    private Vector3 centeredOffset;
    private Vector3 anchorPoint;

    private GameObject weaponType;
    public GameObject grenade;
    public GameObject potion;
    public GameObject cannonBall;
    public GameObject bullet;
    public GameObject arrow;

    public Select_Weapon select_Weapon;
    public Transform ballista;
    public Manage_Sounds manage_Sounds;
    public GameObject weaponAnchor;
    public GameObject arrowAnchor;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        changeWeapon();
    }

    //specify what weapon the player has
    public void changeWeapon()
    {
        if (transform.gameObject.name == "Boomer Cannon")
            weaponType = grenade;

        if (transform.gameObject.name == "Potion Cannon")
            weaponType = potion;

        if (transform.gameObject.name == "Gunpowder Cannon")
            weaponType = cannonBall;

        if (transform.gameObject.name == "Ice Arrow Cannon")
            weaponType = arrow;

        for (int i = 0; i < weaponType.transform.childCount; i++)
            ammo.Add(weaponType.transform.GetChild(i).gameObject);
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
                centeredOffset = -camera.ScreenToWorldPoint(initPosition) + arrowAnchor.transform.position + new Vector3(0, 0, zOffset);
                
                lr.SetPosition(0, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
                lr.SetPosition(1, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
           
                //setup charge arrow
                if (lr_2 == null) 
                    lr_2 = Charge_Arrow.GetComponent<LineRenderer>();     

                lr_2.enabled = true;
                lr_2.widthMultiplier = chargeArrWidth;
                lr_2.positionCount = 2;                
                
                //set anchor point of charge arrow
                lr_2.SetPosition(0, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                //Change endpoint of the aim arrow vector to where you move your finger
                endPosition = new Vector3(touch.position.x, touch.position.y, 6);
                if ((endPosition - initPosition).magnitude > maxArrowLength)
                    endPosition = initPosition + (endPosition-initPosition).normalized * maxArrowLength;

                //Update start and end point
                centeredOffset = -camera.ScreenToWorldPoint(initPosition) + arrowAnchor.transform.position + new Vector3(0, 0, -4);
                Vector3 firstPoint = camera.ScreenToWorldPoint(initPosition) + centeredOffset;
                lr.SetPosition(1, firstPoint);

                float aimArrowLength = (endPosition-initPosition).magnitude;
                Vector3 secondPoint = camera.ScreenToWorldPoint(endPosition) + centeredOffset;
                lr.SetPosition(0, 2 * firstPoint - secondPoint);
                
                //Set the length of the charge arrow
                endPosition2 = new Vector3(touch.position.x, touch.position.y, 6);
                if (chargeArrowLength > maxArrowLength)
                    chargeArrowLength = maxArrowLength;
                if (lr_2.widthMultiplier > aimArrWidth)
                    lr_2.widthMultiplier = aimArrWidth;

                //update start and end point
                endPosition2 = initPosition + (endPosition2-initPosition).normalized * chargeArrowLength;
                Vector3 secondPoint2 = camera.ScreenToWorldPoint(endPosition2) + centeredOffset;
                lr_2.SetPosition(1, firstPoint);
                lr_2.SetPosition(0, 2 * firstPoint - secondPoint2);
                
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
                rot = Mathf.Atan2(initPosition.y - endPosition.y, initPosition.x - endPosition.x) * Mathf.Rad2Deg;
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
                
        /*//____Ballista needs to load each arrow well before shooting
        if (weaponType == arrow && loaded == false)
        {
            ammo[counter].gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = false;
            ammo[counter].transform.position = muzzle.position;
            ammo[counter].transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            ammo[counter].transform.GetComponent<PolygonCollider2D>().enabled = false;
            loaded = true;
            StartCoroutine(delayAppearance());
        }*/
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

        //rotation to flip bullet images over when shooting left vs right
        float rotY = (touchPosition.x / Mathf.Abs(touchPosition.x) - 1) * 90f;
        float rotZ = (touchPosition.x / Mathf.Abs(touchPosition.x)) * rot + 90 * (touchPosition.x / Mathf.Abs(touchPosition.x) - 1);
        
        //set weapon rotations
        if (weaponType == grenade)
            transform.rotation = Quaternion.Euler(0f, 0f, rot);

        if (weaponType == cannonBall)
            transform.rotation = Quaternion.Euler(0f, 0f, rot);

        if (weaponType == potion)
            transform.rotation = Quaternion.Euler(0f, 0f, rot + 180);

        if (weaponType == arrow)
            transform.rotation = Quaternion.Euler(0f, rotY, rotZ);

        //rotate the aim arrows around the cannon 
        weaponAnchor.transform.rotation = transform.rotation;
    }

    //Fire the weapon
    void Fire(float rot)
    {
        Manage_Sounds m = manage_Sounds;

        //Modify the position/rotation code based on weapon type
        if (weaponType == grenade)
            prepareBullet(rot + 270, m.cannonShot);

        if (weaponType == cannonBall)
            prepareBullet(rot + 270, m.cannonShot);

        /*if (weaponType == arrow)
        {
            transform.GetComponent<AudioSource>().PlayOneShot(m.arrowshot, 0.35f * Manage_Sounds.soundMultiplier);
            ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot-90);
            ammo[counter].transform.GetComponent<arrow>().oneLaunch = false;
            ammo[counter].transform.GetComponent<arrow>().oneHit = false;
            ammo[counter].transform.GetComponent<PolygonCollider2D>().enabled = true;
            ammo[counter].transform.GetComponent<Rigidbody2D>().gravityScale = 2;
            StartCoroutine(Load());
        }*/

        ammo[counter].SetActive(true);
        counter += 1;
        counter %= (weaponType.transform.childCount);
    }

    //-------------------------------------------------------------------------------
    //------------------------------SHORT SUB-METHODS--------------------------------
    //----------(Small methods that server to make code more readable)---------------
    //-------------------------------------------------------------------------------

    //Set bullet properties and location
    private void prepareBullet(float rot, AudioClip hit_Sound) {
        //set bullet's position and rotation
        ammo[counter].transform.position = muzzle.position;
        ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot);

        //play bullet fire sound
        transform.GetComponent<AudioSource>().PlayOneShot(hit_Sound, 0.8f * Manage_Sounds.soundMultiplier);
        
        //launch the bullet through its script's specifications
        if (weaponType == cannonBall)
            ammo[counter].GetComponent<cannon_ball>().oneLaunch = false;
        if (weaponType == grenade)
            ammo[counter].GetComponent<grenade>().oneLaunch = false;
        if (weaponType == potion)
            ammo[counter].GetComponent<potion>().oneLaunch = false;            
        if (weaponType == arrow)
            ammo[counter].GetComponent<arrow>().oneLaunch = false;
    }

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

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class shooting : MonoBehaviour
{
    [Space(10)]
    [Header("Arrows")]
    public GameObject Aim_Arrow;
    public GameObject Charge_Arrow;
    private LineRenderer lr;
    private LineRenderer lr_2;
    
    [Space(10)]
    [Header("Arrow Settings")]
    public float arrowInitLength = 95f;
    public float maxArrowLength = 200f;
    public float arrowGrowRate = 32f;
    public float aimArrWidth = 1.2f;
    public float chargeArrWidth = 0.6f;
    public float arrowWidthGrowRate = 0.14f;
    private float chargeArrowLength;
    public float minSwipeToShoot = 20f;
    public float zOffset = 0f;
    public float aimRotationExtreme = 75f;
    public float shotForce = 1.1f;

    private int counter = 0;
    private float rot;
    public static float touchPercent;
    private Vector3 initPosition;
    private Vector3 endPosition;
    private Vector3 initPosition2;
    private Vector3 endPosition2;
    private Vector3 centeredOffset;
    private Vector3 anchorPoint;

    [HideInInspector]
    public GameObject weaponType;
    private List<GameObject> ammo = new List<GameObject>();

    [Space(10)]
    [Header("Rage Mechanic")]
    public Image rage;
    public float max_hits = 93;
    private float initialRage = 0;
    public ParticleSystem slow_effect;
    public Transform pulse;
    public Sprite triple_cannon;
    public Sprite cannon;
    private int proj_num;
    private int shot_num = 1;

    public static float rage_count;
    public static float max_count;

    [Space(10)]
    [Header("Objects")]
    public Transform muzzle;
    public Camera camera;
    public GameObject weaponAnchor;
    public GameObject arrowAnchor;
    private weapon_loadout wL;
    private AudioSource audioSource;

    private Vector2 topLeft;
    private Vector2 topRight;
    public float rainOffset = 0.2f;

    [Space(10)]
    [Header("Player Bullets")]
    public GameObject playerWeapons;
    [HideInInspector] public List<GameObject> bullets = new List<GameObject>();

    [Space(10)]
    [Header("Player Character")]

    public GameObject player_Head;

    void Awake()
    {
        //define initial components 
        audioSource = transform.GetComponent<AudioSource>();
        lr = Aim_Arrow.GetComponent<LineRenderer>();  
        lr_2 = Charge_Arrow.GetComponent<LineRenderer>();   
        wL = transform.GetComponent<weapon_loadout>();

        //initial rage settings
        rage_count = initialRage;
        max_count = max_hits;
        
        //camera bounds to reference later
        topLeft = camera.ViewportToWorldPoint(new Vector2(0,1)) + new Vector3 (3, 2, 0);
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1)) + new Vector3 (3, 2, 0);

        //add all weapons' bullets to the bullet list
        foreach (Transform weapon in playerWeapons.transform) 
            bullets.Add(weapon.gameObject);
    }

    //specify what weapon the player has
    public void equipWeapon()
    {
        //empty the ammo array (whatever weapon's ammo is in it)
        ammo.Clear();

        //add bullets of the newly updated weapon type into the ammo Array
        for (int i = 0; i < weaponType.transform.childCount; i++)
            ammo.Add(weaponType.transform.GetChild(i).gameObject);

        //reset the ammo array index to start from the first bullet in the array
        counter = 0;
    }

    //called when rage button is pressed and activates stages based on the shots accumulated
    public void Rage()
    {
        //rage stage 1
        if(rage_count >= max_hits/3 && rage_count < 2f/3f * max_hits)
        {
            rage_count -= max_hits / 3;
            rainOffset = 0.001f;
            StartCoroutine(rainBullets(6));
            StartCoroutine(Slow());
        }

        //rage stage 2
        else if (rage_count >= 2f/3f * max_hits && rage_count < max_hits)
        {
            rage_count -= 2f/3f * max_hits;
            rainOffset = 0.13f;
            StartCoroutine(rainBullets(18));
            StartCoroutine(Slow());
        }

        //rage stage 3
        else if(rage_count >= max_hits)
        {
            rage_count -= max_hits;
            rainOffset = 0.13f;
            StartCoroutine(Slow());
            StartCoroutine(rainBullets(22));
            StartCoroutine(TripleShot(2));
        }
    }

    //-------------------------------------------------------------------------------
    //------------------------FINGER DRAG + HOLD CONTROLS----------------------------
    //-------------------------------------------------------------------------------

    void Update()
    {
        //fill rage bar
        rage.fillAmount = rage_count / max_hits;

        //dont let the rage meter go beyond or below its bounds
        if (rage_count > max_hits)
            rage_count = max_hits;
        if (rage_count < 0)
            rage_count = 0;

        float rotation, magnitude;

        //When one finger is on the screen
        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began) {

                //setup aim arrow
                lr.enabled = true;
                lr.widthMultiplier = aimArrWidth;
                lr.positionCount = 2;                

                //set anchor point of aim arrow
                initPosition = new Vector3(touch.position.x ,touch.position.y, 6);
                centeredOffset = -camera.ScreenToWorldPoint(initPosition) + arrowAnchor.transform.position + new Vector3(0, 0, zOffset);
                
                lr.SetPosition(0, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
                lr.SetPosition(1, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
           
                //setup charge arrow
                lr_2.enabled = true;
                lr_2.widthMultiplier = chargeArrWidth;
                lr_2.positionCount = 2;                
                
                //set anchor point of charge arrow
                lr_2.SetPosition(0, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
                lr_2.SetPosition(1, camera.ScreenToWorldPoint(initPosition) + centeredOffset);
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {

                //Get the point where you touched the screen 
                endPosition = new Vector3(touch.position.x, touch.position.y, 6);

                //Get the rotation and magnitude of the vector between the touch point and cannon pivot
                rot = Mathf.Atan2(initPosition.y - endPosition.y, initPosition.x - endPosition.x) * Mathf.Rad2Deg;
                float aimArrowLength = (endPosition-initPosition).magnitude;

                //Update the endpoint to never face backwards
                if (rot < -aimRotationExtreme) 
                    endPosition = initPosition + Quaternion.Euler(0, 0, -aimRotationExtreme) * Vector3.left * aimArrowLength;
                else if (rot > aimRotationExtreme)
                    endPosition = initPosition + Quaternion.Euler(0, 0, aimRotationExtreme) * Vector3.left *  aimArrowLength; 

                //Update the endpoint of the aim arrow vector to stay a constant distance from the cannon's pivot as it moves with ur finger
                if ((endPosition - initPosition).magnitude > maxArrowLength)
                    endPosition = initPosition + (endPosition-initPosition).normalized * maxArrowLength;

                //There will be no arrow if the player hasn't swiped the minimum distance
                if ((endPosition - initPosition).magnitude < minSwipeToShoot)
                    endPosition = initPosition;

                //Update the start and end point
                centeredOffset = -camera.ScreenToWorldPoint(initPosition) + arrowAnchor.transform.position + new Vector3(0, 0, -4);
                Vector3 firstPoint = camera.ScreenToWorldPoint(initPosition) + centeredOffset;
                lr.SetPosition(1, firstPoint);

                Vector3 secondPoint = camera.ScreenToWorldPoint(endPosition) + centeredOffset;
                lr.SetPosition(0, 2 * firstPoint - secondPoint);
                
                //Set the length of the charge arrow
                endPosition2 = new Vector3(touch.position.x, touch.position.y, 6);
                if (chargeArrowLength > maxArrowLength)
                    chargeArrowLength = maxArrowLength;
                if (lr_2.widthMultiplier > aimArrWidth)
                    lr_2.widthMultiplier = aimArrWidth;

                //Update the endpoint to never face backwards
                endPosition2 = initPosition + (endPosition2-initPosition).normalized * chargeArrowLength;
                
                if (rot < -aimRotationExtreme) {
                    endPosition2 = initPosition + Quaternion.Euler(0, 0, -aimRotationExtreme) * Vector3.left * chargeArrowLength;
                    rot = -aimRotationExtreme;
                }
                else if (rot > aimRotationExtreme) {
                    endPosition2 = initPosition + Quaternion.Euler(0, 0, aimRotationExtreme) * Vector3.left * chargeArrowLength;
                    rot = aimRotationExtreme;
                }
                
                //update start and end point
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

                //Rotate gun while aiming (but not when aiming backwards)
                if ((endPosition - initPosition).magnitude > minSwipeToShoot)
                   changeRotation(rot, endPosition);
            }

            if (touch.phase == TouchPhase.Ended) {
                //arrows disappear when you lift your finger
                lr.enabled = false;
                lr_2.enabled = false;

                //set important values that your cannon requires
                magnitude = (endPosition - initPosition).magnitude * shotForce;
                touchPercent = magnitude / Screen.width;

                //if the player every dragged out the arrow, fire
                if (magnitude > minSwipeToShoot) 
                   StartCoroutine(checkForWeaponChangeOrFire(rot, endPosition));
            }
        }
    }
    
    //-------------------------------------------------------------------------------
    //--------------------------ROTATE AND FIRE WEAPON-------------------------------
    //-------------------------------------------------------------------------------
   
    //Set weapon rotation and fire if button wasn't pressed
    private IEnumerator checkForWeaponChangeOrFire(float rot, Vector3 touchPosition)
    {
        yield return new WaitForSeconds(0.01f);

        changeRotation(rot, touchPosition);
        StartCoroutine(Fire(rot));
    }

    //Set cannon rotation
    private void changeRotation(float rot, Vector3 touchPosition) {

        //set cannon rotation and position
        transform.rotation = Quaternion.Euler(0f, 0f, rot);
        transform.localPosition = cannonPosition(rot);

        //set the player head's rotation and position 
        player_Head.transform.rotation = Quaternion.Euler(0f, 0f, rot / 5f);
        player_Head.transform.localPosition = headPosition(rot / 5f);

        //rotate the guide aim arrows around the cannon 
        weaponAnchor.transform.rotation = transform.rotation;
    }

    //Calculate and return the cannon's position for a given rotation
    private Vector2 cannonPosition(float rot)
     {
        float posX = 0.07f;
        float posY = 4.09f; 

        if (rot > 0) {
            posX += rot * (0.1f) / (75f);
            posY -= rot * (-0.11f) / (75f);
        }

        else if (rot < 0) {
            posX += rot * (0.17f) / (-75f);
            posY += rot * (0.1f) / (-75f);
        }

        return new Vector2(posX, posY);
    }

    //Calculate and return the player head's position when looking up and down
    private Vector2 headPosition(float rot)
    {
        float posX = 0.14f;
        float posY = 4.7f;

        if (rot > 0) {
            posX += rot * (0.042f) / 15f;
            posY += rot * (-0.011f) / 15f;
        }

        if (rot < 0) {
            posX += rot * (-0.009f) / -15f;
            posY += rot * (-0.048f) / -15f;
        }

        return new Vector2(posX, posY);
    }

    //Fire the cannon
    private IEnumerator Fire(float rot)
    {            
        //lose one ammo
        wL.shotTaken();
        
        for (int i = 0; i < shot_num; i++)
        {
            //set bullet's position and rotation
            ammo[counter].transform.position = muzzle.GetChild(i).position;
            ammo[counter].transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 90);
            ammo[counter].GetComponent<player_bullet>().oneLaunch = false;

            //play bullet fire sound
            audioSource.PlayOneShot(Manage_Sounds.Instance.cannonShot, 0.8f * Manage_Sounds.soundMultiplier);

            //active bullet and cycle to the next one in the future
            ammo[counter].SetActive(false);
            ammo[counter].SetActive(true);
            counter += 1;
            counter %= (weaponType.transform.childCount);

            yield return new WaitForSeconds(0.05f);
        }
    }

    //launch a pulse that slows enemies on contact
    private IEnumerator Slow()
    {
        /*slow_effect.Play();
        yield return new WaitForSeconds(0.75f);
        slow_effect.Stop();*/
        
        pulse.gameObject.SetActive(true);
        pulse.GetComponent<Rigidbody2D>().velocity = new Vector3(10, 0, 0);

        yield return new WaitUntil(() => pulse.position.x>18);

        pulse.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        pulse.gameObject.SetActive(false);
        pulse.SetPositionAndRotation(new Vector3(-1.39f, -0.04f, 0.0f), Quaternion.Euler(0,0,0));
    }

    private IEnumerator TripleShot(float r3)
    {
        transform.GetComponent<SpriteRenderer>().sprite = triple_cannon;
        shot_num = 3;
        yield return new WaitForSeconds(r3);
        shot_num = 1;
        transform.GetComponent<SpriteRenderer>().sprite = cannon;
    }
    
    public IEnumerator rainBullets(int bulletsToRain) {

        float distance = topRight.x - topLeft.x;

        for (int i = 0; i < bulletsToRain; i++) 
        {
            //set bullet's position and rotation
            ammo[counter].transform.position = topLeft + new Vector2(distance * i / (float) (bulletsToRain-1), 0);
            ammo[counter].transform.rotation = Quaternion.Euler(0, 0, 180f);
            ammo[counter].GetComponent<player_bullet>().rainingBullet = true;

            //active bullet and cycle to the next one in the future
            ammo[counter].SetActive(false);
            ammo[counter].SetActive(true);
            counter += 1;
            counter %= (weaponType.transform.childCount);

            yield return new WaitForSeconds(rainOffset);
        }
    }


    //-------------------------------------------------------------------------------
    //------------------------------SHORT SUB-METHODS--------------------------------
    //----------(Small methods that serve to make code more readable)---------------
    //-------------------------------------------------------------------------------

    //Flash effect for the gatling gun sub-method
    private IEnumerator Flash()
    {
        float r = UnityEngine.Random.Range(0.08f, 0.15f);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(r);
        transform.GetChild(2).gameObject.SetActive(false);
    }

}

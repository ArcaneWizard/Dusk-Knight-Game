using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    public float arrowInitLength = 28f;
    public float maxArrowLength = 60f;
    public float arrowGrowRate = 2f;
    public float aimArrWidth = 1.2f;
    public float chargeArrWidth = 1.2f;
    public float arrowWidthGrowRate = 0.1f;
    private float chargeArrowLength;
    public float minSwipeToShoot = 0.1f;
    public float zOffset = 0f;
    public float aimRotationExtreme = 75f;

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
    [Header("Objects")]
    public Transform muzzle;
    public Camera camera;
    public GameObject weaponAnchor;
    public GameObject arrowAnchor;
    private weapon_loadout wL;
    private AudioSource audioSource;

    [Space(10)]
    [Header("Bullet Type")]
    public GameObject[] bullets;

    [Space(10)]
    [Header("Player Character")]

    public GameObject player_Head;

    void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        lr = Aim_Arrow.GetComponent<LineRenderer>();  
        lr_2 = Charge_Arrow.GetComponent<LineRenderer>();   
        wL = transform.GetComponent<weapon_loadout>();
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
                magnitude = (endPosition - initPosition).magnitude;
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
        Fire(rot);
    }

    //Set cannon rotation
    private void changeRotation(float rot, Vector3 touchPosition) {

        //set cannon rotation if not facing backwards
        transform.rotation = Quaternion.Euler(0f, 0f, rot);

        //set the player head's rotation to look in the direction the player is aiming
        player_Head.transform.rotation = Quaternion.Euler(0f, 0f, rot / 5f);

        //rotate the guide aim arrows around the cannon 
        weaponAnchor.transform.rotation = transform.rotation;
    }

    //Fire the cannon
    void Fire(float rot)
    { 
        //set bullet's position and rotation
        ammo[counter].transform.position = muzzle.position;
        ammo[counter].transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z - 90);
        ammo[counter].GetComponent<player_bullet>().oneLaunch = false;
        
        //play bullet fire sound
        audioSource.PlayOneShot(Manage_Sounds.Instance.cannonShot, 0.8f * Manage_Sounds.soundMultiplier);
        
        //active bullet and cycle to the next one in the future
        ammo[counter].SetActive(false);
        ammo[counter].SetActive(true);
        counter += 1;
        counter %= (weaponType.transform.childCount);

        //lose one ammo
        wL.shotTaken();
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
}

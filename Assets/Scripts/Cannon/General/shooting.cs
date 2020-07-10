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

    private int counter = 0;
    private float rot;
    public static float touchPercent;
    private Vector3 initPosition;
    private Vector3 endPosition;
    private Vector3 initPosition2;
    private Vector3 endPosition2;
    private Vector3 centeredOffset;
    private Vector3 anchorPoint;
    private GameObject weaponType;
    private List<GameObject> ammo = new List<GameObject>();

    [Space(10)]
    [Header("Objects")]
    public Transform muzzle;
    public Camera camera;
    public GameObject weaponAnchor;
    public GameObject arrowAnchor;
    private AudioSource audioSource;

    [Space(10)]
    [Header("Bullet Type")]
    public int startingBullet;
    public GameObject[] bullets;

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();

        //select cannon ball as default bullet
        weaponType = bullets[startingBullet];
        equipWeapon();
    }

    //specify what weapon the player has
    public void equipWeapon()
    {
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

        //rotation to flip bullet images over when shooting left vs right
        float rotY = (touchPosition.x / Mathf.Abs(touchPosition.x) - 1) * 90f;
        float rotZ = (touchPosition.x / Mathf.Abs(touchPosition.x)) * rot + 90 * (touchPosition.x / Mathf.Abs(touchPosition.x) - 1);
        
        //set cannon rotation
        transform.rotation = Quaternion.Euler(0f, 0f, rot);

        //rotate the guide aim arrows around the cannon 
        weaponAnchor.transform.rotation = transform.rotation;
    }

    //Fire the cannon
    void Fire(float rot)
    {
        prepareBullet(rot + 270, Manage_Sounds.Instance.cannonShot);

        ammo[counter].SetActive(false);
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
        ammo[counter].transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 90);
        ammo[counter].GetComponent<player_bullet>().oneLaunch = false;

        //play bullet fire sound
        audioSource.PlayOneShot(hit_Sound, 0.8f * Manage_Sounds.soundMultiplier);
    }

    //Flash effect for the gatling gun sub-method
    private IEnumerator Flash()
    {
        float r = UnityEngine.Random.Range(0.08f, 0.15f);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(r);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}

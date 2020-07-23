using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //enemy's current health
    public float hp = 1;
    private float maxHP;

    //Detects whenever enemy health changes
    private float lastHP;

    //enemy hps
    private int orc = 100;
    private int ogre = 70;
    private int goblin = 100;
    private int reaper_1 = 60;
    private int reaper_3 = 80;

    //enemy speeds
    public static float orc_speed = 0.7f;
    public static float ogre_speed = 0.52f;
    public static float goblin_speed = 2.1f;
    public static float R1_speed = 1.8f;
    public static float R3_speed = 1f;

    //enemy dmg 
    public static float orcDmg = 3; //Every 1.2-4 seconds, it hits the tower (meelee)
    public static float ogreDmg = 2; //Every 5-13 seconds, it shoots (short-med range)
    public static float goblinDmg = 1.2f; //Every second (meelee)
    public static float R1Dmg = 14f; //Once upon reaching the tower
    public static float R3Dmg = 2f; //Every 2.5-5 seconds, it shoots (any range)
    
    //enemy attributes and conditions
    [HideInInspector] public bool deploy = false;
    [HideInInspector] public float freezeTimer;
    [HideInInspector] public float fireTimer = 0;
    private float freezeDuration = 3;
    private float fireDuration = 3;
    private float fireConstantDmgTimer = 0.99f;
    private bool death = false;
    private bool lowHP = false;
    private float ogScaleX;
    private float ogScaleY;
    [HideInInspector] public bool resetPath;

    //enemy feedback when taking dmg or dying
    private float hitRednessduration = 0.04f;
    private bool spinUponDeath = true; 
    private float deathDelay = 0.8f; 
    private float spinDelay = 0.3f; 
    private float scale = 0.93f;
    private float rotSpeed = 25f;

    //Gameobject References
    public Shop shop;
    public GameObject player;
    public Manage_Sounds manage_Sounds;
    private GameObject floating_Texts;
    private PolygonCollider2D enemyCollider;
    private GameObject snowball;
    private GameObject fire;
    private Animator animator;
    private SpriteRenderer renderer;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private string tag;

    //Floating texts which get recycled
    private List<GameObject> floatingTexts = new List<GameObject>();
    private int ft_cycleLength;
    private int ft_currentIndex;
    private Vector3 textOffset = new Vector3(1f, 0.3f, 0);
    private bool respawnDelay;
    private float resetTimer;

    [Space(10)]
    [Header("Sprites")]

    //Animation controllers
    public Sprite redSprite;
    public Sprite usualSprite;

    [Space(10)]
    [Header("Animation controllers")]

    public AnimatorOverrideController red;
    public RuntimeAnimatorController usual;

    // Start is called before the first frame update
    void Awake()
    {   
        //Define initial values to revert back to when respawned
        ogScaleX = transform.localScale.x;
        ogScaleY = transform.localScale.y;
        spinUponDeath = true;
    
        //Define components that need to be accessed
        animator = transform.GetComponent<Animator>();
        audioSource = transform.GetComponent<AudioSource>();
        renderer = transform.GetComponent<SpriteRenderer>();
        enemyCollider = transform.GetComponent<PolygonCollider2D>();
        rig = transform.GetComponent<Rigidbody2D>();

        snowball = transform.GetChild(0).gameObject;
        floating_Texts = transform.GetChild(1).gameObject;
        fire = transform.GetChild(2).gameObject;

        tag = gameObject.tag;

        //Add floating text child objects to an array
        foreach (Transform child in floating_Texts.transform) 
        {  floatingTexts.Add(child.gameObject);   }
        ft_cycleLength = floatingTexts.Count;

        //set stats like hp based off enemy type
        setStats();
    }

    public void setStats()
    {
        //Set hp based off specific enemy
        if (tag == "Enemy 1") 
            hp = goblin;         

        if (tag == "Enemy 2") 
            hp = orc;

        if (tag == "Enemy 3") 
            hp = reaper_3;

        //want reaper to explode on death not spin
        if (tag == "Enemy 4")
        {
            hp = reaper_1;
            spinUponDeath = false;
        }

        if (tag == "Enemy 5") 
            hp = ogre;            

        //reset Enemy values
        lastHP = hp;
        maxHP = hp;
        lowHP = false; 
        death = false;
        enemyCollider.enabled = true;
        resetPath = false;
        renderer.color = new Color32(255, 255, 255, 255);

        //reset Enemy physical attributes
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        transform.rotation = Quaternion.Euler(0, 0, 0); 

        //reset Enemy sprite/animation
        setAnimationOrSprite("usual");
    }

    void Update()
    {
        //dies
        if (hp <= 0 && death == false)
        {
            death = true;
            checkDeath();
            animator.enabled = false;
            StartCoroutine(fade());
        }
 
        //took dmg
        if (lastHP != hp)
        {
            lastHP = hp;
            StartCoroutine(flinch());
        }

        //at low HP
        if ((hp <= maxHP/3f || hp <= 20) && lowHP == false)
        {
            lowHP = true;
            setAnimationOrSprite("red");
        }

        //floating popup delay (in-between popups) will count down here
        if (resetTimer > 0)
            resetTimer -= Time.deltaTime;

        //freeze duration timer will count down here
        if (freezeTimer >= 0) 
            freezeTimer -= Time.deltaTime;  

        //When the freezeTimer has expired for a frozen enemy, unfreeze the enemy
        if (freezeTimer < 0 && snowball.activeSelf == true)
            freezeState(false);

        //fire duration timer will count down here
        if (fireTimer >= 0) {
            fireTimer -= Time.deltaTime;  

            //fire harms the enemy every second
            if (fireConstantDmgTimer <= 0)  {
                fireConstantDmgTimer = 0.95f;
                hp -= player_bullet.fireDmgPerSecond;
            }
            else
                fireConstantDmgTimer -= Time.deltaTime;
        }

        //When the firetimer has expired for an enemy lit on fire, stop burning it
        if (fireTimer < 0 && fire.activeSelf == true)
            fire.SetActive(false);
    }

    //flicker red when hit
    private IEnumerator flinch()
    {        
        //Turn the enemy red
        setAnimationOrSprite("red");

        yield return new WaitForSeconds(hitRednessduration);

        //Turn the enemy back to its normal skin tone (if it isn't low on health)
        if (lowHP == false) {
            setAnimationOrSprite("usual");
        }
    }

    //Turn the enemy red or back to its normal skin tone
    private void setAnimationOrSprite(string type)
    {
        //If an override controller isn't there for the enemy, use the single enemy red sprite 
        //Otherwise, use the override controller where the enemy is red in its animation
        if (type == "red")
        {
            if (red) {
                animator.runtimeAnimatorController = red;
            }
            else {
                animator.runtimeAnimatorController = null;
                renderer.sprite = redSprite;
            }

            //specific enemy filters when hurt
            if (tag == "Enemy 3") 
                renderer.color = new Color32(243, 227, 227, 255);
        }   
        
        if (type == "usual")
        {
            if (usual)
                animator.runtimeAnimatorController = usual;
            else
                renderer.sprite = usualSprite;

            //turn off any color filters
            renderer.color = new Color32(255, 255, 255, 255);
        }
    }

    //player projectile calls this to freeze the enemy with the ice effect
    public void activateFreeze() {
        freezeTimer = freezeDuration;
        freezeState(true);
    }

    //choose to freeze or unfreeze the enemy code-wise
    private void freezeState(bool freeze)
    {
        //enable snowball / disable snowball
        snowball.SetActive(freeze);

        //freeze the enemy animation / unfreeze the enemy animation
        animator.enabled = !freeze;
            
        //For the flying reaper, disable its movement script / enable its script
        if (transform.GetComponent<Reaper_3>() && freeze)
            rig.gravityScale = 1;
        if (transform.GetComponent<Reaper_3>() && !freeze) 
            rig.gravityScale = 0;

        //stop the enemy / let the enemy continue moving
        if (freeze) 
            rig.velocity = new Vector2(0, 0);
        else {
            resetPath = true;
            rig.WakeUp();
        }
    }

    //player projectile calls this to light the enemy on fire
    public void activateFire() {
        fireConstantDmgTimer = 0.95f;
        fireTimer = fireDuration;
        fire.SetActive(true);
    }

    //enemy fade animation
    private IEnumerator fade()
    {   
        //Un-ice enemy b4 it fades away
        freezeState(false);
        freezeTimer = 0;

        //Unlight enemy on fire b4 it fades away
        fire.SetActive(false);
        fireTimer = 0;

        //turn off enemy collisions with the outside environment
        enemyCollider.enabled = false;

        //render enemy still
        rig.gravityScale = 0;
        rig.velocity = new Vector2(0, 0);

        //if spin upon death is wanted, enemy spins and fades upon death
        if (spinUponDeath == true)
        {
            //kill velocity with slight delay (buffer for specific enemy AI scripts to stop setting enemy velocity)
            yield return new WaitForSeconds(0.02f);
            rig.velocity = new Vector2(0, 0);

            //kill dying animation after another delay
            yield return new WaitForSeconds(spinDelay);
            animator.runtimeAnimatorController = null;

            for (int yo = 17; yo >= 0; yo--)
            {
                //reduce sprite's alpha value to fade the enemy
                Color32 c = renderer.color;
                renderer.color = new Color32(c.r, c.g, c.b, (byte)(15 * yo));

                //scale down and rotate the enemy over time
                transform.localScale *= scale;
                transform.Rotate(new Vector3(0, 0, rotSpeed));

                yield return new WaitForSeconds(0.02f);
            }
        }
        else
            yield return new WaitForSeconds(deathDelay);

        //Disable floating texts 
        foreach(GameObject text in floatingTexts)   
            text.gameObject.SetActive(false);

        //Turn off the enemy
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //hit by a player projectile
        if (col.gameObject.layer == 25)
        {
            //play hit sound effect
            audioSource.PlayOneShot(Manage_Sounds.Instance.enemyHit, Manage_Sounds.soundMultiplier);
        }
    }

    //floating text that pops up above the enemy when hit
    public void floatText(string display, Color32 color) {

        //enable floating text popup
        GameObject t = floatingTexts[ft_currentIndex];
        t.gameObject.SetActive(false);
        t.gameObject.SetActive(true);
        
        //always spawn it at the same starting position 
        t.transform.localPosition = new Vector3(-0.25f, 2.44f, 0f);

        //make it appear in front of enemies
        t.transform.GetComponent<MeshRenderer>().sortingLayerName = "Enemies";
        t.transform.GetComponent<MeshRenderer>().sortingOrder = 100;

        //Add a slight offset to its position for variety
        t.transform.localPosition += new Vector3(UnityEngine.Random.Range(-textOffset.x,
         textOffset.x), UnityEngine.Random.Range(0, textOffset.y), 0);

        //Special popups appear above dmg text
        if (display == "Headshot")
        t.transform.localPosition += new Vector3(0, UnityEngine.Random.Range(1f, 2.2f), 0);

        //show the dmg or trickshot just dealt by the player
        t.transform.GetComponent<TextMesh>().text = display;

        //adjust the floating text's color
        t.transform.GetComponent<TextMesh>().color = color;

        //recycle the next text in the list next time
        ft_currentIndex = ++ft_currentIndex % ft_cycleLength;
    }

    //when killed, give player money 
    private void checkDeath()
    {
        animator.SetBool("Dead", true);

        giveJewels("Enemy 1", 5);
        giveJewels("Enemy 2", 3);
        giveJewels("Enemy 3", 2);
        giveJewels("Enemy 4", 3);
        giveJewels("Enemy 5", 3);
    }

    private void giveJewels(string enemyNumber, int jewels)
    {
        if (tag == enemyNumber)
            shop.jewels += jewels;
    }

}

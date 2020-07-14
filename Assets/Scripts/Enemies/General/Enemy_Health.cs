using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector] enemy's current health
    public float hp = 1;

    //Detects whenever enemy health changes
    private float lastHP;

    //enemy hps
    private int orc = 100;
    private int ogre = 70;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    //enemy speeds
    public static float orc_speed = 0.7f;
    public static float ogre_speed = 0.52f;
    public static float goblin_speed = 2.1f;
    public static float R1_speed = 1.8f;
    public static float R2_speed = 0.9f;
    public static float R3_speed = 1f;
    
    //enemy attributes and conditions
    public bool deploy = true;
    public bool poison = false;
    public bool isPoisoned = false;
    public bool iced = false;
    public bool isIced = false;
    public int isIcedWhileIced = 0;
    public int isIcedWhileIcedCheck = 0;
    private bool death = false;
    private bool lowHP = false;
    public float dmgMultiplier = 1;
    private float ogScaleX;
    private float ogScaleY;

    //enemy feedback when taking dmg or dying
    private float hitRednessduration = 0.2f;
    private bool spinUponDeath = true; 
    private float deathDelay = 1f; 
    private float spinDelay = 0.6f; 
    private float scale = 0.93f;
    private float rotSpeed = 25f;

    //Enemy color overlays
    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 flinchColor = new Color32(192, 0, 0, 255);

    //Gameobject References
    public Shop shop;
    public GameObject player;
    public Manage_Sounds manage_Sounds;
    private GameObject floating_Texts;
    private PolygonCollider2D enemyCollider;
    private GameObject snowball;
    private Animator animator;
    private SpriteRenderer render;
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
        render = transform.GetComponent<SpriteRenderer>();
        enemyCollider = transform.GetComponent<PolygonCollider2D>();
        snowball = transform.GetChild(0).gameObject;
        floating_Texts = transform.GetChild(1).gameObject;
        rig = transform.GetComponent<Rigidbody2D>();
        tag = gameObject.tag;

        //Add floating text child objects to an array
        foreach (Transform child in floating_Texts.transform) 
        {  floatingTexts.Add(child.gameObject);   }
        ft_cycleLength = floatingTexts.Count;

        //set hp based off enemy type
        setHP();
    }

    public void setHP()
    {
        //Set hp and red took-dmg color based on the specific enemy
        flinchColor = new Color32(215, 39, 39, 255);  

        if (tag == "Enemy 1") 
            hp = goblin;         

        if (tag == "Enemy 2") {
            hp = orc;
            flinchColor = new Color32(255, 46, 46, 255);
        }

        if (tag == "Enemy 3") 
            hp = reaper_3;

        if (tag == "Enemy 4")
            hp = reaper_1;

        if (tag == "Enemy 5") 
            hp = ogre;            

        //reset Enemy values
        lastHP = hp;
        lowHP = false; 
        death = false;
        enemyCollider.enabled = true;

        //reset Enemy physical attributes
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        transform.rotation = Quaternion.Euler(0, 0, 0); 
        render.color = normal;
    }

    void Update()
    {
        //poisoned
        if (poison == true && isPoisoned == false)
        {
            poison = false;
            isPoisoned = true;
            StartCoroutine(poisoned());
        }

        //frozen
        if (iced == true)
        {
            iced = false;
            isIced = true;
            isIcedWhileIced++;
            StartCoroutine(frozen());
        }

        //dead
        if (hp <= 0 && death == false)
        {
            death = true;
            checkDeath();
        }
 
        //just took a hit
        if (lastHP != hp)
        {
            lastHP = hp;

            if (isPoisoned == false)
                StartCoroutine(flinch());
        }

        //at low HP
        if (hp <= 20 && lowHP == false)
        {
            lowHP = true;
            render.color = flinchColor;
        }

        //floating popup in-between delay counting down
        if (resetTimer > 0)
            resetTimer -= Time.deltaTime;
    }

    private IEnumerator flinch()
    {
        render.color = flinchColor;
        yield return new WaitForSeconds(hitRednessduration);

        if (lowHP == false)
            render.color = normal;
    }

    private IEnumerator frozen()
    {
        //enable snowball
        snowball.SetActive(true);

        //freeze
        animator.enabled = false;
        setSpeed(0, 1);

        if (tag == "Enemy 1")
            transform.GetComponent<Goblin>().enabled = false;

        if (tag == "Enemy 2")
            transform.GetComponent<Orc>().enabled = false;

        if (tag == "Enemy 3") {
            transform.GetComponent<Reaper_3>().enabled = false;
            rig.gravityScale = 1;
        }

        if (tag == "Enemy 4")
            transform.GetComponent<Reaper_1>().enabled = false;
        
        if (tag == "Enemy 5")
            transform.GetComponent<Ogre>().enabled = false;

        yield return new WaitForSeconds(4f);

        isIcedWhileIcedCheck++;

        //if not iced again and freeze time is over
        if (isIcedWhileIcedCheck == isIcedWhileIced)
        {
            snowball.SetActive(false);
            transform.GetComponent<Animator>().enabled = true;
            setSpeed(1, 1);
            isIced = false;
        }
    }

    //alter enemy speed 
    private void setSpeed(float sign, float multiplier)
    {
        if (tag == "Enemy 1")
        {
            transform.GetComponent<Goblin>().enabled = true;
            rig.velocity = new Vector2(goblin_speed * sign * multiplier, 0);
        }

        if (tag == "Enemy 2")
        {
            transform.GetComponent<Orc>().enabled = true;
            rig.velocity = new Vector2(orc_speed * sign * multiplier, 0);
        }

        if (tag == "Enemy 3")
        {
            transform.GetComponent<Reaper_3>().enabled = true;
            rig.velocity = new Vector2(R2_speed * sign * multiplier, 0);
        }

        if (tag == "Enemy 4")
        {
            transform.GetComponent<Reaper_1>().enabled = true;
            rig.velocity = new Vector2(R1_speed * sign * multiplier, 0);
        }

        if (tag == "Enemy 5")
        {
            transform.GetComponent<Ogre>().enabled = true;
            rig.velocity = new Vector2(ogre_speed * sign * multiplier, 0);
        }
    }

    //enemy is poisoned
    private IEnumerator poisoned()
    {
        for (int i = 0; i < 5; i++)
        {
            if (death == false)
            {
                render.color = new Color32(56, 219, 143, 255);
                yield return new WaitForSeconds(0.22f);
                hp -= 10;
                render.color = normal;
                yield return new WaitForSeconds(0.8f);
            }
        }
        isPoisoned = false;
    }

    //enemy fade animation
    private IEnumerator fade()
    {
        if (gameObject.layer == 19)
            gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        
        //Un-ice and unpoison enemy b4 it fades away
        isPoisoned = false;
        isIced = false;
        snowball.SetActive(false);

        //turn off enemy collisions with the outside environment
        enemyCollider.enabled = false;

        //render enemy still
        rig.gravityScale = 0;
        rig.velocity = new Vector2(0, 0);   
    
        //if spin upon death is wanted, enemy spins and fades upon death
        if (spinUponDeath == true)
        {
            yield return new WaitForSeconds(spinDelay);
            for (int yo = 17; yo >= 0; yo--)
            {
                //reduce sprite's alpha value to fade the enemy
                Color32 c = render.color;
                render.color = new Color32(c.r, c.g, c.b, (byte)(15 * yo));

                //scale down and rotate the enemy over time
                transform.localScale *= scale;
                transform.Rotate(new Vector3(0, 0, rotSpeed));

                yield return new WaitForSeconds(0.02f);
            }
        }
        else 
            yield return new WaitForSeconds(deathDelay);

        //Reset the iced value for when enemy next spawns
        isIcedWhileIced = isIcedWhileIcedCheck;

        //Disable floating texts 
        foreach(GameObject text in floatingTexts)   
            text.gameObject.SetActive(false);


        //Turn off the enemy
        gameObject.SetActive(false);
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

        StartCoroutine(fade());
    }

    public float returnSpeed()
    {
        GameObject enemy = transform.gameObject;
        float speed = 0;

        if (tag == "Enemy 1")
            speed = goblin_speed;
        if (tag == "Enemy 2")
            speed = orc_speed;
        if (tag == "Enemy 3")
            speed = R3_speed;
        if (tag == "Enemy 4")
            speed = R1_speed;
        if (tag == "Enemy 5")
            speed = ogre_speed;

        if (enemy.transform.position.x > player.transform.position.x)
            speed *= -1;

        return speed;
    }

    public void giveJewels(string enemyNumber, int jewels)
    {
        if (tag == enemyNumber)
            shop.jewels += jewels;
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
}

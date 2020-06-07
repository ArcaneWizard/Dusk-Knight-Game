using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector]
    public float hp = 1;
    private float lastHP;

    //enemy hps
    private int orc = 100;
    private int ogre = 70;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    //enemy speeds
    public static float orc_speed = 0.4f;
    public static float ogre_speed = 0.4f;
    public static float goblin_speed = 2f;
    public static float R1_speed = 0.9f;
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

    //enemy feedback when taking dmg
    private float hitRednessduration = 0.2f;
    private bool spinUponDeath = true; 
    private float deathDelay = 1f; 
    private float spinDelay = 0.6f; 

    //Enemy color overlays
    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 flinchColor = new Color32(192, 0, 0, 255);

    //Gameobject References
    public Shop shop;
    public GameObject player;
    public Manage_Sounds manage_Sounds;
    private PolygonCollider2D enemyCollider;
    private GameObject snowball;
    private Animator animator;
    private SpriteRenderer render;
    private Rigidbody2D rig;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        ogScaleX = transform.localScale.x;
        ogScaleY = transform.localScale.y;
        spinUponDeath = true;

        animator = transform.GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        render = transform.GetComponent<SpriteRenderer>();
        enemyCollider = transform.GetComponent<PolygonCollider2D>();
        snowball = transform.GetChild(0).gameObject;
        rig = transform.GetComponent<Rigidbody2D>();

        setHP();
    }

    public void setHP()
    {
        //Set hp based on enemy type
        if (gameObject.layer == 8) {
            hp = orc;
            flinchColor = new Color32(255, 46, 46, 255);
        }

        if (gameObject.layer == 9) 
            hp = ogre;            

        if (gameObject.layer == 11) {
            hp = goblin;         
            flinchColor = new Color32(215, 39, 39, 255);   
        }

        if (gameObject.layer == 19) {
            hp = reaper_1;
            flinchColor = new Color32(215, 39, 39, 255);
        }

        if (gameObject.layer == 20)
            hp = reaper_2;

        if (gameObject.layer == 21) {
            hp = reaper_3;

            if (gameObject.GetComponent<Reaper_3>() != null) {
                Destroy(gameObject.GetComponent<Reaper_3>());
                gameObject.AddComponent<Reaper_3>();
            }
        }

        //reset Enemy values
        lastHP = hp;
        lowHP = false; 
        death = false;
        enemyCollider.enabled = true;

        //reset Enemy physical attributes
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        rig.gravityScale = 1;
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

        if (gameObject.layer == 8)
            transform.GetComponent<Orc>().enabled = false;

        if (gameObject.layer == 9)
            transform.GetComponent<Ogre>().enabled = false;

        if (gameObject.layer == 11)
            transform.GetComponent<Goblin>().enabled = false;

        if (gameObject.layer == 19)
            transform.GetComponent<Reaper_1>().enabled = false;

        if (gameObject.layer == 20)
            transform.GetComponent<Reaper_2>().enabled = false;

        if (gameObject.layer == 21) {
            transform.GetComponent<Reaper_3>().enabled = false;
            rig.gravityScale = 1;
        }

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
        if (gameObject.layer == 8)
        {
            transform.GetComponent<Orc>().enabled = true;
            rig.velocity = new Vector2(orc_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 9)
        {
            transform.GetComponent<Ogre>().enabled = true;
            rig.velocity = new Vector2(ogre_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 11)
        {
            transform.GetComponent<Goblin>().enabled = true;
            rig.velocity = new Vector2(goblin_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 19)
        {
            transform.GetComponent<Reaper_1>().enabled = true;
            rig.velocity = new Vector2(R1_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 20)
        {
            transform.GetComponent<Reaper_2>().enabled = true;
            rig.velocity = new Vector2(R2_speed * sign * multiplier, 0);
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
        yield return new WaitForSeconds(0.1f);
        
        //Un-ice and unpoison enemy b4 it fades away
        isPoisoned = false;
        isIced = false;
        snowball.SetActive(false);
        enemyCollider.enabled = false;

        //enemy now remains still
        rig.gravityScale = 0;
        rig.velocity = new Vector2(0, 0);        
        animator.enabled = true;

        float scale = 0.93f;
        float rotSpeed = 25f;

        //if spin upon death is wanted 
        if (spinUponDeath == true)
        {
            yield return new WaitForSeconds(spinDelay);
            for (int yo = 17; yo >= 0; yo--)
            {
                Color32 c = render.color;
                render.color = new Color32(c.r, c.g, c.b, (byte)(15 * yo));
                transform.localScale *= scale;
                transform.Rotate(new Vector3(0, 0, rotSpeed));

                yield return new WaitForSeconds(0.02f);
            }
        }
        else 
            yield return new WaitForSeconds(deathDelay);

        //Reset all values for the enemy when it next spawns
        isIcedWhileIced = isIcedWhileIcedCheck;

        //Individual enemy reset requests
        if (gameObject.name == "Reaper 1")
            Destroy(rig);

        gameObject.SetActive(false);
    }

    //when killed, give player money 
    private void checkDeath()
    {
        animator.SetBool("Dead", true);

        giveJewels(8, 5);
        giveJewels(9, 3);
        giveJewels(11, 2);
        giveJewels(19, 3);
        giveJewels(20, 3);
        giveJewels(21, 2);

        StartCoroutine(fade());
    }

    public float returnSpeed()
    {
        GameObject enemy = transform.gameObject;
        float speed = 0;

        if (enemy.layer == 8)
            speed = orc_speed;
        if (enemy.layer == 9)
            speed = ogre_speed;
        if (enemy.layer == 11)
            speed = goblin_speed;
        if (enemy.layer == 19)
            speed = R1_speed;
        if (enemy.layer == 20)
            speed = R2_speed;

        if (enemy.transform.position.x > player.transform.position.x)
            speed *= -1;

        return speed;
    }

    public void giveJewels(int layer, int jewels)
    {
        if (gameObject.layer == layer)
           shop.jewels += jewels;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //hit by a player projectile
        if (col.gameObject.layer == 25)
            audioSource.PlayOneShot(Manage_Sounds.Instance.enemyHit, Manage_Sounds.soundMultiplier);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        //If a flying reaper hits the ground when iced
        if (transform.gameObject.layer == 21) 
        {  
            //hit ground or player cannon
            if (col.gameObject.layer == 22 || col.gameObject.layer == 10)
                hp = 0f;
        }
    }

}

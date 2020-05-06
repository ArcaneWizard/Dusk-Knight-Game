using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector]
    public float hp = 1;
    private float lastHP;

    private int orc = 100;
    private int ogre = 70;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    public static float orc_speed = 0.4f;
    public static float ogre_speed = 0.4f;
    public static float goblin_speed = 0.9f;
    public static float R1_speed = 0.9f;
    public static float R2_speed = 0.9f;

    Animator animator;
    private bool death = false;
    private bool lowHP = false;

    private float ogScaleX;
    private float ogScaleY;

    public bool deploy = true;
    public bool poison = false;
    public bool isPoisoned = false;
    public bool iced = false;
    public bool isIced = false;
    public int isIcedWhileIced = 0;
    public int isIcedWhileIcedCheck = 0;

    //For testing gem animation + enemy colors taking dmg
    private float hitRednessduration = 0.01f;
    private bool spinUponDeath = true; 
    private float deathDelay = 1f; //set this if spinUponDeath is false
    private float spinDelay = 0.6f; //set this if spinUponDeath is true (ignores deathDelay)

    private Color32 color;
    private Color32 designatedColor;
    private Color32 normal = new Color32(255, 255, 255, 255);
    private Color32 medium = new Color32(0, 16, 255, 255);
    private Color32 powerful = new Color32(184, 12, 255, 255);

    private Color32 flinchColor = new Color32(192, 0, 0, 255);

    public float dmgMultiplier = 1;

    // Start is called before the first frame update
    void Awake()
    {
        setHP();
        ogScaleX = transform.localScale.x;
        ogScaleY = transform.localScale.y;
        animator = transform.GetComponent<Animator>();
        gameObject.AddComponent<AudioSource>();
        color = transform.GetComponent<SpriteRenderer>().color;
        spinUponDeath = true;
    }

    public void setHP()
    {
        //Set hp based on enemy type
        if (gameObject.layer == 8)
            hp = orc;

        if (gameObject.layer == 9)
            hp = ogre;

        if (gameObject.layer == 11) {
            hp = goblin;            
        }

        if (gameObject.layer == 19)
            hp = reaper_1;

        if (gameObject.layer == 20)
            hp = reaper_2;

        if (gameObject.layer == 21)
        {
            hp = reaper_3;
           
            //Re-add reaper3 script for new flight pattern
            if (gameObject.GetComponent<Reaper_3>() != null) 
                Destroy(gameObject.GetComponent<Reaper_3>());
                gameObject.AddComponent<Reaper_3>();
        }

        lastHP = hp;
        death = false;

        transform.GetComponent<PolygonCollider2D>().enabled = true;
        if (gameObject.tag == "Ranged Shooter")
            transform.localPosition = new Vector2(0, 0);

        //Assigns 3 different enemy modes with different stats
        int c = UnityEngine.Random.Range(0, 20);
        if (c >= 0 && c <= 15)
        {
            designatedColor = normal;
            hp *= 1;
            dmgMultiplier = 1.0f;
        }
        if (c >= 15 && c <= 17)
        {
            designatedColor = medium;
            hp *= 2;
            dmgMultiplier = 1.5f;
        }
        if (c >= 18)
        {
            designatedColor = powerful;
            hp *= 3;
            dmgMultiplier = 2f;
        }
            color = designatedColor;
        
        //Assign the color when an enemy is at low health;
        //deathColor = designatedColor;
    }

    void Update()
    {
        if (poison == true && isPoisoned == false)
        {
            poison = false;
            isPoisoned = true;
            StartCoroutine(poisoned());
        }

        if (iced == true)
        {
            iced = false;
            isIced = true;
            isIcedWhileIced++;
            StartCoroutine(frozen());
        }

        if (hp <= 0 && death == false)
        {
            transform.GetComponent<ParticleSystem>().Play();
            checkDeath();
            death = true;
        }

        if (hp <= 20 && lowHP == false)
        {
            lowHP = true;
            //gameObject.transform.GetComponent<SpriteRenderer>().color = deathColor;
        }

        if (lastHP != hp)
        {
            lastHP = hp;
            print("hit");
            if (isPoisoned == false)
                StartCoroutine(alter());
        }
    }

    private IEnumerator frozen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        float sign = 1;
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            sign = -1;

        setSpeed(sign, 0.5f);

        transform.GetComponent<Animator>().enabled = false;

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

        if (gameObject.layer == 21)
            Destroy(transform.GetComponent<Reaper_3>());

        if (transform.parent.parent.name == "R3 Group")
            transform.GetComponent<Rigidbody2D>().gravityScale = 1;

        yield return new WaitForSeconds(4f);


        if (transform.parent.parent.name == "R3 Group")
        {
            transform.GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.gameObject.AddComponent<Reaper_3>();
        }



        isIcedWhileIcedCheck++;

        if (isIcedWhileIcedCheck == isIcedWhileIced)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetComponent<Animator>().enabled = true;

            setSpeed(sign, 1);
            isIced = false;
        }
    }

    private void setSpeed(float sign, float multiplier)
    {

        if (gameObject.layer == 8)
        {
            transform.GetComponent<Orc>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(orc_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 9)
        {
            transform.GetComponent<Ogre>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(ogre_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 11)
        {
            transform.GetComponent<Goblin>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(goblin_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 19)
        {
            transform.GetComponent<Reaper_1>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(R1_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 20)
        {
            transform.GetComponent<Reaper_2>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(R2_speed * sign * multiplier, 0);
        }
    }

    private IEnumerator poisoned()
    {
        for (int i = 0; i < 5; i++)
        {
            if (death == false)
            {
                gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(56, 219, 143, 255);
                yield return new WaitForSeconds(0.22f);
                hp -= 10;
                gameObject.transform.GetComponent<SpriteRenderer>().color = designatedColor;
                yield return new WaitForSeconds(0.8f);
            }
        }
        isPoisoned = false;
    }

    private IEnumerator fade()
    {
        yield return new WaitForSeconds(0.1f);
        //Un-ice and unpoison enemy b4 it fades away
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().gravityScale = 0;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        transform.GetComponent<SpriteRenderer>().color = designatedColor;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetComponent<Animator>().enabled = true;
        setSpeed(1, 0);
        isIced = false;

        isPoisoned = false;

        // yield return new WaitForSeconds(1.0f);

        float scale = 0.93f;
        float rotSpeed = 25f;

        if (spinUponDeath == true)
        {
                print("wa!");
            yield return new WaitForSeconds(spinDelay);
            for (int yo = 17; yo >= 0; yo--)
            {
                print("spin!");
                Color32 c = transform.GetComponent<SpriteRenderer>().color;
                transform.GetComponent<SpriteRenderer>().color = new Color32(c.r, c.g, c.b, (byte)(15 * yo));
                transform.localScale *= scale;
                transform.Rotate(new Vector3(0, 0, rotSpeed));

                yield return new WaitForSeconds(0.02f);
            }
        }
        else {
            yield return new WaitForSeconds(deathDelay);
        }   

        print("woah");
        isIcedWhileIced = isIcedWhileIcedCheck;
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        transform.rotation = Quaternion.Euler(0, 0, 0); 
        transform.GetComponent<SpriteRenderer>().color = designatedColor;

        if (gameObject.tag != "Ranged Shooter")
            gameObject.SetActive(false);
        else
            gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void undoFade()
    {

    }

    private void checkDeath()
    {
        print("dead");
        animator.SetBool("Dead", true);

        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().gravityScale = 0;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        if (gameObject.layer == 21)
            transform.GetChild(1).gameObject.SetActive(false);

        giveJewels(8, 5);
        giveJewels(9, 3);
        giveJewels(11, 2);
        giveJewels(19, 3);
        giveJewels(20, 3);
        giveJewels(21, 2);

        StartCoroutine(fade());
    }

    public void giveJewels(int layer, int jewels)
    {
        if (gameObject.layer == layer)
            GameObject.Find("Shop").gameObject.GetComponent<Shop>().jewels += jewels;
    }

    private IEnumerator alter()
    {
        gameObject.transform.GetComponent<SpriteRenderer>().color = flinchColor;
        yield return new WaitForSeconds(hitRednessduration);
        //if (lowHP == false)
        gameObject.transform.GetComponent<SpriteRenderer>().color = designatedColor;
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

        if (enemy.transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            speed *= -1;

        return speed;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 25) //hits a player projectile
        {
            Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
            if (col.gameObject.name == "arrow")
            {
                transform.GetComponent<AudioSource>().PlayOneShot(m.arrowhit, 0.2f * Manage_Sounds.soundMultiplier);
            }
            if (col.gameObject.name == "flask")
            {
                transform.GetComponent<AudioSource>().PlayOneShot(m.potionhit, 1.25f * Manage_Sounds.soundMultiplier);
            }
            transform.GetComponent<AudioSource>().PlayOneShot(m.enemyHit, 0.3f * Manage_Sounds.soundMultiplier);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (transform.parent.parent.name == "R3 Group") //If a flying reaper hits the ground (meaning it was frozen in air and dropped down)
        {
            if (col.gameObject.layer == 22 || col.gameObject.layer == 10)
                col.gameObject.transform.GetComponent<Enemy_Health>().hp = 0;
        }
    }
    void OnCollisionStay2D(Collision2D col)
    {
        if (transform.parent.parent.name == "R3 Group") //If a flying reaper hits the ground (meaning it was frozen in air and dropped down)
        {
            if (col.gameObject.layer == 22 || col.gameObject.layer == 10)
                gameObject.transform.GetComponent<Enemy_Health>().hp = 0f;
        }
    }

}

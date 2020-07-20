using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class player_bullet : MonoBehaviour
{
    //Bullet characteristics or modifiers
    public float speed = 3500f;
    private bool enoughEnemiesHit, syncRotation;
    [HideInInspector]
    public bool oneLaunch = false;

    //Bounds detection 
    Vector3 bottomLeft, topRight, p;
    float minX, minY, maxX, maxY;
    private bool inBounds;

    //Objects used
    public Camera camera;
    private Rigidbody2D rig;
    private SpriteRenderer renderer;
    private Collider2D collider;
    private string weapon;

    //Weapon Dmg
    private int arrowDmg = 40;
    private int boulderDmg = 80;
    private int iceShardDmg = 30;
    private int cannonBallDmg = 30;
    private int soulAxeDmg = 25;
    private int rocketDmg = 30;
    private int fireContactDmg = 20;
    public static float fireDmgPerSecond = 10f;

    //Weapon Effects 
    private int enemiesPierced = 0;
    private int enemiesToBePierced = 1;
    private int rocketSpeed = 100;
    private int rocketCentripetalForce = 200;
    private float rocketLaunchDelay = 0.01f;

    [Space(10)]
    [Header("For Animated bullets only")]

    //Main sprite for animated bullet
    public Sprite DefaultSprite;
    
    void Awake() {
       //Get bounds of the screen for any screen size
        bottomLeft = camera.ViewportToWorldPoint(new Vector2(0,0));
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        minX = bottomLeft.x - 1;
        minY = bottomLeft.y - 1;
        maxX = topRight.x + 1;
        maxY = topRight.y + 1;

        //Define components
        rig = transform.GetComponent<Rigidbody2D>();
        renderer = transform.GetComponent<SpriteRenderer>();
        collider = transform.GetComponent<Collider2D>();
        weapon = transform.parent.name;
    }

    void Update()
    {
        p = transform.position;
        inBounds = p.x > minX && p.y > minY && p.x < maxX;
        
        //called once when the bullet is shot out
        if (oneLaunch == false)
        {   
            //configure rigidbody settings
            rig.velocity = new Vector2(0, 0);
            rig.gravityScale = 2;
            rig.AddForce(transform.up * speed * shooting.touchPercent);

            //rotate as the bullet falls
            syncRotation = false;
            Invoke("enableRotation", 0.03f);
            
            //configure specific weapon effects
            configureBullet();

            //no accidental splash dmg + call this method once
            enoughEnemiesHit = false;
            oneLaunch = true;

            //reset sprite to fully visibility and re-enable the collider 
            renderer.color = new Color32(255, 255, 255, 255);
            collider.enabled = true;

            //if the bullet is animated upon destruction, reset its main sprite
            if (DefaultSprite)
                renderer.sprite = DefaultSprite;
        }
        
        //check when/if the bullet exits the screen bounds
        if (!inBounds && weapon != "Mini Rockets")
            transform.gameObject.SetActive(false);

        //sync bullet rotation with how it travels through the air
        if (syncRotation == true && weapon != "Soul Axes")
           transform.up = rig.velocity.normalized;

        //whirl method for mini Rocket
        if (syncRotation == true && weapon == "Mini Rockets") {
            rig.gravityScale = 0;            
            rig.AddForce(transform.right * rocketCentripetalForce);
        }
    }
    
    //ENEMY COLLISIONS
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            /* NOTE: (enoughEnemiesHit = true) ensures there is no accidental splash dmg when the bullet 
               collides with multiple enemies simulatanaeously */

            if (weapon == "Arrows" && enoughEnemiesHit == false)
            {
                //Show a dmg popup above the enemy with the dmg the arrow does
                dmgPopup(arrowDmg, col);

                //increase enemiesPierced every time this arrow passes through an enemy
                enemiesPierced++;

                //Check if the maximum number of enemies this arrow can pierce through has been reached
                if (enemiesPierced >= enemiesToBePierced) 
                {
                    //Set this bool to stop accidental splash dmg
                    enoughEnemiesHit = true;

                    //De-activate this bullet
                    gameObject.SetActive(false);
                }
            }
            
            if (weapon == "Boulders" && enoughEnemiesHit == false)
            {
                dmgPopup(boulderDmg, col);
                enoughEnemiesHit = true;

                //De-activate this bullet
                gameObject.SetActive(false);                
            }
            
            if (weapon == "Ice shards" && enoughEnemiesHit == false)
            {
                dmgPopup(iceShardDmg, col);
                enoughEnemiesHit = true;

                //Call a method to freeze the enemy and then de-activate this bullet
                col.transform.GetComponent<Enemy_Health>().activateFreeze();
                gameObject.SetActive(false);                
            }
            
            if (weapon == "Cannon Balls" && enoughEnemiesHit == false)
            {
                enoughEnemiesHit = true;

                //Launch an enemy up into the air and enable its gravity
                float xForce = UnityEngine.Random.Range(20, 30);
                float yForce = UnityEngine.Random.Range(200, 300);

                Vector2 bulletDir = transform.up.normalized;
                Vector2 force = new Vector2(xForce, yForce);

                StartCoroutine(knockback(col, force));            
            }
            
            if (weapon == "Soul Axes" && enoughEnemiesHit == false)
            {
                enoughEnemiesHit = true;      
                
                //add hp-dependent dmg code here 
                dmgPopup(soulAxeDmg, col); 
                gameObject.SetActive(false);
            }            

            if (weapon == "Mini Rockets" && enoughEnemiesHit == false) {
                enoughEnemiesHit = true;
                
                dmgPopup(rocketDmg, col); 
                gameObject.SetActive(false);
            }

            if (weapon == "Fireballs" && enoughEnemiesHit == false) {
                enoughEnemiesHit = true;
                
                //call a method to light the enemy on fire
                col.transform.GetComponent<Enemy_Health>().activateFire();
                dmgPopup(fireContactDmg, col);
                gameObject.SetActive(false);
            }
        }

        //HILL COLLISIONS

        //collides with the hill
        if (col.gameObject.layer == 15)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            //turn off rotation and the collider
            syncRotation = false;
            collider.enabled = false;

            //stop the bullet's movement completely
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            //fade out over time or play an animation where the bullet breaks
            if (gameObject.activeSelf == true && gameObject.tag == "Fading bullet")
                StartCoroutine(fade());
            else if (gameObject.activeSelf == true && gameObject.tag == "Animated bullet")
                StartCoroutine(fadeAnimation());
            else
                gameObject.SetActive(false);

        }
    }

    //slowly fade this bullet tills its invisible (called for some bullets that hit the ground)
    private IEnumerator fade() {
        byte alpha = 255;
        while (alpha > 5) {
            alpha -= 5;
            renderer.color = new Color32(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);        
    }

   
    //play bullet breaking animation and then make the bullet disappear
    private IEnumerator fadeAnimation() {

        //play animation right side up always
        transform.rotation = Quaternion.Euler(0, 0, 180f);
        transform.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(0.583f);
        
        //stop animation and reset rotation after the animation is complete
        transform.GetComponent<Animator>().enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, 90);

        gameObject.SetActive(false);        
    }

    //sync rotation with movement (called with a slight delay)
    private void enableRotation() 
    {     
        if (weapon != "Mini Rockets")
            syncRotation = true;
        else
            StartCoroutine(startRocketWhirl());

        if (weapon == "Soul Axes")
            StartCoroutine(rotateSoulAxe());
    }

    //Soul axe rotation 
    private IEnumerator rotateSoulAxe() {
        if (syncRotation == true)
        {
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 15);
            yield return new WaitForSeconds(0.03f);
            StartCoroutine(rotateSoulAxe());
        }
    }

    //Rocket rotation
    private IEnumerator startRocketWhirl() {
        yield return new WaitForSeconds(rocketLaunchDelay);
        speed = rocketSpeed;
        syncRotation = true;
    }

    //Call for a dmg popup above the enemy hit
    private void dmgPopup(int dmg, Collider2D col) 
    {
        //show the dmg recieved in a text popup
        Enemy_Health e = col.gameObject.transform.GetComponent<Enemy_Health>();
        e.floatText(dmg.ToString(), Color.white);

        //show any headshots in a text popup 
        if (transform.eulerAngles.z > 180f && transform.eulerAngles.z < 220f)
            e.floatText("Headshot", new Color32(28, 75, 123, 255));

        //dmg that enemy and then turn off the bullet
        col.gameObject.transform.GetComponent<Enemy_Health>().hp -= dmg;
    }

    //configure general and specific bullet settings based on the weaponType
    private void configureBullet()
    {
        if (weapon == "Arrows") {
            enemiesPierced = 0;
            enemiesToBePierced = UnityEngine.Random.Range(1, 3);
        }
    }

    //Add knockback effect to an enemy
    private IEnumerator knockback(Collider2D col, Vector2 force)
    {
        //Disable velocity and add knockback force to the enemy
        col.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        print(col.transform.GetComponent<Rigidbody2D>().gravityScale + ",, " + col.gameObject.name);
        col.transform.GetComponent<Rigidbody2D>().AddForce(force);

        //Re-enable gravity for the enemy, albeit with a buffer effect so it can get off the ground 
        yield return new WaitForSeconds(0.03f);
        col.transform.GetComponent<Rigidbody2D>().gravityScale = 1;
        print(col.transform.GetComponent<Rigidbody2D>().gravityScale + ", " + col.gameObject.name);

        //De-activate this bullet
        dmgPopup(cannonBallDmg, col);
        gameObject.SetActive(false);
    }
} 

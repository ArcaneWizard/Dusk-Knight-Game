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
    [HideInInspector] public bool oneLaunch = false;

    //Bounds detection 
    Vector3 bottomLeft, topRight, p;
    float minX, minY, maxX, maxY;
    private bool inBounds;

    //Objects used
    public Camera camera;
    private Rigidbody2D rig;
    private SpriteRenderer renderer;
    private Collider2D collider;
    private Animator animator;
    private string weapon;

    //Weapon Dmg
    private int arrowDmg = 40;
    private int boulderDmg = 80;
    private int iceShardDmg = 30;
    private int cannonBallDmg = 30;
    private int rocketDmg = 30;
    private int fireContactDmg = 20;
    private int soulAxeDmg; 

    private Vector2 soulAxeDmgBound = new Vector2(20, 80);
    public static int rocketExplosionDmg = 10;
    public static float fireDmgPerSecond = 10f;

    //Dmg multiplier based off knight's state
    public static float dmgMultiplier = 1;

    //Weapon Effects 
    private int enemiesPierced = 0;
    private int enemiesToBePierced = 1;

    //Rocket effects
    private int rocketSpeed = 100;
    private int rocketCentripetalForce = 400;
    private float rocketLaunchDelay = 0.01f;
    private rocketExplosion explosion;

    private Vector2 knockbackForce;
    public static int enemiesHitSpree;

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
        animator = transform.GetComponent<Animator>();
        weapon = transform.parent.name;
    }

    void Update()
    {
        p = transform.position;
        inBounds = p.x > minX && p.y > minY && p.x < maxX;
        
        //called once when the bullet is shot out
        if (oneLaunch == false)
        {   
            //configure specific weapon effects
            configureBullet();

            //configure rigidbody settings
            rig.velocity = new Vector2(0, 0);
            rig.gravityScale = 2;
            rig.AddForce(transform.up * speed * shooting.touchPercent);

            //rotate as the bullet falls
            syncRotation = false;
            Invoke("enableRotation", 0.03f);

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
        if (!inBounds) // && weapon != "Mini Rockets")
            transform.gameObject.SetActive(false);

        //sync bullet rotation with how it travels through the air
        if (syncRotation == true && weapon != "Soul Axes")
           transform.up = rig.velocity.normalized;

        //whirl method for mini Rocket
        /*if (syncRotation == true && weapon == "Mini Rockets") {
            rig.gravityScale = 0;            
            rig.AddForce(transform.right * rocketCentripetalForce);
        }*/
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

                //Deliver a knockback force only to light enemies
                if (col.transform.GetComponent<Rigidbody2D>().mass < 2f)
                    StartCoroutine(knockback(col, knockbackForce));
                else {
                    dmgPopup(cannonBallDmg, col);
                    gameObject.SetActive(false);             
                }   
            }
            
            if (weapon == "Soul Axes" && enoughEnemiesHit == false)
            {
                enoughEnemiesHit = true;      
                
                //add hp-dependent dmg code here 
                dmgPopup(soulAxeDmg, col); 
                gameObject.SetActive(false);
            }            

            if (weapon == "Fireballs" && enoughEnemiesHit == false) {
                enoughEnemiesHit = true;
                
                //call a method to light the enemy on fire
                col.transform.GetComponent<Enemy_Health>().activateFire();
                dmgPopup(fireContactDmg, col);
                gameObject.SetActive(false);
            }

            if (weapon == "Mini Rockets" && enoughEnemiesHit == false) {
                enoughEnemiesHit = true;
                
                //stop whirling 
                syncRotation = false;
                collider.enabled = false;
                rig.gravityScale = 0;
                rig.velocity = new Vector2(0, 0);

                //turn on explosion
                explosion.setOffExplosion();
                dmgPopup(rocketDmg, col);
            }
        }

        //if the bullet collides with the hill
        if (col.gameObject.layer == 15)
        {
            //turn off rotation and the collider
            syncRotation = false;
            collider.enabled = false;

            //freeze in place
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            //reset enemies hit in a row
            enemiesHitSpree = 0;

            //Return that you missed the shot
            state.missedShot = true;

            //fade out over time or play an animation where the bullet breaks
            if (gameObject.activeSelf == true && gameObject.tag == "Fading bullet")
                StartCoroutine(fadingBullet());

            else if (gameObject.activeSelf == true && gameObject.tag == "Animated bullet")
                StartCoroutine(animatedToBreakBullet());

            else if (weapon == "Mini Rockets") {
                explosion.setOffExplosion();
            }

            else
                gameObject.SetActive(false);
        }
    }

    //slowly fade this bullet tills its invisible (called for some bullets that hit the ground)
    private IEnumerator fadingBullet() {
        byte alpha = 255;
        while (alpha > 5) {
            alpha -= 5;
            renderer.color = new Color32(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);        
    }

   
    //play bullet breaking animation and then make the bullet disappear
    private IEnumerator animatedToBreakBullet() {

        //play animation right side up always
        transform.rotation = Quaternion.Euler(0, 0, 180f);
        animator.enabled = true;
        yield return new WaitForSeconds(0.583f);
        
        //stop animation and reset rotation after the animation is complete
        animator.enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, 90);

        gameObject.SetActive(false);        
    }

    //sync rotation with movement (called with a slight delay)
    private void enableRotation() 
    {     
        syncRotation = true;

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

    //Call for a popup above the enemy hit (default white color)
    public void dmgPopup(int dmg, Collider2D col) 
    {
        //show the dmg recieved in a text popup
        Enemy_Health e = col.gameObject.transform.GetComponent<Enemy_Health>();
        e.floatText((dmg * dmgMultiplier).ToString(), Color.white);

        specialShotsAndDmg(dmg, col, e);
    }
    
    //Call for a popup above the enemy hit in a specified color
    public void dmgPopup(int dmg, Collider2D col, Color color) 
    {
        //show the dmg recieved in a text popup
        Enemy_Health e = col.gameObject.transform.GetComponent<Enemy_Health>();
        e.floatText((dmg * dmgMultiplier).ToString(), color);

        specialShotsAndDmg(dmg, col, e);
    }

    //Popups for special shots and do dmg to the enemy
    private void specialShotsAndDmg(int dmg, Collider2D col, Enemy_Health e) 
    {
        //describe special shots in a text popup
        float slope = Mathf.Abs(rig.velocity.y / rig.velocity.x);
        enemiesHitSpree++;

        if (slope > 2f)
            e.floatText("Steep shot", new Color32(28, 75, 123, 255));

        else if (enemiesHitSpree % 4 == 0)
            e.floatText("Bullseye", new Color32(28, 75, 123, 255));

        else {}

        //dmg that enemy and then turn off the bullet
        col.gameObject.transform.GetComponent<Enemy_Health>().hp -= dmg * dmgMultiplier;
    }

    //configure general and specific bullet settings based on the weaponType
    private void configureBullet()
    {
        if (weapon == "Arrows") {
            enemiesPierced = 0;
            enemiesToBePierced = UnityEngine.Random.Range(1, 3);
        }

        if (weapon == "Cannon Balls") {
            float xForce = UnityEngine.Random.Range(20, 30);
            float yForce = UnityEngine.Random.Range(200, 300);
            knockbackForce = new Vector2(xForce, yForce);
        }

        if (weapon == "Soul Axes") {
            float slopeOfDmg = (soulAxeDmgBound.x - soulAxeDmgBound.y) / (float) health.maxHp;
            soulAxeDmg = Mathf.RoundToInt(slopeOfDmg * health.hp + soulAxeDmgBound.y);
        }

        if (weapon == "Mini Rockets")  {
            //get access to the rocketExplosion script associated with the rocket
            explosion = transform.GetChild(0).GetComponent<rocketExplosion>();
            explosion.configureExplosion();

            //make the rocket visible (since it's invisible after its last explosion)
            transform.GetComponent<PolygonCollider2D>().enabled = true;
            transform.GetComponent<SpriteRenderer>().enabled = true;
        }

        if (weapon == "Cannon Balls") {
            transform.GetComponent<CircleCollider2D>().enabled = true;
            transform.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    //Add knockback effect to an enemy
    private IEnumerator knockback(Collider2D col, Vector2 force)
    {
        Rigidbody2D enemyRig = col.transform.GetComponent<Rigidbody2D>();

        //Make the boulder invisible
        transform.GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponent<SpriteRenderer>().enabled = true;

        //Disable velocity only if the enemy is grounded and not already in the air from knockback
        if (enemyRig.gravityScale == 0)
            enemyRig.velocity = new Vector2(0, 0);

        //Add a knockback force
        enemyRig.AddForce(force);

        //Re-enable gravity for the enemy, albeit with a buffer effect so it can get off the ground 
        yield return new WaitForSeconds(0.06f);
        enemyRig.gravityScale = 1;

        //Turn on the enemy's ground collider if it has one (to physically land/collide with the ground)
        foreach(Transform child in col.transform) {
            if (child.CompareTag("Ground Collider"))
                child.gameObject.SetActive(true);
        }

        dmgPopup(cannonBallDmg, col);
        gameObject.SetActive(false);
    }
} 

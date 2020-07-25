using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_projectile : MonoBehaviour
{
    private Vector2 dir;
    private float randomX;
    private float randomY;

    [HideInInspector]
    public bool setupOnce;

    private AudioSource audioSource;
    private Rigidbody2D rig;
    private SpriteRenderer renderer;
    private Collider2D collider;
    public float volume;

    public GameObject tower;
    public float speed = 5f;
    public float variance = 2f;
    public Vector2 xRandomness;
    public Vector2 yRandomness;
    public float dmgMultiplier;

    void Awake() 
    {
        //define components
        audioSource = transform.GetComponent<AudioSource>();
        rig = transform.GetComponent<Rigidbody2D>();
        renderer = transform.GetComponent<SpriteRenderer>();
        collider = transform.GetComponent<Collider2D>();
    }

    void Update()
    {
        //When the orb is shot out
        if (setupOnce)
        {   
            //perform this setup only once
            setupOnce = false;

            //reset color, collider and gravity
            renderer.color = new Color32(255, 255, 255, 255);            
            collider.enabled = true;

            //assign movement in the air based of which projectile it is
            if (gameObject.tag == "Reaper projectile")
                ReaperProjectile();
            if (gameObject.tag == "Ogre projectile")
                OgreProjectile();
        }
    }

    void OnTriggerEnter2D(Collider2D col) {

        //collides with the hill
        if (col.gameObject.layer == 15) {
            collider.enabled = false;
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            gameObject.SetActive(false);
        }

        //connected with the tower, cannon or player
        if (col.gameObject.layer == 9)
        {
            if (gameObject.tag == "Reaper projectile") {
                health.hp -= Enemy_Health.R3Dmg * dmgMultiplier;
                Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            }

            if (gameObject.tag == "Ogre projectile") {
                health.hp -= Enemy_Health.ogreDmg * dmgMultiplier;
                Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            }
            
            gameObject.SetActive(false);
        }
    }

    //Fade a projectile over time when it hits the ground
    private IEnumerator fade() {
        byte alpha = 255;
        while (alpha > 12) {
            alpha -= 12;
            renderer.color = new Color32(255, 26, 26, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);        
    }

    //Setup for Reaper's projectile 
    private void ReaperProjectile() {

        //set direction and velocity of orb
        dir = tower.transform.position - transform.position;
        dir = new Vector2(dir.x / 1.4f, 0);

        //add randomness to the direction, with a bias towards undershooting
        randomX = UnityEngine.Random.Range(-2.5f * variance, 2 * variance) / 3f;
        if (randomX >= -1.7f && randomX <= -0.83f)
        {
            randomX = UnityEngine.Random.Range(-2.5f * variance, 0) / 3f;
        }
        randomY = UnityEngine.Random.Range(-variance, variance) / 3f;

        rig.gravityScale = 3;
        rig.velocity = dir * speed + new Vector2(dir.x * randomX, dir.y * randomY);

        //play cast orb sound effect
        audioSource.PlayOneShot(Manage_Sounds.Instance.R1Attack, volume * Manage_Sounds.soundMultiplier);
    }

    //Setup for Ogre's projectile
    private void OgreProjectile() {

        //set direction and velocity of boulder
        dir = tower.transform.position - transform.position;
        dir = new Vector2(dir.x / 1.4f, 0);

        //add randomness to the direction the boulder is shot in
        randomX = UnityEngine.Random.Range(xRandomness.x, xRandomness.y);
        randomY = UnityEngine.Random.Range(yRandomness.x, yRandomness.y);

        rig.gravityScale = 2;
        rig.AddForce(new Vector2(randomX, randomY));

        //play throw boulder sound effect
        audioSource.PlayOneShot(Manage_Sounds.Instance.R1Attack, volume * Manage_Sounds.soundMultiplier);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketExplosion : MonoBehaviour
{
    private player_bullet rocket;
    private CircleCollider2D explosionCol;
    private GameObject explosionParticles;

    public float rocketTimer = 3;
    private float explosionRate = 10f;
    private float maxExplosionRadius = 2.7f;

    void Awake() 
    {
        //get access to the parent rocket, explosion particles, and explosion collider
        rocket = transform.parent.GetComponent<player_bullet>();        
        explosionParticles = transform.parent.transform.GetChild(1).gameObject;
        explosionCol = transform.GetComponent<CircleCollider2D>();

        configureExplosion();
    }

    void Update() 
    {
        //If the rocketTimer has been set at 2 seconds, start the count down
        if (rocketTimer > 0 && rocketTimer <= 2) {
            rocketTimer -= Time.deltaTime;

            //grow the explosion collider
            if (explosionCol.radius < maxExplosionRadius)
                explosionCol.radius +=  explosionRate * Time.deltaTime;

            //turn it off when its reached its max
            else if (explosionCol.radius >= maxExplosionRadius)
                explosionCol.enabled = false;
        }

        //Once the rocketTimer countdown is over, reset this script for the next time the rocket's launched
        else if (rocketTimer <= 0) {
            configureExplosion();
            rocket.gameObject.SetActive(false);
        }
    }

    public void setOffExplosion() 
    {
        //disable the parent rocket
        rocket.transform.GetComponent<PolygonCollider2D>().enabled = false;
        rocket.transform.GetComponent<SpriteRenderer>().enabled = false;        

        //turn on the explosion particles
        explosionParticles.SetActive(true);

        //grow the rocket's area collider
        explosionCol.enabled = true;
        rocketTimer = 1.99f;
    }

    public void configureExplosion() 
    {     
        rocketTimer = 3;
        explosionParticles.SetActive(false);
        explosionCol.radius = 0.5f;
        explosionCol.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
       //if the rocket collider touches an enemy, do dmg
        if (col.gameObject.layer == 8) 
            StartCoroutine(dmgEnemy(col));
    }

    //do the dmg with a slight explosion delay 
    private IEnumerator dmgEnemy(Collider2D col) {
        yield return new WaitForSeconds(0.3f);
        rocket.dmgPopup(player_bullet.rocketExplosionDmg, col, new Color32(99, 22, 9, 255));
    }
}

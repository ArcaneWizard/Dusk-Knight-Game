using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Sounds : MonoBehaviour
{
    public AudioClip enemyHit;



    //Always refer to clips from here so they are all in one place + ez to swap out for testing :D    I outlined the system I followed below:







    void example() {

        //Add to some script's Start Method:
        gameObject.AddComponent<AudioSource>();

        //Play sound code:
        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.enemyHit);  

    }

    //Btw, all the projectiles disable immediately upon collision so playing sounds on their scripts is useless.
    //You'll have to add sounds to the player or enemy_health script
}

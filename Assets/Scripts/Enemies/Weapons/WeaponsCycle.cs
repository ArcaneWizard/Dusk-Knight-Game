using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCycle : MonoBehaviour
{

    [Space(10)]
    [Header("Objects")]
    public Transform Projectile_Group;

    public static int index = 0;
    private List<Transform> projectiles = new List<Transform>();
    private Animator anim;

    [Space(10)]
    [Header("Characteristics")]
    public Vector2 reload;
    
    [HideInInspector]
    public bool reloadAttack;
    private float progress;

    void Start()
    {
        //define all components
        anim = transform.GetComponent<Animator>();

        //add all weapon projectiles to a list
        foreach (Transform projectile in Projectile_Group.transform) {
            projectiles.Add(projectile);
        }
        
        //temporary ogre shooting method before its animated shooting
        if (gameObject.tag == "Enemy 5") {
           reloadAttack = true;
           StartCoroutine(resetAttack());
           StartCoroutine(ogreShoot());
        }
    }

    //temporary ogre shooting method before its animated shooting
    IEnumerator ogreShoot() {
        yield return new WaitForSeconds(0.5f);

        if (reloadAttack == false) {
            reloadAttack = true;
            StartCoroutine(resetAttack());
            ogreShot();
        }

        StartCoroutine(ogreShoot());
    }
    
    void Update()
    {   
        //Reaper casts orbs 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 3 floating"))
        {
            //Shoot once per animation cycle
            progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.45f && progress <= 0.55f && reloadAttack == false)
            {
                reloadAttack = true;
                StartCoroutine(reaperShot());
                StartCoroutine(resetAttack());  
            }
        }

    }

    //wait for a while till the next shot
    private IEnumerator resetAttack()
    {
        float r = UnityEngine.Random.Range(reload.x, reload.y);
        yield return new WaitForSeconds(r);
        reloadAttack = false;
    }

    //sync Reaper animation and shot
    private IEnumerator reaperShot() {

        //start cast orb animation
        anim.SetBool("Attack", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("Attack", false);

        //use the next available bullet
        index = ++index % projectiles.Count;

        //Get bullet and specify target
        projectiles[index].transform.position = transform.position; 
        projectiles[index].GetComponent<enemy_projectile>().setupOnce = true;   
        projectiles[index].gameObject.SetActive(true);
    }

    //sync Ogre animation and shot
    private void ogreShot() {

        //use the next available bullet
        index = ++index % projectiles.Count;

        //Get bullet and specify target
        projectiles[index].transform.position = transform.position; 
        projectiles[index].GetComponent<enemy_projectile>().setupOnce = true;   
        projectiles[index].gameObject.SetActive(true);
    }
}

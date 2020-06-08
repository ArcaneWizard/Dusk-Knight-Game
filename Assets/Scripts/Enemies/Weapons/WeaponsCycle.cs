using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCycle : MonoBehaviour
{
    private Animator anim;
    private GameObject bullet;

    public GameObject darkReaperOrb;
    public GameObject Player;

    public Vector2 darkOrbReload;
    
    private float progress;
    public bool reloadAttack = false;

    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }
    
    void Update()
    {   
        //Dark reaper casts orbs 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 3 floating"))
        {
            //Shoot once per animation cycle
            progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.45f && progress <= 0.55f && reloadAttack == false)
            {
                reloadAttack = true;
                StartCoroutine(darkOrbAttack());
                StartCoroutine(resetAttack(darkOrbReload));  
            }
        }
    }

    //wait till next shot
    private IEnumerator resetAttack(Vector2 range)
    {
        float reload = UnityEngine.Random.Range(range.x, range.y);
        yield return new WaitForSeconds(reload);
        reloadAttack = false;
    }

    //sync darkOrb animation and shot
    private IEnumerator darkOrbAttack() {

        //start cast orb animation
        anim.SetBool("Attack", true);
        yield return new WaitForSeconds(0.3f);
        anim.SetBool("Attack", false);

        //Create bullet and specify target
        bullet = Instantiate(darkReaperOrb, transform.position, Quaternion.identity);
        bullet.transform.GetComponent<Orb>().tower = Player;        
    }
}

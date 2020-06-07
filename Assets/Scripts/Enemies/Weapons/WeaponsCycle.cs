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
    
    private float progress;
    private bool attacked = false;

    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }
    
    void Update()
    {   
        //Dark reaper casts orbs 
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 3 Throwing"))
        {
            //Shoot once per animation cycle
            progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.45f && progress <= 0.55f && attacked == false)
            {
                attacked = true;

                //Create bullet and specify target
                bullet = Instantiate(darkReaperOrb, transform.position, Quaternion.identity);       
                bullet.transform.GetComponent<Orb>().tower = Player;
                StartCoroutine(resetAttack());
            }
        }
    }

    private IEnumerator resetAttack()
    {
        yield return new WaitForSeconds(0.4f);
        attacked = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCycle : MonoBehaviour
{
    private Animator anim;

    private bool attacked = false;

    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }
    
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 3 Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.45f && progress <= 0.55f && attacked == false)
            {
                attacked = true;
                StartCoroutine(resetAttack());
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 1 Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.25f && progress <= 0.35f && attacked == false)
            {
                attacked = true;
                StartCoroutine(resetAttack());
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ogre Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.25f && progress <= 0.35f && attacked == false)
            {
                attacked = true;
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

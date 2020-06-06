using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsCycle : MonoBehaviour
{
    public List<GameObject> ammo;
    private int cycle = 0;
    private bool counter = false;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }
    
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 3 Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.45f && progress <= 0.55f && counter == false)
            {
                counter = true;
                StartCoroutine(check());

                ammo[cycle].transform.position = transform.GetChild(2).transform.position;
                ammo[cycle].transform.GetComponent<Orb>().switchOrbs = true;
                ammo[cycle].SetActive(true);

                cycle++;
                cycle = cycle % ammo.Count;
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reaper 1 Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.25f && progress <= 0.35f && counter == false)
            {
                counter = true;
                StartCoroutine(check());

                ammo[cycle].transform.position = transform.GetChild(1).transform.position;
                ammo[cycle].transform.GetComponent<Orb>().switchOrbs = true;
                ammo[cycle].SetActive(true);

                cycle++;
                cycle = cycle % ammo.Count;
            }
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Ogre Throwing"))
        {
            float progress = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (progress >= 0.25f && progress <= 0.35f && counter == false)
            {
                counter = true;
                StartCoroutine(check());

                ammo[cycle].transform.position = transform.GetChild(1).transform.position;
                ammo[cycle].transform.GetComponent<Boulder>().switchBoulders = true;
                if (gameObject.layer == 9 && gameObject.transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x)
                    ammo[cycle].transform.GetComponent<Boulder>().side = "left";

                ammo[cycle].SetActive(true);

                cycle++;
                cycle = cycle % ammo.Count;
            }
        }
    }

    private IEnumerator check()
    {
        yield return new WaitForSeconds(0.4f);
        counter = false;
    }
}

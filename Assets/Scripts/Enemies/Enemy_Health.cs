using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector]
    public int hp = 1;

    private int orc = 70;
    private int ogre = 120;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    Animator animator;
    private bool death = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (gameObject.layer == 8)
            hp = orc;

        if (gameObject.layer == 9)
            hp = ogre;

        if (gameObject.layer == 11)
            hp = goblin;

        if (gameObject.layer == 19)
            hp = reaper_1;

        if (gameObject.layer == 20)
            hp = reaper_2;

        if (gameObject.layer == 21)
            hp = reaper_3;

        animator = transform.GetComponent<Animator>();
    }

    void Update()
    {
        if (hp <= 0 && death == false)
        {
            StartCoroutine(checkDeath());
        }
        if (death == true && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.85f)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator checkDeath()
    {
        animator.SetBool("Dead", true);
        yield return new WaitForSeconds(0.01f); 
        death = true;
    }
}

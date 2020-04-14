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
            checkDeath();
            death = true;
        }
    }

    private IEnumerator fade()
    {
        yield return new WaitForSeconds(0.1f);
        /*while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("rip");
        }*/

        float scale = 0.93f;
        float rotSpeed = 25f;

        for (int i = 17; i >= 0; i--)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)(15*i));
            transform.localScale *= scale;
            transform.Rotate(new Vector3(0, 0, rotSpeed));

            yield return new WaitForSeconds(0.05f); 
        }
    }

    private void checkDeath()
    {
        animator.SetBool("Dead", true);

        transform.GetComponent<Rigidbody2D>().gravityScale = 0;
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        if (gameObject.layer == 21)
            transform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(fade());
    }
}

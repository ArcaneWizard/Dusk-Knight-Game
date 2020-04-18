using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector]
    public int hp = 1;
    private int lastHP;

    private int orc = 120;
    private int ogre = 70;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    public static float orc_speed = 0.4f;
    public static float ogre_speed = 0.5f;
    public static float goblin_speed = 0.9f;
    public static float R1_speed = 0.5f;
    public static float R2_speed = 0.7f;

    Animator animator;
    private bool death = false;

    private float ogScaleX;
    private float ogScaleY;

    public bool deploy = true;
    public bool poison = false;
    public bool isPoisoned = false;
    public bool iced = false;
    public bool isIced = false;
    public int isIcedWhileIced = 0;
    public int isIcedWhileIcedCheck = 0;

    // Start is called before the first frame update
    void Awake()
    {
        setHP();
        ogScaleX = transform.localScale.x;
        ogScaleY = transform.localScale.y;
        animator = transform.GetComponent<Animator>();
    }

    public void setHP()
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

        lastHP = hp;
        death = false;

        if (gameObject.tag == "Ranged Shooter")
            transform.localPosition = new Vector2(0, 0);
    }

    void Update()
    {
        if (poison == true && isPoisoned == false)
        {
            poison = false;
            isPoisoned = true;
            StartCoroutine(poisoned());
        }

        if (iced == true)
        {
            iced = false;
            isIced = true;
            isIcedWhileIced++;
            StartCoroutine(frozen());
        }

        if (hp <= 0 && death == false)
        {
            checkDeath();
            death = true;
        }

        if (lastHP != hp)
        {
            lastHP = hp;
            if (isPoisoned == false)
               StartCoroutine(alter());
        }
    }

    private IEnumerator frozen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        float sign = 1;
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            sign = -1;

        setSpeed(sign, 0.5f);

        transform.GetComponent<Animator>().enabled = false;

        if (gameObject.layer == 8)
            transform.GetComponent<Orc>().enabled = false;

        if (gameObject.layer == 9)
            transform.GetComponent<Ogre>().enabled = false;

        if (gameObject.layer == 11)
            transform.GetComponent<Goblin>().enabled = false;

        if (gameObject.layer == 19)
            transform.GetComponent<Reaper_1>().enabled = false;

        if (gameObject.layer == 20)
            transform.GetComponent<Reaper_2>().enabled = false;

        if (gameObject.layer == 21)
            transform.GetComponent<Reaper_3>().enabled = false;

        yield return new WaitForSeconds(1.5f);
        isIcedWhileIcedCheck++;

        if (isIcedWhileIcedCheck == isIcedWhileIced)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetComponent<Animator>().enabled = true;

            setSpeed(sign, 1);
            isIced = false;
        }
    }

    private void setSpeed(float sign, float multiplier)
    {

        if (gameObject.layer == 8)
        {
            transform.GetComponent<Orc>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(orc_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 9)
        {
            transform.GetComponent<Ogre>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(ogre_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 11)
        {
            transform.GetComponent<Goblin>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(goblin_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 19)
        {
            transform.GetComponent<Reaper_1>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(R1_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 20)
        {
            transform.GetComponent<Reaper_2>().enabled = true;
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(R2_speed * sign * multiplier, 0);
        }

        if (gameObject.layer == 21)
        {
            transform.GetComponent<Reaper_3>().enabled = true;
        }
    }

    private IEnumerator poisoned()
    {
        for (int i = 0; i < 5; i++)
        {
            if (death == false)
            {
                gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(56, 219, 143, 255);
                yield return new WaitForSeconds(0.22f);
                hp -= 10;
                gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                yield return new WaitForSeconds(0.8f);
            }
        }
        isPoisoned = false;
    }

    private IEnumerator fade()
    {
        yield return new WaitForSeconds(0.1f);
        //Un-ice and unpoison enemy b4 it fades away
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetComponent<Animator>().enabled = true;
        setSpeed(1, 0);
        isIced = false;

        isPoisoned = false;

        float scale = 0.93f;
        float rotSpeed = 25f;

        for (int i = 17; i >= 0; i--)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)(15*i));
            transform.localScale *= scale;
            transform.Rotate(new Vector3(0, 0, rotSpeed));

            yield return new WaitForSeconds(0.05f); 
        }

        isIcedWhileIced = isIcedWhileIcedCheck;

        if (gameObject.tag != "Ranged Shooter")
            gameObject.SetActive(false);
        else
            gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void undoFade()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }

    private void checkDeath()
    {
        animator.SetBool("Dead", true);

        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().gravityScale = 0;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        if (gameObject.layer == 21)
            transform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(fade());
    }

    private IEnumerator alter()
    {
        gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(245, 0, 0, 255);
        yield return new WaitForSeconds(0.1f);
        gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
}

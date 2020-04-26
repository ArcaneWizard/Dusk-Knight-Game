using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static int playerHP;
    public static int maxPlayerHP;

    private BoxCollider2D boxy;
    private BoxCollider2D boxy2;

    private int stage = 1;
    public static int OrcDmg = 50;
    public static int GobDmg = 20;
    public static int R2Dmg = 25;
    public static int R3Dmg = 8;
    public static int R1Dmg = 10;
    public static int OgreDmg = 12;

    public static int arrow = 40;
    public static int bullet = 15;
    public static int CB = 40;
    public static float flame = 1.3f;
    public static float grenade = 30;
    public static int potion = 40;

    private Image hp;
    private GameObject head;
    private int applyTowerChangeOnce = 1;
    public bool hpBoost = false;

    private bool diedOnce;

    // Start is called before the first frame update
    void Start()
    {
        maxPlayerHP = 1000;
        playerHP = maxPlayerHP;

        hp = GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Image>();
        head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        stage = PlayerPrefs.GetInt("Tower") + 1;
        hp.fillAmount = (float)playerHP / maxPlayerHP;
        hp.transform.parent.transform.GetChild(1).transform.GetComponent<Text>().text = stage.ToString();

        if (stage == 1)
            transform.GetChild(2).gameObject.SetActive(true);

        if (stage >= 2 && applyTowerChangeOnce != stage)
        {
            transform.GetChild(stage).gameObject.SetActive(false);
            transform.GetChild(stage + 1).gameObject.SetActive(true);

            applyTowerChangeOnce = stage;
            maxPlayerHP += 1000;
            playerHP += 1000;
        }

        if (stage == 1)
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 1.05f);
        if (stage == 2)
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 1.63f);
        if (stage == 3)
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 2.18f);
        if (stage == 4)
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 2.18f);

        if (hpBoost == true)
        {
            playerHP += maxPlayerHP / 2;
            hpBoost = false;
        }

        if (playerHP > maxPlayerHP)
            playerHP = maxPlayerHP;

        if (playerHP <= 0 && diedOnce == false)
        {
            diedOnce = true;
            GameObject.FindGameObjectWithTag("Game Over").transform.GetChild(4).gameObject.SetActive(true);
            StartCoroutine(reset_level());
        }
    }

    private IEnumerator reset_level()
    {
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene(0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 23 || col.gameObject.layer == 24)
        {
            if (col.gameObject.tag == "Witch orb")
                playerHP -= R3Dmg;

            if (col.gameObject.tag == "Reaper orb")
                playerHP -= R1Dmg;

            if (col.gameObject.tag == "Boulder")
                playerHP -= OgreDmg;
        }
    }

    /*public void Reset()
    {
        maxPlayerHP = 700;
        playerHP = maxPlayerHP;

        boxy.offset = new Vector2(0.02191818f, -0.5f);
        boxy.size = new Vector2(2.344175f, 1.9f);
        boxy2.offset = new Vector2(0.03646278f, 0.9f);
        boxy2.size = new Vector2(2.458501f, 0.7449135f);
        transform.GetChild(2).GetChild(5).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(6).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(7).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
    }

    void Upgrade()
    {
        if (stage == 2)
        {
            boxy.offset = new Vector2(0.02191818f, -0.3f);
            boxy.size = new Vector2(2.344175f, 2.5f);
            boxy2.offset = new Vector2(0.03646278f, 1.4f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
            maxPlayerHP += 100;
            playerHP = maxPlayerHP;
        }
        if (stage == 3)
        {
            boxy.offset = new Vector2(0.02191818f, 0f);
            boxy.size = new Vector2(2.344175f, 3f);
            boxy2.offset = new Vector2(0.03646278f, 2f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
            maxPlayerHP += 100;
            playerHP = maxPlayerHP;
        }
        if (stage == 4)
        {
            boxy.offset = new Vector2(0.02191818f, -0.5f);
            boxy.size = new Vector2(2.344175f, 1.9f);
            boxy2.offset = new Vector2(0.03646278f, 0.9f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(5).gameObject.SetActive(true);
            transform.GetChild(2).GetChild(6).gameObject.SetActive(true);
            transform.GetChild(2).GetChild(7).gameObject.SetActive(true);
            maxPlayerHP += 100;
            playerHP = maxPlayerHP;
        }
    }*/
}
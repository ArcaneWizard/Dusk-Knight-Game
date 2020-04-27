using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Vector2 dir;
    [HideInInspector]
    public bool switchBoulders = false;
    public string side = "right";
    private float multiplier;

    // Start is called before the first frame update
    void Update()
    {
        if (switchBoulders == true)
        {
            multiplier = UnityEngine.Random.Range(1f, 1.5f);

            if (side == "left")
                dir = new Vector2(200, 300);
            else
                dir = new Vector2(-200, 300);
            transform.GetComponent<Rigidbody2D>().AddForce(dir * multiplier);
            switchBoulders = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 22)
            gameObject.SetActive(false);

        if (col.gameObject.layer == 10)
        {
            if (gameObject.tag == "Witch orb")
                Health.playerHP -= Health.R3Dmg;

            if (gameObject.tag == "Reaper orb")
                Health.playerHP -= Health.R1Dmg;

            if (gameObject.tag == "Boulder")
                Health.playerHP -= Health.OgreDmg;

            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Move()
    {
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(speed, 0);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Range activation"))
        {
            animator.SetBool("Attack", true);
            rig.velocity = new Vector2(0, 0);
        }
    }
}

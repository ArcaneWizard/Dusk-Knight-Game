using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    private float speed = 1f;

    void Start()
    {

    }

    
    void Update()
    {
        if(Mathf.Abs(transform.position.x)<9.0f || Mathf.Abs(transform.position.y) < 5.7f)
        {
            transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.name == "Enemies")
        {
            collision.gameObject.transform.GetComponent<Enemy_Health>().hp -= 100;
        }
    }
}

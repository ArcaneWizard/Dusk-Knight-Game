using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    public float speed;
    public float fuse;

    void Start()
    {
        Invoke("DestroyGrenade", fuse);
    }

    
    void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);
        
    }

    void DestroyGrenade()
    {

        Destroy(gameObject);
    }

}

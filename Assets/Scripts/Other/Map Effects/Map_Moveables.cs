using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Moveables : MonoBehaviour
{
    private Rigidbody2D rig;
    private float largeCloudSpeed = 0.5f;
    private float largeCloudSpeed2 = 0.4f;
    private float smallCloudSpeed = 0.1f;

    private bool largeCounter;
    private bool largeCounter2;
    private bool smallCounter;

    // Start is called before the first frame update
    void Start()
    {
        rig = transform.GetComponent<Rigidbody2D>();

        if (gameObject.name == "large cloud 2")
            StartCoroutine(largeCloud2());
        if (gameObject.name == "large cloud")
            StartCoroutine(largeCloud());
        if (gameObject.name == "small cloud")
            rig.velocity = new Vector2(smallCloudSpeed, 0);
    } 
    
    private IEnumerator largeCloud()
    {
        float r = Random.Range(1, 3);
        yield return new WaitForSeconds(r);
        transform.localPosition = new Vector2(-11.5f, transform.localPosition.y);
        rig.velocity = new Vector2(largeCloudSpeed, 0);
        largeCounter = false;
    }
    private IEnumerator largeCloud2()
    {
        float r = Random.Range(5, 7);
        yield return new WaitForSeconds(r);
        transform.localPosition = new Vector2(-11.5f, transform.localPosition.y);
        rig.velocity = new Vector2(largeCloudSpeed2, 0);
        largeCounter2 = false;
    }

    private IEnumerator smallCloud()
    {
        yield return new WaitForSeconds(0.05f);
        smallCloudSpeed *= -1;
        rig.velocity = new Vector2(smallCloudSpeed, 0);
        yield return new WaitForSeconds(1f);
        smallCounter = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "large cloud" && transform.localPosition.x > 11 && largeCounter == false)
        {
            StartCoroutine(largeCloud());
            largeCounter = true;
        }
        if (gameObject.name == "large cloud 2" && transform.localPosition.x > 11 && largeCounter2 == false)
        {
            StartCoroutine(largeCloud2());
            largeCounter2 = true;
        }
        if (gameObject.name == "small cloud" && smallCounter == false && (transform.localPosition.x >= 0.4f || transform.localPosition.x <= -0.2f))
        {
            StartCoroutine(smallCloud());
            smallCounter = true;
        }
    }
}

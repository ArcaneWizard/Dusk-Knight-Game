using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flame : MonoBehaviour
{

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 1;
        }
    }
}

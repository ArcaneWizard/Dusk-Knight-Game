using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale_Camera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float width = 5f / 720f * 1280f;
        Camera.main.orthographicSize = width / Screen.width * Screen.height;        
    }
}

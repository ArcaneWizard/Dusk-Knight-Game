using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    public float magnitude;

    void Start() 
    {
        originalPos = transform.localPosition;       
    }

    //camera shake
    public IEnumerator Shake (float duration) 
    { 
        float elapsed = 0.0f;

        //shake lasts for a specified duration
        while (elapsed < duration) 
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        //After the shake's over, reset camera position
        transform.localPosition = originalPos;
    }
}

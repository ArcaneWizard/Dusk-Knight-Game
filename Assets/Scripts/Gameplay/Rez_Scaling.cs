using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rez_Scaling : MonoBehaviour
{
    public Camera mainCamera;
    public Transform rightBound;
    public Transform InGameUI;
    
    private float rez;
    private float zoom;
    private float canvasZoom;

    private Vector3 initialCamPos;
    private Vector3 bottomRight;
    private float distanceFromRight;

    void Awake() {
        scaleGameForThisResolution();
    }

    void Update() 
    {
        scaleGameForThisResolution();
    }

    private void scaleGameForThisResolution() 
    {  
        //get the screen's width to height ratio
        rez = (float) Screen.width / (float) Screen.height;
        
        //updateCanvasScale();
        updateCameraZoom();
        anchorRightEdgeOfScreen();
    }

    //update the scale of all in game UI elements for this screen resolution
    private void updateCanvasScale() 
    {
        canvasZoom = canvasAlgorithm(); 
        foreach (Transform child in InGameUI) {
            child.transform.GetComponent<RectTransform>().localScale = new Vector2(canvasZoom, canvasZoom);
        }
    }

    //find the new camera zoom for this screen resolution
    private void updateCameraZoom() 
    {
        zoom = zoomAlgorithm();
        mainCamera.orthographicSize = zoom;
    }

    //algorithm that determines the camera zoom based off screen resolution
    private float zoomAlgorithm() 
    {
        float z = 6.5f;

        if (rez < 2.22f) {
            z += Mathf.Min(2.22f - rez, 2.22f - 1.77f) * 2f;
        }

        if (rez < 1.77f) {
            z += Mathf.Min(1.77f - rez, 1.77f - 1.66f) * 0.3f / 0.11f;
        }

        if (rez < 1.66f) {
            z += Mathf.Min(1.66f - rez, 1.66f - 1.40f) * 5f;
        }

        if (rez < 1.40f) {
            z += (1.40f - rez) * 0.5f / 0.066f;
        }

        return z;
    }

    //algorithm that determines the canvas elements' scale based off screen resolution
    private float canvasAlgorithm() 
    {
        float c = 1f;
        
        if (rez > 1.68f) {
            c -= (rez - 1.68f) * 0.1f / (2.222f - 1.68f);
        }

        return c;
    }

    //position camera's right edge to the same world position every time
    private void anchorRightEdgeOfScreen() 
    {
        //get the camera's position
        initialCamPos = mainCamera.transform.position;

        //get the world coordinate distance between the camera's left edge and where its left edge should be
        bottomRight = mainCamera.ViewportToWorldPoint(new Vector2(1, 0));
        distanceFromRight = rightBound.position.x - bottomRight.x;

        //teleport the camera so that its left edge is where it should be (barely to the right of where enemies spawn)
        mainCamera.transform.position = new Vector3(initialCamPos.x + distanceFromRight, initialCamPos.y, initialCamPos.z); 
    }
}

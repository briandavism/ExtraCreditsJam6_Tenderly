using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

    private float zoom = 10;
    Vector3 newPosition;

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && zoom > 9)
        {
            zoom -= 1;
            Camera.main.orthographicSize = zoom;
            newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.1F);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && zoom < 101)
        {
            zoom += 1;
            Camera.main.orthographicSize = zoom;
        }

    }

}

//https://stackoverflow.com/questions/47203687/unity-2d-mouse-zoom-but-zoom-in-to-location-of-mouse-point
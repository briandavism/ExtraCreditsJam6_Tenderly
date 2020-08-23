using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;

    void Start()
    {
        // A base position we can use to resent the cameras position to.
        ResetCamera = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        // Input Handler logic for various user inputs.
        // On mouse click...
        if (Input.GetMouseButton(0))
        {
            Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            Camera.main.transform.position = Origin - Diference;
        }

        // On right click, reset camera to ResetCamera position.
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = ResetCamera;
        }
    }
}

//https://forum.unity.com/threads/click-drag-camera-movement.39513/


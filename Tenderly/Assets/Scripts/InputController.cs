using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

public class InputController : MonoBehaviour
{
    // Camera Drag
    private Vector3 ResetCamera;
    private Vector3 Origin;
    private Vector3 Diference;
    private bool Drag = false;
    // Camera Zoom
    private float zoom = 10;
    Vector3 newPosition;
    // Mouse hover
    public Grid grid;

    void Start()
    {
        // A base position we can use to resent the cameras position to.
        ResetCamera = Camera.main.transform.position;
    }

    void LateUpdate()
    {
        /************** Input Handler logic for various user inputs. **************/
        // On mouse click, prepare to drag.
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

        // On mouse wheel, adjust the zoom.
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && zoom > 9)
        {
            zoom -= 1;
            Camera.main.orthographicSize = zoom;
            newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.1F);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && zoom < 101)
        {
            zoom += 1;
            Camera.main.orthographicSize = zoom;
        }

        // On mouse hover, get tile info.
        if (Input.GetKeyDown("space"))
        {
            // Used for finding tiles from mouse position input.
            Vector3Int cellPos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            List<Tile> tiles = grid.GetComponent<GridManager>().GetTiles(cellPos);
            for (int i = 0; i < tiles.Count; i++)
            {
                Debug.Log(tiles[i]);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    // Camera Drag
    private Vector3 resetCamera;
    private Vector3 cameraOrigin;
    private Vector3 cameraDiff;
    private bool mouseDrag = false;
    // Camera Zoom
    public float cameraZoom = 16;
    Vector3 cameraNewPosition;
    // Mouse hover
    public Grid grid;
    public Text hoverText;
    private string hoverGroundString = "";
    private string hoverPlantString = "";
    private Tile hoverGroudTile;
    private Tile hoverPlantTile;
    // Tool Selection
    public static string activeTool = "shovel";
    public Button setBucket;
    public Button setShovel;
    public Image BucketGlow;
    public Image ShovelGlow;

    // Water Placement
    public static int waterInventory = 99;
    public Text waterText;

    void Start()
    {
        // A base position we can use to resent the cameras position to.
        resetCamera = Camera.main.transform.position;

        // Initialize hover text to be empty.
        hoverText.text = "";

        // set water inventory
        waterText.text = waterInventory.ToString();

    }

    void Update()
    {
        //Update Water Inventory Text
        waterText.text = waterInventory.ToString();

        if(activeTool == "shovel")
        {
            BucketGlow.color = new Color (1, 1, 1, 0);
            ShovelGlow.color = new Color(1, 1, 1, 1);
        }
        else
        {
            BucketGlow.color = new Color(1, 1, 1, 1);
            ShovelGlow.color = new Color(1, 1, 1, 0);
        }
    }
    void LateUpdate()
    {
        

        
        /************** Input Handler logic for various user inputs. **************/
        // Useful parameters:
        // The Vector3Int corresponding to the mouse position over the grid.
        Vector3Int cellPos = grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // The tiles, ground and plant, that may be on the grid under the mouse.
        List<Tile> tiles = grid.GetComponent<GridManager>().GetTiles(cellPos);

        //Left click to use tool
        if (Input.GetMouseButtonDown(0))
        {
            if (activeTool == "shovel")
            {
                grid.GetComponent<GridManager>().ClearPlants(cellPos);
            }
            else
            {
                grid.GetComponent<GridManager>().PlaceWater(cellPos);
                waterInventory = waterInventory - 1;
            }
        }

        // On middle mouse click, prepare to drag.
        if (Input.GetMouseButton(2))
        {
            cameraDiff = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (mouseDrag == false)
            {
                mouseDrag = true;
                cameraOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            mouseDrag = false;
        }
        if (mouseDrag == true)
        {
            Camera.main.transform.position = cameraOrigin - cameraDiff;
        }

        // On right click, reset camera to ResetCamera position.
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = resetCamera;
        }

        // On mouse wheel, adjust the zoom.
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && cameraZoom > 9)
        {
            cameraZoom -= 1;
            Camera.main.orthographicSize = cameraZoom;
            cameraNewPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = Vector3.Lerp(transform.position, cameraNewPosition, 0.1F);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && cameraZoom < 101)
        {
            cameraZoom += 1;
            Camera.main.orthographicSize = cameraZoom;
        }

        // Hover Text manager: Change text based on where the mouse is over the grid. Format is <Ground>:<Plant>
        // If this tile is a different tile than previous updates, change the hover text and update the tile.
        if (tiles != null && tiles.Count > 0)
        {
            // Only update hoverText when needed.
            bool updateHoverText = false;

            // Check ground tile difference.
            if (tiles[0].name != hoverGroundString)
            {
                hoverGroundString = tiles[0].name;
                updateHoverText = true;
            }

            // Check plant tile if present.
            if (tiles.Count == 2 && tiles[1].name != hoverPlantString)
            {
                hoverPlantString = tiles[1].name;
                updateHoverText = true;
            } else if (tiles.Count < 2 && hoverPlantString != "")
            {
                hoverPlantString = "";
                updateHoverText = true;
            }

            // Update hoverText if needed.
            if (updateHoverText)
            {
                hoverText.text = hoverGroundString + ":" + hoverPlantString;
            }

            // Update Water Text

        }

        // Water Placement: Place Water in the tile under the mouse.
        if (Input.GetKeyDown("space") && waterInventory > 0)
        {
            grid.GetComponent<GridManager>().PlaceWater(cellPos);
            waterInventory = waterInventory - 1;
            waterText.text = waterInventory.ToString();
        }

        // Clear Tile: Clears off plants in the tile, if there are any.
        if (Input.GetKeyDown("backspace"))
        {
            grid.GetComponent<GridManager>().ClearPlants(cellPos);
        }

        // Tool Selector
      
    }
}

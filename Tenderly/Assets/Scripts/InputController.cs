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
    private Vector3 clickStartPos;
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
    public static string activeTool = "bucket";
    public Button setBucket;
    public Button setShovel;
    public Image BucketGlow;
    public Image ShovelGlow;
    public static bool disableTool = false;
    public List<RectTransform> disableMouseOver;
    public RectTransform canvasRT;

    // Water Placement
    public static int waterInventory = 99;
    private int waterSpent = 0;
    public Text waterText;
    private int startingWater = 4;

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
        waterInventory = startingWater + ProgressBar.waterEarned - waterSpent;
        waterText.text = waterInventory.ToString();

        if(activeTool.Equals("shovel", System.StringComparison.Ordinal))
        {
            BucketGlow.color = new Color (1, 1, 1, 0);
            ShovelGlow.color = new Color(1, 1, 1, 1);
        }
        else if (activeTool.Equals("bucket", System.StringComparison.Ordinal))
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

        // On left mouse button down log the start position
        if(Input.GetMouseButtonDown(0))
        {
            clickStartPos = Input.mousePosition;
            //Debug.Log("startPos=" + clickStartPos);
        }
        
        // On left mouse click, prepare to drag.
        if (Input.GetMouseButton(0))
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

        //Left click to use tool
        if (Input.GetMouseButtonUp(0))
        {
            // Call UseTool
            UseTool(cellPos);
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
                hoverText.text = string.Join(": ", hoverGroundString, hoverPlantString);
            }

            // Update Water Text

        }
    }

    public void UseTool(Vector3Int cellPos)
    {
        // If mouse position is over the tool buttons, don't use a tool!
        if (MouseWithinToolContainer() || disableTool)
        {
            return;
        }

        if (activeTool.Equals("shovel", System.StringComparison.Ordinal) && clickStartPos == Input.mousePosition)
        {
            grid.GetComponent<GridManager>().ClearPlants(cellPos);
        }
        else if (activeTool.Equals("bucket", System.StringComparison.Ordinal))
        {
            // If waterInventory is > 0, then you can place water.
            if (waterInventory > 0 && clickStartPos == Input.mousePosition)
            {
                // Only places water and subtracts invntory if succesful.
                if (grid.GetComponent<GridManager>().PlaceWater(cellPos))
                {
                    //waterInventory--;
                    waterSpent++;
                }
            }
        }        
    }

    public bool MouseWithinToolContainer()
    {
        // Get mouse position in the world.
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // For each rect transform in disableMouseOver:
        foreach (RectTransform rT in disableMouseOver)
        {
            // Get toolContainer corners
            Vector3[] corners = new Vector3[4];
            rT.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Scale the toolContainer Rect Transform to the canvas.
            Vector2 scale = canvasRT.localScale;
            Vector2 scaledSize = new Vector2(scale.x * rT.rect.size.x, scale.y * rT.rect.size.y);
            Rect scaledToolContainer = new Rect(topLeft, scaledSize);

            if (scaledToolContainer.Contains(mousePosInWorld))
            {
                return true;
            }
        }

        return false;
    }
}

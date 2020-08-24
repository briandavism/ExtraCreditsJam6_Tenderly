using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeTool : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public void ToolUpdate(string tool)
    {
        //InputController.waterInventory ++;
        //Debug.Log(InputController.waterInventory);
        InputController.activeTool = tool;
    }

    /*
    //Do this when the cursor enters the rect area of this selectable UI object.
    public void OnPointerEnter(PointerEventData eventData)
    {
        InputController.disableTool = true;
        Debug.Log("Blaaah! Enter.");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        InputController.disableTool = false;
        Debug.Log("Booooo, exit.");
    }
    */
}

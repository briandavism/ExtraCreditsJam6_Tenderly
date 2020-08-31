using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTool : MonoBehaviour
{
    public void ToolUpdate(string tool)
    {
        //InputController.waterInventory ++;
        //Debug.Log(InputController.waterInventory);
        InputController.activeTool = tool;
    }
}

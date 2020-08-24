using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTool : MonoBehaviour
{
    //public InputController MyInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("hi");
    }

    public void ToolUpdate(string tool)
    {
        //InputController.waterInventory ++;
        //Debug.Log(InputController.waterInventory);
        InputController.activeTool = tool;
    }

}

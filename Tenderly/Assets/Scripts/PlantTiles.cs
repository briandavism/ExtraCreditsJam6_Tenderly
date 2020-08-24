using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlantTiles : MonoBehaviour
{

    public List<Tile> plantList;
    public Dictionary<string, Tile> plantDictionary = new Dictionary<string, Tile>();

    void Start()
    {
        // For every plant in the plant list, add it to the dictionary.
        for(int plantIndex = 0; plantIndex < plantList.Count; plantIndex++)
        {
            plantDictionary.Add(plantList[plantIndex].name, plantList[plantIndex]);
        }

    }

}

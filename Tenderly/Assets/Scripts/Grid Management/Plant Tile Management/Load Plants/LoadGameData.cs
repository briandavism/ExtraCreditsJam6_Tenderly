using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=mAeTRCT0qZg

public class LoadGameData : MonoBehaviour
{
    //Create list of plants and terrains to populate
    List<Plant> plants = new List<Plant>();
    List<Terrain> terrains = new List<Terrain>();

    // Start is called before the first frame update
    void Start()
    {
        //Load and parse the plant data
        TextAsset plantData = Resources.Load<TextAsset>("PlantData");

        string[] pData = plantData.text.Split(new char[] { '\n' });

        for (int i = 1; i < pData.Length; i++)
        {
            string[] row = pData[i].Split(new char[] { ',' });
            Plant p = new Plant();
            int.TryParse(row[0], out p.id);
            p.name = row[1];
            int.TryParse(row[2], out p.score);
            int.TryParse(row[3], out p.barrenDrop);
            int.TryParse(row[4], out p.soilDrop);
            int.TryParse(row[5], out p.marshDrop);
            int.TryParse(row[6], out p.waterDrop);
            p.ingredient1 = row[7];
            int.TryParse(row[8], out p.count1);
            p.ingredient2 = row[9];
            int.TryParse(row[10], out p.count2);

            plants.Add(p);
        }

        /*
        foreach (Plant p in plants)
        {
            Debug.Log(
                "id:" + p.id +
                " Name:" + p.name +
                " Score:" + p.score
                );
        }
        */

        //Load and parse the terrain data
        TextAsset terrainData = Resources.Load<TextAsset>("TerrainData");

        string[] tData = terrainData.text.Split(new char[] { '\n' });

        //barrenDrop,soilDrop,marshDrop,waterDrop
        for (int i = 1; i < tData.Length; i++)
        {
            string[] row = tData[i].Split(new char[] { ',' });
            Terrain t = new Terrain();
            t.name = row[0];
            float.TryParse(row[1], out t.spawnRate);
            t.dropsDict = new Dictionary<string, int>();

            foreach (Plant p in plants)
            {
                //append a plant to the dict if it has a tDrop score >0
                if (t.name == "Barren" && p.barrenDrop > 0)
                {
                    t.dropsDict.Add(p.name, p.barrenDrop);
                }
                else if (t.name == "Soil" && p.soilDrop > 0)
                {
                    t.dropsDict.Add(p.name, p.soilDrop);
                }
                else if (t.name == "Marsh" && p.marshDrop > 0)
                {
                    t.dropsDict.Add(p.name, p.marshDrop);
                }
                else if (t.name == "Water" && p.waterDrop > 0)
                {
                    t.dropsDict.Add(p.name, p.waterDrop);
                }

            }
            terrains.Add(t);
        }
        /*
        foreach (Terrain t in terrains)
        {
            Debug.Log(
                "Name:" + t.name +
                " Rate:" + t.spawnRate +
                " Drops:");
            
            foreach (KeyValuePair<string, int> kvp in t.dropsDict)
            {
                Debug.Log("Key = "+ kvp.Key + "Value = " + kvp.Value);
            }
            
        }
        */

    }

}

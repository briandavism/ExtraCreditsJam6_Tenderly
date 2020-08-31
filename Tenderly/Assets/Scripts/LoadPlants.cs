using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=mAeTRCT0qZg

public class LoadPlants : MonoBehaviour
{
    List<Plant> plants = new List<Plant>();
    // Start is called before the first frame update
    void Start()
    {
        TextAsset plantData = Resources.Load<TextAsset>("PlantData");

        string[] data = plantData.text.Split(new char[] { '\n' });

        for (int i = 1; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            Plant q = new Plant();
            int.TryParse(row[0], out q.id);
            q.Name = row[1];
            int.TryParse(row[2], out q.Score);
            int.TryParse(row[3], out q.BarrenDrop);
            int.TryParse(row[4], out q.SoilDrop);
            int.TryParse(row[5], out q.MarshDrop);
            int.TryParse(row[6], out q.WaterDrop);
            q.Ingredient1 = row[7];
            int.TryParse(row[8], out q.Count1);
            q.Ingredient2 = row[9];
            int.TryParse(row[10], out q.Count2);

            plants.Add(q);
        }

        foreach (Plant q in plants)
        {
            Debug.Log(
                "id:" + q.id +
                " Name:" + q.Name +
                " Score" + q.Score
                );
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MergeManager : MonoBehaviour
{
    public static List<Recipe> recipeMasterList;

    private void Start()
    {
        // Populate the recipeMasterList!

        
    }


    // Attempt Merge: Given a tilemap of plants and a Vector3Int position, can the tile at this location merge?
    //  If so, return the Vector3Int of the resulting merge, which can differ from the orignial tile.
    public static Vector3Int AttemptMerge(Tilemap plantTiles, Vector3Int tilePosition)
    {
        // What will we eventually return? Initially, a null value until we find a valid merge.
        Vector3Int chosenTileLocation = new Vector3Int();

        // What type of tile am I?
        string tileName = plantTiles.GetTile<Tile>(tilePosition).name;

        // Create empty list of triples that we can add all valid recipes into.
        // Also create a list of the desired upgrade that cooresponds 1 to 1 indexwise with completed recipes.
        List<List<Vector3Int>> completedRecipes = new List<List<Vector3Int>>();
        List<string> potentialTileNames = new List<string>();

        // Who are my neighboring triples?
        List<List<Vector3Int>> tileTriples = GetClusters(plantTiles, tilePosition);
        tileTriples.AddRange(GetTendrils(plantTiles, tilePosition));

        // For every triple, check if it forms a valid recipe and add to compleedRecipes list.
        foreach (List<Vector3Int> triple in tileTriples)
        {
            // ValidRecipe should return a Tile.name if this triple is a valid recipe, or a null otherwise.
            string recipeName = ValidRecipe(plantTiles, triple);

            // If ValidRecipe returned a name, ther was a valid recipe. If null, no recipe found for this triple.
            if (recipeName != null)
            {
                completedRecipes.Add(triple);
                potentialTileNames.Add(recipeName);
            }
        }

        // Now that we have every valid recipe, we just need to pick one from among them and complete the merge.
        // First, pick one of the completedRecipes by some method. 
        // TODO: By some other method than just random?
        int randomInt = Random.Range(0, completedRecipes.Count);
        List<Vector3Int> chosenTriple = completedRecipes[randomInt];
        Dictionary<string, Tile> plantDictionary = plantTiles.GetComponent<PlantTiles>().plantDictionary;
        string plantName = "";

        // From the chosenTriple, choose one of the tiles in the triple to upgrade. 
        randomInt = Random.Range(0, chosenTriple.Count);
        for (int tripleIndex = 0; tripleIndex < chosenTriple.Count; tripleIndex++)
        {
            // If this tile is not the chosen tile, clear it.
            if (randomInt != tripleIndex)
            {
                // TODO: How to set the tile to empty?
                plantName = potentialTileNames[tripleIndex];
                plantTiles.SetTile(chosenTriple[tripleIndex], plantDictionary[plantName]);
            } else
            {
                // This is the chosen tile. Set it to the chosen tile type.
                chosenTileLocation = chosenTriple[randomInt];
                plantName = potentialTileNames[tripleIndex];
                plantTiles.SetTile(chosenTriple[tripleIndex], plantDictionary[plantName]);
            }
        }

        return chosenTileLocation;
    }


    // Given a list of three tiles, return the product of the recipe as a string if valid, null otherwise.
    public static string ValidRecipe(Tilemap plantTiles, List<Vector3Int> triple)
    {
        List<string> potentialRecipes = new List<string>();

        // Form a Dictionary that represents the ingredents (Tiles) as keys and how many times they occur (int)
        //  as values. // TODO use custom dictionary to instantiate faster.
        Dictionary<string, int> tripleDict = new Dictionary<string, int>(); 
        foreach(Vector3Int tile in triple)
        { 
            tripleDict[plantTiles.GetTile(tile).name]  += 1;
        }

        // Next, for every recipes, check the triple against the recipes in a recipeMasterList to find a match. 
        // Note that we want to maintain a list of valid recipe indices.
        List<int> validRecipeIndices = new List<int>(); // TODO: Is this needed? What could it be used for?
        for (int recipeIndex = 0; recipeIndex < recipeMasterList.Count; recipeIndex++)
        {
            // Check if tripleDict recipe matches the recipe found at index of the masterRecipeList
            string recipeName = recipeMasterList[recipeIndex].RecipeDictEquality(tripleDict);
            if (recipeName != null)
            {
                // If the tile isn't null then we found that this tripleDict satisfies the recipe at recipeIndex.
                validRecipeIndices.Add(recipeIndex);

                // We can add the tile to the list of potentialRecipes
                potentialRecipes.Add(recipeName);
            }

        }

        // From our list of validRecipes, pick a random one and return its name.
        int randomInt = Random.Range(0, potentialRecipes.Count);
        string chosenRecipe = potentialRecipes[randomInt];

        return chosenRecipe;
    }


    // Get Clusters: There are 15 unique clusters that form around a central tile.
    // This method makes a list of them and returns them.
    private static List<List<Vector3Int>> GetClusters(Tilemap tilemap, Vector3Int centerTilePosition)
    {
        List<List<Vector3Int>> tileClusters = new List<List<Vector3Int>>();

        // The outer loop represents the 6 possible tiles that surround the central tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every cluster.
            List<Vector3Int> cluster = new List<Vector3Int>();
            cluster.Add(centerTilePosition);

            // Add the first outer loop tile of this cluster, based on its position relative to center.
            // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = HexMath.cubeDirections[i] + HexMath.OddrToCube(centerTilePosition);
            Vector3Int targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
            cluster.Add(targetOddrPosition);

            // The inner loop goes through each unique third tile addition to the group.
            for (int j = i + 1; j < 6; j++)
            {
                // Add the inner loop tile of this cluster, based on its position relative to center.
                // That is, centerTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
                targetCubePosition = HexMath.cubeDirections[j] + HexMath.OddrToCube(centerTilePosition);
                targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
                cluster.Add(targetOddrPosition);

                // Now add this cluster of three tiles to the tileClusters list.
                tileClusters.Add(cluster);
            }
        }

        return tileClusters;
    }


    // Get Tendrils: There are 18 unique tendrils that form using a given tile as a base, not including the
    // tendrils that are also considered clusters.
    private static List<List<Vector3Int>> GetTendrils(Tilemap tilemap, Vector3Int baseTilePosition)
    {
        List<List<Vector3Int>> tileTendrils = new List<List<Vector3Int>>();

        // The for loop represents the 6 possible tiles that surround the base tile.
        for (int i = 0; i < 6; i++)
        {
            // Add the center tile since it will be part of every tendril.
            List<Vector3Int> tendril = new List<Vector3Int>();
            tendril.Add(baseTilePosition);

            // Add this loop's tile to this tednril, based on its position relative to the base.
            // That is, baseTilePosition + cubeDirections. Of course we have to convert to and from cube coords.
            Vector3Int targetCubePosition = HexMath.cubeDirections[i] + HexMath.OddrToCube(baseTilePosition);
            Vector3Int targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
            tendril.Add(targetOddrPosition);

            // The third tile slightly is more complicated. It can be in any three directions based on the initial
            // direction, deviating by -1, 0, and +1, and making sure to wrap from -1 to 5.
            for (int d = -1; d <= 1; d++)
            {
                int newDirection = ((i + d) % 6 + 6) % 6;
                targetCubePosition = HexMath.cubeDirections[newDirection] + HexMath.OddrToCube(baseTilePosition);
                targetOddrPosition = HexMath.CubeToOddr(targetCubePosition);
                tendril.Add(targetOddrPosition);

                // Now add this tendril of three tiles to the tileTendrils list.
                tileTendrils.Add(tendril);
            }
        }

        return tileTendrils;
    }
}

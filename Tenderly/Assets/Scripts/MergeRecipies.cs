using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Numerics;

public class MergeRecipies : MonoBehaviour
{
    public static List<Recipe> recipeMasterList;

    private void Start()
    {
        // Populate the recipeMasterList!
    }


    // Given a list of three tiles, return the product of the recipe if valid, null otherwise.
    public static Tile ValidRecipe(List<Tile> triple)
    {
        List<Tile> potentialRecipes = new List<Tile>();

        // First, given the each tile's name, get the associated plantVector
        // TODO: IMPORTANT! BITVECTOR32 won't work, we need vectors that can store actual integers.
        Dictionary<Tile, int> tripleDict = new Dictionary<Tile, int>(); //TODO use custom dictionary to instantiate faster.
        foreach(Tile tile in triple)
        {
            // TODO: create a dictionary, keyed on string tilename, value of the associated plant vector.
            tripleDict[tile]  += 1;
        }

        // Next, for every recipes, check the triple against the recipes in a recipeMasterList to find a match. 
        // Note that we want to maintain a list of valid recipe indices.
        List<int> validRecipeIndices = new List<int>();
        for (int recipeIndex = 0; recipeIndex < recipeMasterList.Count; recipeIndex++)
        {
            // Check if tripleDict recipe matches the recipe found at index of the masterRecipeList
            Tile r = recipeMasterList[recipeIndex].RecipeDictEquality(tripleDict);
            if (r != null)
            {
                // If the tile isn't null then we found that this tripleDict satisfies the recipe at recipeIndex.
                validRecipeIndices.Add(recipeIndex);

                // We can add the tile to the list of potentialRecipes
                potentialRecipes.Add(r);
            }

        }

        return null;
    }


    // recipesMasterDict is a dictionary of dictionaries. That is, given a tile (as a key), look up the dictionary
    // associated with that kind of tile, which is a dictionary keyed on all its ingredients and their associated
    // count. For example, recipesMasterDict[Bracken] returns the dictionary {Fiddlehead : 3} because it is made
    // from 
}

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Numerics;

public class MergeRecipies : MonoBehaviour
{
    public static Dictionary<string, List<string>> mergeInto = new Dictionary<string, List<string>>
    {
        ["Fiddlehead"] = new List<string>()
        {
            "Bracken"
        },
        ["SoftRush"] = new List<string>()
        {
            "Bullrush"
        },
        ["Lilypad"] = new List<string>()
        {
            "WaterLily"
        }
    };

    public static Dictionary<string, List<string>> mergeFrom = new Dictionary<string, List<string>>
    {
        ["Bracken"] = new List<string>()
        {
            "Fiddlehead","Fiddlehead","Fiddlehead"
        },
        ["Bullrush"] = new List<string>()
        {
            "SoftRush","SoftRush","SoftRush"
        },
        ["WaterLily"] = new List<string>()
        {
            "Lilypad","Lilypad","Lilypad"
        }
    };


    // Given a list of three tiles, return the product of the recipe if valid, null otherwise.
    public static Tile ValidRecipe(List<Tile> triple)
    {
        Tile recipeResult = null;

        // First, given the each tile's name, get the associated plantVector
        // TODO: IMPORTANT! BITVECTOR32 won't work, we need vectors that can store actual integers.
        BitVector32 tripleVector = new BitVector32(0);
        foreach(Tile tile in triple)
        {
            // TODO: create a dictionary, keyed on string tilename, value of the associated plant vector.
            tripleVector += plantVectors[tile.name];
        }

        // Next, for every recipes, check the tripleVector against the recipeVector to find a match.
        foreach(Tile result in recipes)   
        {
            // TODO: Create a dictionary, keyed on string tilename, value of the associated recipe vector.
            BitVector32 recipeVector = recipeVectors[result.name];

            if(recipeVector.Equals(tripleVector))
            {
                recipeResult = result;
            }
        }

        return recipeResult;
    }

}

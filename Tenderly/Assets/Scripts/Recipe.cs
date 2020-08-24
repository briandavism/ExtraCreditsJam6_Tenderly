using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Recipe : MonoBehaviour
{
    public Tile result;
    public Dictionary<Tile, int> recipeDict;
    public int uniqueIngredientCount;
    public int recipeCost = 0;

    public Recipe(Tile r, Dictionary<Tile, int> rD)
    {
        result = r;
        recipeDict = rD;
        uniqueIngredientCount = recipeDict.Count;

        recipeCost = RecipeCostCalculator(recipeDict);

    }

    public Tile RecipeDictEquality(Dictionary<Tile, int> potentialRecipe)
    {
        // Are the costs the same?
        if (recipeCost != RecipeCostCalculator(potentialRecipe))
        {
            return null;
        }

        // Do they have the same number of ingredients?
        if (uniqueIngredientCount != potentialRecipe.Count)
        {
            return null;
        }

        // For each Key Value pair, are the keys the same?
        foreach (KeyValuePair<Tile, int> ingredient in potentialRecipe)
        {
            // For this ingredient, does its Key exist in our recipeDict?
            if (recipeDict.ContainsKey(ingredient.Key))
            {
                // If the keys are the same, are the values the same?
                if (recipeDict[ingredient.Key] != ingredient.Value)
                {
                    // If not, return null.
                    return null;
                }
            } else
            {
                // If keys don't match, return null.
                return null;
            }
        }

        // Otherwise, the potentalRecipe matches this recipe, so return its reslting tile.
        return result;
    }


    // RecipeCost: Given a dictionary of tiles and their costs, what is the total recipeCost?
    public int RecipeCostCalculator(Dictionary<Tile, int> rD)
    {
        int rC = 0;

        // For every ingredient, we add its amount to the total rC.
        foreach (KeyValuePair<Tile, int> ingredient in rD)
        {
            rC += ingredient.Value;
        }

        return rC;
    }
}

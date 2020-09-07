using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RecipeManager : MonoBehaviour
{
    public static Dictionary<string, Dictionary<string, int>> recipeMasterDict = 
        new Dictionary<string, Dictionary<string, int>>();

    public static List<string> recipeNamesDumb = new List<string>()
    {
        "Bracken",
        "Bullrush,",
        "Waterlily",
        "CallaLily",
        "Mustard",
        "Shrub",
        "CorpseFlower",
        "Fireweed",
        "Sumac"
    };

    public Dictionary<string, int> brackenDict = new Dictionary<string, int>()
    {
        {"Fiddlehead", 2},
        {"Grass", 1}
    };
    public Dictionary<string, int> bullrushDict = new Dictionary<string, int>()
    {
        {"SoftRush", 2},
        {"Grass", 1}
    };
    public Dictionary<string, int> waterlilyDict = new Dictionary<string, int>()
    {
        {"Liliypad", 2},
        {"Algae", 1}
    };
    public Dictionary<string, int> callaLilyDict = new Dictionary<string, int>()
    {
        {"Waterlily", 1},
        {"Bullrush", 2}
    };
    public Dictionary<string, int> mustardDict = new Dictionary<string, int>()
    {
        {"Grass", 2},
        {"Bracken", 1}
    };
    public Dictionary<string, int> shrubDict = new Dictionary<string, int>()
    {
        {"Bracken", 3},
    };
    public Dictionary<string, int> corpseFlowerDict = new Dictionary<string, int>()
    {
        {"CallaLily", 2},
        {"Waterlily", 1}
    };
    public Dictionary<string, int> fireweedDict = new Dictionary<string, int>()
    {
        {"Mustard", 2},
        {"Bracken", 1}
    };
    public Dictionary<string, int> sumacDict = new Dictionary<string, int>()
    {
        {"Shrub", 2},
        {"CallaLily", 1}
    };

    public static TextAsset recipeJsonAsset;

    void Start()
    {
        // HARDCODEBADCODE
        recipeMasterDict.Add("Bracken", brackenDict);
        recipeMasterDict.Add("Bullrush", bullrushDict);
        recipeMasterDict.Add("Waterlily", waterlilyDict);
        recipeMasterDict.Add("CallaLily", callaLilyDict);
        recipeMasterDict.Add("Mustard", mustardDict);
        recipeMasterDict.Add("Shrub", shrubDict);
        recipeMasterDict.Add("CorpseFlower", corpseFlowerDict);
        recipeMasterDict.Add("Fireweed", fireweedDict);
        recipeMasterDict.Add("Sumac", sumacDict);


        /*
        recipeJsonAsset = Resources.Load<TextAsset>("Recipes.json");

        Debug.Log(recipeJsonAsset.text);

        recipeMasterDict = new Dictionary<string, Dictionary<string, int>>();
        Dictionary<string, int> recipeDict = new Dictionary<string, int>();

        Recipes recipesFromJson = JsonUtility.FromJson<Recipes>(recipeJsonAsset.text);

        // For each recipe in recipes:
        string recipeName;
        string ingredientName;
        int ingredientCount;
        RecipeIngredients recipeIngredients = new RecipeIngredients();
        foreach (Recipe recipe in recipesFromJson.recipes)
        {
            // For each recipe
            recipeName = recipe.recipeName;
            recipeIngredients = recipe.recipeIngredients;

            // Loop through the recipeIngredients
            for (int i = 0; i < recipeIngredients.ingredients.Length; i++)
            {
                ingredientName = recipeIngredients.ingredients[i];
                ingredientCount = recipeIngredients.count[i];

                // Add an entry to the dictionary based on this info. // TODO: Duplicate and error checking.
                recipeDict.Add(ingredientName, ingredientCount);
            }

            recipeMasterDict.Add(recipeName, recipeDict);
        } */
    }


    // RecipeInMasterDictionary: returns the resultName of a valid recipe if the provided dictionary 
    //  matches one of the recipes in the master dictionary.
    public static string RecipeInMasterDict(Dictionary<string, int> potentialRecipe)
    {
        // For each recipeName, does the provided ingredient dictionary match?
        foreach (KeyValuePair<string, Dictionary<string, int>> recipe in recipeMasterDict)
        {
            // Are the costs the same?
            if (RecipeCostCalculator(recipe.Value) != RecipeCostCalculator(potentialRecipe))
            {
                return null;
            }

            // Do they have the same number of ingredients?
            if (recipe.Value.Count != potentialRecipe.Count)
            {
                return null;
            }

            // For each Key Value pair, are the keys the same?
            foreach (KeyValuePair<string, int> ingredient in potentialRecipe)
            {
                // For this ingredient, does its Key exist in our recipeDict?
                if (recipe.Value.ContainsKey(ingredient.Key))
                {
                    // If the keys are the same, are the values the same?
                    if (recipe.Value[ingredient.Key] != ingredient.Value)
                    {
                        // If not, return null.
                        return null;
                    }
                }
                else
                {
                    // If keys don't match, return null.
                    return null;
                }
            }

            // Otherwise, the potentalRecipe matches this recipe, so return its reslting tile.name.
            return recipe.Key;
        }

        // No recipes matched.
        return null;
    }


    // RecipeCost: Given a dictionary of tile.names and their costs, what is the total recipeCost?
    public static int RecipeCostCalculator(Dictionary<string, int> rD)
    {
        int rC = 0;

        // For every ingredient, we add its amount to the total rC.
        foreach (KeyValuePair<string, int> ingredient in rD)
        {
            rC += ingredient.Value;
        }

        return rC;
    }
}

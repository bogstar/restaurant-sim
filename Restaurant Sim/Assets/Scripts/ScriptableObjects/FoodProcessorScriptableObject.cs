using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Processor", menuName = "MammaMia/Food Processor")]
public class FoodProcessorScriptableObject : DatabaseEntryScriptableObject
{
	public new string name;
	public RecipeItemScriptableObject[] recipes;

	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "food_processor_recipe" + newString;
	}
}
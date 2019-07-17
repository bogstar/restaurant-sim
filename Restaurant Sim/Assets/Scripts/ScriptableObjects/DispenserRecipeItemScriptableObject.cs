using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dispenser Recipe", menuName = "MammaMia/Dispenser Recipe")]
public class DispenserRecipeItemScriptableObject : DatabaseEntryScriptableObject
{
	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "dispenser_recipe_" + newString;
	}

	public MachineRecipe recipe;

	[System.Serializable]
	public struct MachineRecipe
	{
		public ItemStack input;
		public ItemScriptableObject output;
		public float time;
		public bool packageOutput;
	}

	[System.Serializable]
	public struct ItemStack
	{
		public ItemScriptableObject item;
		public float count;
	}
}
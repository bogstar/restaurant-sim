using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Recipe", menuName = "MammaMia/Recipe")]
public class RecipeItemScriptableObject : DatabaseEntryScriptableObject
{
	public MachineRecipe recipe;

	public bool CanCraft(Carryable[] items)
	{
		Dictionary<string, int> inputItemMap = new Dictionary<string, int>();
		Dictionary<string, int> recipeItemMap = new Dictionary<string, int>();

		foreach (var item in items)
		{
			if (inputItemMap.ContainsKey(item.data.id))
				inputItemMap[item.data.id]++;
			else
				inputItemMap.Add(item.data.id, 1);
		}

		foreach (var item in recipe.input)
		{
			if (recipeItemMap.ContainsKey(item.item.id))
				recipeItemMap[item.item.id]++;
			else
				recipeItemMap.Add(item.item.id, Mathf.Max(item.count, 1));
		}

		for (int i = inputItemMap.Count - 1; i > -1; i--)
		{
			string inputId = inputItemMap.Keys.ToArray()[i];
			int inputCount = inputItemMap.Values.ToArray()[i];

			if (recipeItemMap.ContainsKey(inputId))
			{
				int count = Mathf.Min(recipeItemMap[inputId], inputItemMap[inputId]);
				inputItemMap[inputId] -= count;
				recipeItemMap[inputId] -= count;

				if (inputItemMap[inputId] < 1)
					inputItemMap.Remove(inputId);
				if (recipeItemMap[inputId] < 1)
					recipeItemMap.Remove(inputId);
			}
			else
			{
				return false;
			}
		}

		if (recipeItemMap.Count < 1)
		{
			return true;
		}

		return false;
	}

	[System.Serializable]
	public struct MachineRecipe
	{
		public float craftTime;
		public ItemStack[] input;
		public ItemStack[] output;
	}

	[System.Serializable]
	public struct ItemStack 
	{
		public ItemScriptableObject item;
		public int count;
	}

	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "recipe_item_" + newString;
	}
}
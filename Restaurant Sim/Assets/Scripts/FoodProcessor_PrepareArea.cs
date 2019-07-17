using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProcessor_PrepareArea : WorkArea
{
	public DispenserRecipeItemScriptableObject[] recipes;

	public override List<DulibaWaitor.Command> CanInteractWithItem(Carryable item)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		var recipeToDispense = GetRecipeToDispense(item);

		if (recipeToDispense != null)
		{
			if (recipeToDispense.recipe.packageOutput)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = recipeToDispense.recipe.time,
					name = "prepare 6 " + recipeToDispense.recipe.output.name,
					image = recipeToDispense.recipe.output.image,
					item = new Package(recipeToDispense.recipe.output, 6),
					lateCallback = () =>
					{
						OnCanvasUpdate?.Invoke();
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
			else
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = recipeToDispense.recipe.time,
					name = "prepare " + recipeToDispense.recipe.output.name,
					image = recipeToDispense.recipe.output.image,
					item = new Carryable(recipeToDispense.recipe.output),
					lateCallback = () =>
					{
						OnCanvasUpdate?.Invoke();
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
		}

		return actions;
	}

	DispenserRecipeItemScriptableObject GetRecipeToDispense(Carryable item)
	{
		foreach (var recipe in recipes)
		{
			if (recipe.recipe.input.item.id == item.data.id)
			{
				return recipe;
			}
		}

		return null;
	}
}
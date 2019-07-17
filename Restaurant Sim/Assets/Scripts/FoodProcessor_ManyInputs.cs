using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProcessor_ManyInputs : FoodProcessor
{
	public int pipelinesCount = 4;
	ProcessingPipeline[] pipeline;

	protected override void Start()
	{
		base.Start();

		pipeline = new ProcessingPipeline[pipelinesCount];
	}

	private void Update()
	{
		foreach (var pipe in pipeline)
		{
			if (pipe.input == null)
			{
				continue;
			}

			if (pipe.input is Item item)
			{
				pipe.input.Spoil();
				OnWorkAreaUpdate?.Invoke(this);
			}
		}

		if (waitors.Count > 0)
		{
			OnCanvasUpdate?.Invoke();
		}
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable carryable)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		for (int i = 0; i < pipeline.Length; i++)
		{
			var pipe = pipeline[i];
			var temp = i;
			
			if (pipe.input == null && carryable is Item item)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.25f,
					actionType = DulibaWaitor.ActionType.Place,
					name = "place on " + (i + 1) + " " + item.data.name,
					image = item.data.image,
					callback = () =>
					{
						item.Unreturn();
						StartCraft(item, temp);
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
		}

		return actions;
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		for (int i = 0; i < pipeline.Length; i++)
		{
			var item = pipeline[i];
			var temp = i;

			if (item.input != null)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.25f,
					actionType = DulibaWaitor.ActionType.Pickup,
					reflected = true,
					shelf = new ItemDataWithCount(item.input.data, 0, 1, false, false),
					name = "pickup from " + (i + 1) + " " + item.input.data.name,
					image = item.input.data.image,
					callback = () =>
					{
						InterruptCraft(temp);
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
		}

		return actions;
	}

	protected void InterruptCraft(int index)
	{
		pipeline[index].input = null;

		if (pipeline[index].craftingCoroutine != null)
		{
			StopCoroutine(pipeline[index].craftingCoroutine);
			pipeline[index].craftingCoroutine = null;
			pipeline[index].recipe = null;
		}
	}

	protected void StartCraft(Item item, int index)
	{
		pipeline[index].input = item;

		RecipeItemScriptableObject recipe = GetRecipeForItem(item);

		if (recipe != null)
		{
			pipeline[index].input.Unreturn();
			pipeline[index].recipe = recipe;
			pipeline[index].craftingStart = Time.time;
			pipeline[index].craftingEnd = Time.time + recipe.recipe.craftTime;
			pipeline[index].craftingCoroutine = StartCoroutine(WaitForCraftingTime(index));
		}
	}

	RecipeItemScriptableObject GetRecipeForItem(Carryable item)
	{
		foreach (var recipe in foodProcessor.recipes)
		{
			if (recipe.recipe.input[0].item.id == item.data.id)
			{
				return recipe;
			}
		}

		return null;
	}

	void FinishCraft(int index)
	{
		pipeline[index].FinishProduct();

		OnWorkAreaUpdate?.Invoke(this);
	}

	IEnumerator WaitForCraftingTime(int index)
	{
		while (Time.time < pipeline[index].craftingEnd)
		{
			yield return new WaitForEndOfFrame();
		}

		FinishCraft(index);
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items in the processor:\n\n";

		for (int i = 0; i < pipeline.Length; i++)
		{
			var pipe = pipeline[i];
			var temp = i;

			text += "Pipe " + (temp + 1) + ":\n";
			if (pipe.craftingCoroutine != null)
			{
				text += pipe.input.data.name + " --> " + pipe.recipe.recipe.output[0].item.name + " " + (pipe.GetCraftingPercentageDone() * 100).ToString("#") + "%\n\n";
			}
			else
			{
				text += "Item: ";
				if (pipe.input != null)
				{
					text += pipe.input.data.name;
					if (pipe.input is Item item && item.data.rottable)
					{
						text += " - time to rot " + (item.spoilageTime - Time.time).ToString("#");
					}
				}
				else
				{
					text += "None";
				}
				
				text += "\n\n";
			}
		}

		info.text = text;
		return info;
	}

	struct ProcessingPipeline
	{
		public float craftingStart;
		public float craftingEnd;
		public RecipeItemScriptableObject recipe;
		public Coroutine craftingCoroutine;
		public Carryable input;

		public float GetCraftingPercentageDone()
		{
			if (craftingCoroutine != null)
			{
				return Mathf.InverseLerp(craftingStart, craftingEnd, Time.time);
			}

			return 0;
		}

		public void FinishProduct()
		{
			if (craftingCoroutine != null)
			{
				input = new Carryable(recipe.recipe.output[0].item);
				recipe = null;
				craftingCoroutine = null;
			}
		}
	}
}
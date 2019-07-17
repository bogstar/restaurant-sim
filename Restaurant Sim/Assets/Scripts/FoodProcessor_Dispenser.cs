using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodProcessor_Dispenser : WorkArea
{
	public DispenserRecipeItemScriptableObject[] recipes;
	public float maxInputAmount;
	ProcessingPipeline pipeline;

	protected override void Start()
	{
		base.Start();

		pipeline = new ProcessingPipeline(maxInputAmount);
	}

	private void Update()
	{
		if (pipeline.input != null && pipeline.input.data.rottable && Time.time > pipeline.spoilageTimer)
		{
			var rot = new Item((ItemScriptableObject)DatabaseManager.GetItem("rot"));
			rot.data.garbageValue = pipeline.input.data.garbageValue;
			pipeline.input = rot;
			
			pipeline.recipe = null;
			OnWorkAreaUpdate?.Invoke(this);
		}

		if (waitors.Count > 0)
		{
			OnCanvasUpdate?.Invoke();
		}
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		if (CanAdd(item))
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 0.5f,
				actionType = DulibaWaitor.ActionType.Place,
				name = "place " + item.data.name,
				image = item.data.image,
				callback = () =>
				{
					pipeline.Add(item);
					pipeline.recipe = GetRecipeForItem(item);
				},
				lateCallback = () =>
				{
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}

		return actions;
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items in the processor:\n\n";

		var pipe = pipeline;

		text += "Pipe:\n";
		text += "Item: ";
		if (pipe.input != null)
		{
			text += pipe.input.data.name + " " + pipe.inputAmount.ToString("#.##") + "/" + pipe.maxInputAmount.ToString("#.##");
			if (pipe.input.data.rottable)
			{
				text += " - time to rot " + (pipe.spoilageTimer - Time.time).ToString("#");
			}
		}
		else
		{
			text += "None - place for " + pipe.maxInputAmount.ToString("#.##");
		}

		text += "\n\n";

		info.text = text;
		return info;
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		var recipeToDispense = GetRecipeToDispense();

		if (recipeToDispense != null)
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = recipeToDispense.recipe.time,
				reflected = true,
				actionType = DulibaWaitor.ActionType.Pickup,
				name = "pickup " + recipeToDispense.name,
				image = recipeToDispense.recipe.output.image,
				pickupIndex = 0,
				shelf = new ItemDataWithCount(recipeToDispense.recipe.output, 0, 1, false, true),
				callback = () =>
				{
					pipeline.Dispense();
				},
				lateCallback = () =>
				{
					OnCanvasUpdate?.Invoke();
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}

		if (pipeline.input != null && pipeline.inputAmount > 0)
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 3f,
				actionType = DulibaWaitor.ActionType.Action,
				prerequisite = PreRequisite.BothHandsEmpty,
				name = "clean " + name,
				callback = () =>
				{
					var garbageBag = new Carryable((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag"));
					garbageBag.data.garbageValue += (int)(pipeline.input.data.garbageValue * pipeline.inputAmount);
					pipeline.Clean();
					waitor.rightHand.SetItem(garbageBag);
				},
				image = ((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag")).image,
				lateCallback = () =>
				{
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}

		return actions;
	}

	DispenserRecipeItemScriptableObject GetRecipeToDispense()
	{
		if (pipeline.recipe != null && pipeline.CanDispense())
		{
			return pipeline.recipe;
		}

		return null;
	}

	DispenserRecipeItemScriptableObject GetRecipeForItem(Carryable item)
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

	bool CanAdd(Carryable item)
	{
		if (pipeline.CanAdd(item))
		{
			if (GetRecipeForItem(item) != null)
			{
				return true;
			}
		}

		return false;
	}

	class ProcessingPipeline
	{
		/// <summary>
		/// Current recipe according to input.
		/// </summary>
		public DispenserRecipeItemScriptableObject recipe;
		public Carryable input;
		public float inputAmount;
		public float maxInputAmount;
		public float spoilageTimer;

		public ProcessingPipeline(float maxInputAmount)
		{
			this.maxInputAmount = maxInputAmount;
		}

		private ProcessingPipeline() { }

		public bool CanDispense()
		{
			if (recipe == null)
			{
				return false;
			}

			return recipe.recipe.input.count <= inputAmount;
		}

		public void Dispense()
		{
			if (CanDispense())
			{
				inputAmount -= recipe.recipe.input.count;
				if (inputAmount <= 0.001f)
				{
					inputAmount = 0;
					recipe = null;
					input = null;
				}
			}
		}

		public bool CanAdd(Carryable item)
		{
			return inputAmount + 1 <= maxInputAmount && (input == null || input.data.id == item.data.id);
		}

		public bool Add(Carryable carryable)
		{
			if (CanAdd(carryable))
			{
				if (recipe != null && carryable.data.id == input.data.id)
				{
					float c = inputAmount * 100 + 100;
					float d = inputAmount * 100 / c;
					float e = 100 / c;

					spoilageTimer = ((Item)input).spoilageTime * d + ((Item)carryable).spoilageTime * e;

					inputAmount += 1;
				}
				else if (recipe == null)
				{
					input = carryable;
					spoilageTimer = ((Item)carryable).spoilageTime;
					inputAmount = 1;
				}
				return true;
			}

			return false;
		}

		public void Clean()
		{
			input = null;
			recipe = null;
			inputAmount = 0;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FoodProcessor_OneInput : FoodProcessor
{
	ProcessingPipeline pipeline;


	protected override void Start()
	{
		base.Start();

		pipeline = new ProcessingPipeline(12);
	}

	private void Update()
	{
		foreach (var item in pipeline.input)
		{
			if (item == null)
			{
				continue;
			}

			if (item.Spoil())
			{
				OnWorkAreaUpdate?.Invoke(this);
			}
		}

		foreach (var item in pipeline.output)
		{
			if (item == null)
			{
				continue;
			}

			if (item.Spoil())
			{
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

		if (pipeline.craftingCoroutine == null && carryable is Item item && pipeline.CanAdd())
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 0.5f,
				actionType = DulibaWaitor.ActionType.Place,
				name = "place " + carryable.data.name,
				image = carryable.data.image,
				callback = () =>
				{
					carryable.Unreturn();
					pipeline.Add(carryable);
				},
				lateCallback = () =>
				{
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}

		return actions;
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		if (pipeline.craftingCoroutine != null)
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 0.25f,
				actionType = DulibaWaitor.ActionType.Action,
				name = "stop crafting",
				callback = () =>
				{
					InterruptCraft();
				},
				lateCallback = () =>
				{
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}/*
		else if (craftingStopped)
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
					garbageBag.data.garbageValue = outputItems[0].data.garbageValue;
					CleanMachine();
					waitor.rightHand.SetItem(garbageBag);
				},
				image = ((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag")).image,
				lateCallback = () =>
				{
					OnWorkAreaUpdate?.Invoke(this);
				}
			});
		}*/
		else
		{
			for (int i = 0; i < pipeline.output.Count; i++)
			{
				var item = pipeline.output[i];
				int temp = i;

				if (item != null)
				{
					actions.Add(new DulibaWaitor.Command()
					{
						workTime = 0.5f,
						reflected = true,
						actionType = DulibaWaitor.ActionType.Pickup,
						name = "pickup " + item.data.name,
						image = item.data.image,
						pickupIndex = temp,
						item = item,
						callback = () =>
						{
							pipeline.output.Remove(item);
						},
						lateCallback = () =>
						{
							OnCanvasUpdate?.Invoke();
							OnWorkAreaUpdate?.Invoke(this);
						}
					});
				}
			}

			if (!HasItemsInOutput())
			{
				var recipe = GetRecipe();

				if (recipe != null)
				{
					ItemScriptableObject item = recipe.recipe.output[0].item;

					actions.Add(new DulibaWaitor.Command()
					{
						workTime = 0.25f,
						actionType = DulibaWaitor.ActionType.Action,
						name = "craft " + item.name,
						image = item.image,
						prerequisite = PreRequisite.None,
						pickupIndex = 0,
						callback = () =>
						{
							Craft(recipe);
						},
						lateCallback = () =>
						{
							OnWorkAreaUpdate?.Invoke(this);
						}
					});
				}
			}

			if (pipeline.input.Count > 0)
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
						foreach (var item in pipeline.input)
						{
							garbageBag.data.garbageValue += item.data.garbageValue;
						}
						CleanMachine();
						waitor.rightHand.SetItem(garbageBag);
					},
					image = ((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag")).image,
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
		}

		return actions;
	}

	protected void InterruptCraft()
	{
		pipeline.InterruptProcess();
	}

	void Craft(RecipeItemScriptableObject recipe)
	{
		pipeline.StartProcess(recipe);
		pipeline.craftingCoroutine = StartCoroutine(WaitForCraftingTime());

		/*
		Dictionary<string, int> inputItemMap = new Dictionary<string, int>();
		Dictionary<string, int> recipeItemMap = new Dictionary<string, int>();

		foreach (var item in inputItems)
		{
			if (inputItemMap.ContainsKey(item.data.id))
				inputItemMap[item.data.id]++;
			else
				inputItemMap.Add(item.data.id, 1);
		}

		foreach (var item in recipe.recipe.input)
		{
			if (recipeItemMap.ContainsKey(item.item.id))
				recipeItemMap[item.item.id]++;
			else
				recipeItemMap.Add(item.item.id, Mathf.Max(item.count, 1));
		}

		for (int i = recipeItemMap.Count - 1; i > -1; i--)
		{
			string inputId = inputItemMap.Keys.ToArray()[i];
			int inputCount = inputItemMap.Values.ToArray()[i];

			for (int j = 0; j < inputCount; j++)
			{
				inputItems.Remove(GetItem(inputId));
			}
		}

		currentRecipeCrafting = recipe;
		craftingStart = Time.time;
		craftingEnd = Time.time + currentRecipeCrafting.recipe.craftTime;
		crafting = true;

		craftingCoroutine = StartCoroutine(WaitForCraftingTime());*/

		OnWorkAreaUpdate?.Invoke(this);
	}

	IEnumerator WaitForCraftingTime()
	{
		while (Time.time < pipeline.craftingEnd)
		{
			yield return new WaitForEndOfFrame();
		}

		FinishCraft();
	}

	void FinishCraft()
	{
		pipeline.FinishProduct();

		OnWorkAreaUpdate?.Invoke(this);
	}

	Carryable GetItem(string id)
	{
		foreach (var item in pipeline.input)
		{
			if(item.data.id == id)
			{
				return item;
			}
		}

		return null;
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items in the processor:\n\n";

		if (pipeline.craftingCoroutine != null)
		{
			text += "Crafting " + pipeline.recipe.recipe.output[0].item.name + ":\n\n";
			text += "Progress: " + (pipeline.GetCraftingPercentageDone() * 100).ToString("#") + "%\n\n";
		}
		else
		{
			text += "Input Items:\n";
			foreach (var item in pipeline.input)
			{
				text += item.data.name;
				if (item.data.rottable)
				{
					text += " - time to rot " + (((Item)item).spoilageTime - Time.time).ToString("#");
				}
				text += "\n";
			}

			text += "\nOutput Items:\n";
			foreach (var item in pipeline.output)
			{
				text += item.data.name;
				if (item.data.rottable)
				{
					text += " - time to rot " + (((Item)item).spoilageTime - Time.time).ToString("#");
				}
				text += "\n";
			}
		}

		info.text = text;
		return info;
	}

	bool HasItemsInOutput()
	{
		if (pipeline.output.Count > 0)
			return true;

		return false;
	}
	
	void CleanMachine()
	{
		pipeline.input.Clear();
		OnWorkAreaUpdate?.Invoke(this);
	}

	/// <summary>
	/// Returns recipe if the pipeline is able to craft with items provided. Returns null if otherwise.
	/// </summary>
	/// <returns></returns>
	RecipeItemScriptableObject GetRecipe()
	{
		Dictionary<string, int> itemCountMap = new Dictionary<string, int>();

		foreach (var item in pipeline.input)
		{
			if (itemCountMap.ContainsKey(item.data.id))
			{
				itemCountMap[item.data.id]++;
			}
			else
			{
				itemCountMap.Add(item.data.id, 1);
			}
		}

		foreach (var recipe in foodProcessor.recipes)
		{
			bool contains = true;
			foreach (var itemStack in recipe.recipe.input)
			{
				if (!itemCountMap.ContainsKey(itemStack.item.id) || itemCountMap[itemStack.item.id] < itemStack.count)
				{
					contains = false;
					break;
				}
			}

			if (contains)
			{
				return recipe;
			}
		}

		return null;
	}

	class ProcessingPipeline
	{
		float craftingStart;
		public float craftingEnd { get; private set; }
		/// <summary>
		/// Returns recipe that is currently being crafted.
		/// </summary>
		public RecipeItemScriptableObject recipe { get; private set; }
		public Coroutine craftingCoroutine;
		public List<Carryable> input { get; private set; }
		public List<Carryable> output { get; private set; }
		int inputCapacity;
		int collectiveGarbageCount;

		public ProcessingPipeline(int inputCapacity)
		{
			this.inputCapacity = inputCapacity;
			input = new List<Carryable>();
			output = new List<Carryable>();
		}

		private ProcessingPipeline() { }

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
				foreach (var item in recipe.recipe.output)
				{
					Carryable carryable = new Carryable(item.item);
					carryable.Unreturn();
					output.Add(carryable);
				}

				recipe = null;
				craftingCoroutine = null;
			}
		}

		public bool CanAdd()
		{
			return input.Count < inputCapacity;
		}

		/// <summary>
		/// Returns true if can add or false if can't.
		/// </summary>
		/// <param name="carryable"></param>
		/// <returns></returns>
		public bool Add(Carryable carryable)
		{
			if (CanAdd())
			{
				input.Add(carryable);
				return true;
			}

			return false;
		}

		public void StartProcess(RecipeItemScriptableObject recipe)
		{
			if (craftingCoroutine == null)
			{
				foreach (var item in input)
				{
					collectiveGarbageCount += item.data.garbageValue;
				}

				craftingStart = Time.time;
				craftingEnd = Time.time + recipe.recipe.craftTime;
				input.Clear();

				this.recipe = recipe;
			}
		}

		public void InterruptProcess()
		{
			if (craftingCoroutine != null)
			{
				var garbageBag = new Carryable((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag"));
				garbageBag.data.garbageValue = collectiveGarbageCount;
				output.Add(garbageBag);

				recipe = null;
				craftingCoroutine = null;
			}
		}
	}
}
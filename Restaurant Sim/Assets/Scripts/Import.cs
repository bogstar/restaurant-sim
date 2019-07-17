using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Import : WorkArea
{
	public Shelf.ImportItem[] items;

	protected override void Start()
	{
		base.Start();

		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].item == null)
				continue;

			ItemScriptableObject copy = DatabaseManager.GetItem(items[i].item.id) as ItemScriptableObject;
			items[i].item = copy;
		}
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		for (int i = 0; i < GetAvailableItems().Length; i++)
		{
			var item = GetAvailableItems()[i];
			int temp = i;

			if (item.item != null)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					reflected = true,
					actionType = DulibaWaitor.ActionType.Pickup,
					name = "pickup " + item.item.name,
					image = item.item.image,
					pickupIndex = temp,
					shelf = item,
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
		}

		return actions;
	}

	public ItemDataWithCount[] GetAvailableItems()
	{
		ItemDataWithCount[] availableItems = new ItemDataWithCount[4];

		for (int i = 0; i < items.Length; i++)
		{
			availableItems[i].item = items[i].item;
			availableItems[i].count = items[i].itemCount;
			availableItems[i].infinite = items[i].infinite;
			availableItems[i].index = i;
		}

		return availableItems;
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items in the import:\n\n";

		int i = 1;
		foreach (var import in items)
		{
			text += "Import " + i + ":\n";
			if (import.item == null)
			{
				text += "Empty import.\n\n";
			}
			else
			{
				text += import.item.name;
				if (!import.infinite)
				{
					text += " " + import.itemCount + "/" + import.maxItems + "\n\n";
				}
			}

			i++;
		}

		info.text = text;
		return info;
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		return new List<DulibaWaitor.Command>();
	}
}
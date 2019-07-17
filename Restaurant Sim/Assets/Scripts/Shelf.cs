using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : WorkArea
{
	public bool refrigerator;
	public ImportItem[] items;

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

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items in the " + name + ":\n\n";

		int i = 1;
		foreach (var shelf in items)
		{
			text += "Shelf " + i + ":\n";
			if (shelf.item == null)
			{
				text += "Empty shelf. Fits up to " + shelf.maxItems + " items.\n\n";
			}
			else
			{
				text += shelf.item.name;
				if (!shelf.infinite)
				{
					text += " " + shelf.itemCount + "/" + shelf.maxItems + "\n\n";
				}
			}
			
			i++;
		}

		info.text = text;
		return info;
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		if (!item.IsReturnable)
		{
			return new List<DulibaWaitor.Command>();
		}

		if (item.data.cannotShelf)
		{
			return new List<DulibaWaitor.Command>();
		}

		if (item.data.needsFridge && !refrigerator)
		{
			return new List<DulibaWaitor.Command>();
		}

		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		for (int i = 0; i < items.Length; i++)
		{
			var shelf = items[i];
			int temp = i;

			if (item is Package package && shelf.item == null && package.itemsCount <= shelf.maxItems)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.25f * package.itemsCount,
					actionType = DulibaWaitor.ActionType.Place,
					name = "unpack " + item.data.name + " package on shelf " + (temp + 1),
					image = item.data.image,
					pickupIndex = temp,
					callback = () =>
					{
						items[temp].item = (ItemScriptableObject)DatabaseManager.GetItem(package.data.id);
						items[temp].itemCount = package.itemsCount;
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
			else if (item is Package package2 && shelf.item != null && shelf.item.id == item.data.id && shelf.itemCount + package2.itemsCount <= shelf.maxItems)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.25f * package2.itemsCount,
					actionType = DulibaWaitor.ActionType.Place,
					name = "unpack " + item.data.name + " package on shelf " + (temp + 1),
					image = item.data.image,
					pickupIndex = temp,
					callback = () =>
					{
						items[temp].itemCount += package2.itemsCount;
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
			else if (shelf.item != null && shelf.item.id == item.data.id && shelf.itemCount + 1 <= shelf.maxItems)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					actionType = DulibaWaitor.ActionType.Place,
					name = "return " + item.data.name + " on shelf " + (temp + 1),
					image = item.data.image,
					pickupIndex = temp,
					callback = () =>
					{
						items[temp].itemCount++;
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
			else if (shelf.item == null && shelf.itemCount + 1 < shelf.maxItems)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					actionType = DulibaWaitor.ActionType.Place,
					name = "place " + item.data.name + " on shelf " + (temp + 1),
					image = item.data.image,
					pickupIndex = temp,
					callback = () =>
					{
						items[temp].item = item.data;
						items[temp].itemCount = 1;
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

		Carryable itemLH = waitor.leftHand.GetItem();
		Carryable itemRH = waitor.rightHand.GetItem();

		for (int i = 0; i < GetAvailableItems().Length; i++)
		{
			var item = GetAvailableItems()[i];
			int temp = i;

			if (item.item == null)
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					actionType = DulibaWaitor.ActionType.Place,
					shelf = item,
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
			}
			else
			{
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					reflected = true,
					actionType = DulibaWaitor.ActionType.Pickup,
					shelf = item,
					name = "pickup " + item.item.name + " from shelf " + (temp + 1),
					image = item.item.image,
					pickupIndex = temp,
					callback = () =>
					{
						items[temp].itemCount--;
						if (items[temp].itemCount < 1)
						{
							items[temp].item = null;
						}
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

	public void PlaceDownPackage(DulibaWaitor waitor, DulibaInput.Hand hand, int index)
	{
		DulibaHand h = null;

		switch (hand)
		{
			case DulibaInput.Hand.Left:
				h = waitor.leftHand;
				break;
			case DulibaInput.Hand.Right:
				h = waitor.rightHand;
				break;
			default:
				return;
		}

		Package package = (Package) h.ClearItem();
		items[index].item = package.data;
		items[index].itemCount = package.itemsCount;

		OnWorkAreaUpdate?.Invoke(this);
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

	[System.Serializable]
	public struct ImportItem
	{
		public ItemScriptableObject item;
		public bool infinite;
		public int itemCount;
		public int maxItems;
		public Transform displayItemsTransform;
	}
}

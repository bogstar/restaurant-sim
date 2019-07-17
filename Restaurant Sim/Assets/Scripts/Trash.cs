using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : WorkArea
{
	public int maxFullness;
	public int currentFullness;
	public bool infinite;

	public List<string> thrownItems = new List<string>();

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;

		string text = "";

		if (infinite)
		{
			text = "Can accept any number of trash.";
		}
		else
		{
			text = "Garbage can fullness: " + currentFullness + "/" + maxFullness + "\n\n";
			text += "Items in garbage:\n\n";

			if (thrownItems.Count == 0)
			{
				text += "Nothing.";
			}
			else if (thrownItems.Count == 1)
			{
				text += thrownItems[0] + ".";
			}
			else
			{
				for (int i = 0; i < thrownItems.Count - 1; i++)
				{
					text += thrownItems[i] + ", ";
				}
				text += thrownItems[thrownItems.Count - 1] + ".";
			}
		}

		info.text = text;
		return info;
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable carryable)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		if (carryable.data.garbageValue <= maxFullness - currentFullness || infinite)
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 0.5f,
				actionType = DulibaWaitor.ActionType.Place,
				name = "throw " + carryable.data.name,
				image = carryable.data.image,
				callback = () =>
				{
					if (!infinite && carryable.data.garbageValue > 0)
					{
						if (carryable is Package package)
						{
							currentFullness += carryable.data.garbageValue * package.itemsCount;
							thrownItems.Add(carryable.data.name + " package (" + package.itemsCount + ")");
						}
						else
						{
							currentFullness += carryable.data.garbageValue;
							thrownItems.Add(carryable.data.name);
						}
					}
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

		Carryable itemLH = waitor.leftHand.GetItem();
		Carryable itemRH = waitor.rightHand.GetItem();

		if (infinite)
		{
			// We cant empty the infinite trash.
		}
		else if (currentFullness > 0)
		{
			actions.Add(new DulibaWaitor.Command()
			{
				workTime = 2f,
				actionType = DulibaWaitor.ActionType.Action,
				prerequisite = PreRequisite.BothHandsEmpty,
				name = "empty trash",
				callback = () =>
				{
					GetTrash(waitor);
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

	public void GetTrash(DulibaWaitor waitor)
	{
		Carryable garbageBag = new Carryable((ItemScriptableObject)DatabaseManager.GetItem("garbage_bag"));
		garbageBag.data.garbageValue = currentFullness;
		currentFullness = 0;
		thrownItems.Clear();

		waitor.rightHand.SetItem(garbageBag);
	}
}
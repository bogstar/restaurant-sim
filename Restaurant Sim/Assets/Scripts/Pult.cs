using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pult : WorkArea
{
	const int itemSlots = 4;

	public Transform[] itemLocations;
	public Transform trayLocation;
	public Carryable[] items;
	public bool palette;

	protected override void Start()
	{
		base.Start();

		items = new Carryable[4];
	}

	private void Update()
	{
		foreach (var item in items)
		{
			if (item == null)
			{
				continue;
			}

			if (item.Spoil())
			{
				RefreshVisuals();
				OnWorkAreaUpdate?.Invoke(this);
			}

			if (item.Unreturn())
			{
				OnWorkAreaUpdate?.Invoke(this);
			}
		}
		
		if (waitors.Count > 0)
		{
			OnCanvasUpdate?.Invoke();
		}
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		bool foundLoc = false;
		for (int i = 0; i < items.Length; i++)
		{
			var temp = i;

			if (palette)
			{

			}
			else
			{
				if (items[i] == null && foundLoc == false)
				{
					actions.Add(new DulibaWaitor.Command()
					{
						workTime = 0.5f,
						actionType = DulibaWaitor.ActionType.Place,
						name = "place " + item.data.name,
						image = item.data.image,
						pickupIndex = temp,
						callback = () =>
						{
							items[temp] = item;
						},
						lateCallback = () =>
						{
							OnWorkAreaUpdate?.Invoke(this);
							RefreshVisuals();
						}
					});

					foundLoc = true;
				}
				else if (items[i] != null)
				{
					var mixOptions = items[i].GetMixingResult(item);

					if (mixOptions != null)
					{
						var resultItem = mixOptions.resultItem;
						actions.Add(new DulibaWaitor.Command()
						{
							workTime = mixOptions.mixTime,
							actionType = DulibaWaitor.ActionType.Place,
							name = "mix into " + resultItem.name,
							image = resultItem.image,
							pickupIndex = temp,
							callback = () =>
							{
								items[temp] = new Item(resultItem);
								items[temp].SetSpoilageTime((((Item)item).spoilageTime + ((Item)items[temp]).spoilageTime) / 2);
							},
							lateCallback = () =>
							{
								OnWorkAreaUpdate?.Invoke(this);
								RefreshVisuals();
							}
						});
					}
				}
			}
		}

		return actions;
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		Carryable itemLH = waitor.leftHand.GetItem();
		Carryable itemRH = waitor.rightHand.GetItem();

		for (int i = 0; i < items.Length; i++)
		{
			var item = items[i];
			int temp = i;

			if (item != null)
			{
				if (palette)
				{
					actions.Add(new DulibaWaitor.Command()
					{
						workTime = 0.5f,
						reflected = true,
						actionType = DulibaWaitor.ActionType.Pickup,
						name = "pickup " + item.data.name + " package from pult " + (temp + 1),
						image = item.data.image,
						pickupIndex = temp,
						item = item,
						callback = () =>
						{
							items[temp] = null;
						},
						lateCallback = () =>
						{
							OnWorkAreaUpdate?.Invoke(this);
							RefreshVisuals();
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
						name = "pickup " + item.data.name + " from pult " + (temp + 1),
						image = item.data.image,
						pickupIndex = temp,
						item = item,
						callback = () =>
						{
							items[temp] = null;
						},
						lateCallback = () =>
						{
							OnWorkAreaUpdate?.Invoke(this);
							RefreshVisuals();
						}
					});
				}
			}
		}

		return actions;
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "Items on the pult:\n\n";

		int i = 1;
		foreach (var item in items)
		{
			text += "Location " + i + ":\n";
			if (item == null)
			{
				text += "Empty Location.\n\n";
			}
			else
			{
				text += item.data.name;
				if (palette)
				{
					text += " - " + ((Package)item).itemsCount + " items in package.";
				}
				if (item.data.rottable)
				{
					text += " - time to rot " + (((Item)item).spoilageTime - Time.time).ToString("#");
				}
				text += "\n\n";
			}

			i++;
		}

		info.text = text;
		return info;
	}

	public void RefreshVisuals()
	{
		foreach (var t in itemLocations)
		{
			for (int i = t.childCount-1; i > -1; i--)
			{
				Destroy(t.GetChild(i).gameObject);
			}
		}

		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] == null)
			{
				continue;
			}

			GameObject newItemGO = Instantiate(items[i].data.model.gameObject, itemLocations[i]);
			newItemGO.transform.localPosition = Vector3.zero;
		}
	}
}
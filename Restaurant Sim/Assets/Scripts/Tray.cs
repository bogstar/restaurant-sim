using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tray
{
	const int itemSlots = 4;

	public Transform[] itemLocations;
	Carryable[] items;

	public Tray(ItemScriptableObject unsafeCopy, Transform[] itemLocations, Carryable[] items)
	{
		ItemScriptableObject item = (ItemScriptableObject)DatabaseManager.GetItem(unsafeCopy.id);
		/*
		this.data = item;
		this.data.model = DatabaseManager.Instance.packagePrefab.GetComponent<ModelSetup>();
		this.data.rottable = false;*/
	}

	private void Awake()
	{
		items = new Carryable[4];
	}

	/// <summary>
	/// Returns items in a 4 length array with original indices.
	/// </summary>
	/// <returns></returns>
	public WorkArea.ItemWithIndex[] GetAvailableItems()
	{
		WorkArea.ItemWithIndex[] availableItems = new WorkArea.ItemWithIndex[4];

		for (int i = 0; i < items.Length; i++)
		{
			availableItems[i].item = items[i];
			availableItems[i].index = i;
		}

		return availableItems;
	}

	public void RefreshVisuals()
	{
		/*
		foreach (var t in itemLocations)
		{
			if (t.childCount > 0)
				Destroy(t.GetChild(0).gameObject);
		}

		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] == null)
				continue;

			GameObject newItemGO = Instantiate(items[i].data.model.gameObject, itemLocations[i]);
			newItemGO.transform.localPosition = Vector3.zero;
		}*/
	}

	public bool CanInteract(DulibaWaitor waitor)
	{
		return CanPickUp(waitor, DulibaInput.Hand.Either, DulibaInput.ItemChoice.Any)
			|| CanPlaceDown(waitor, DulibaInput.Hand.Either);
	}

	public bool CanPickUp(DulibaWaitor waitor, DulibaInput.Hand hand, DulibaInput.ItemChoice itemChoice)
	{
		/*
		switch (itemChoice)
		{
			case DulibaInput.ItemChoice.Any:
				if (items[0] == null && items[1] == null && items[2] == null && items[3] == null)
					return false;
				break;
			case DulibaInput.ItemChoice.Item1:
				if (items[0] == null)
					return false;
				break;
			case DulibaInput.ItemChoice.Item2:
				if (items[1] == null)
					return false;
				break;
			case DulibaInput.ItemChoice.Item3:
				if (items[2] == null)
					return false;
				break;
			case DulibaInput.ItemChoice.Item4:
				if (items[3] == null)
					return false;
				break;
			default:
				return false;
		}

		switch (hand)
		{
			case DulibaInput.Hand.Left:
				return waitor.GetEmptyHand() == DulibaInput.Hand.Left;
			case DulibaInput.Hand.Right:
				return waitor.GetEmptyHand() == DulibaInput.Hand.Right;
			case DulibaInput.Hand.Either:
				return (waitor.GetEmptyHand() == DulibaInput.Hand.Right) || (waitor.GetEmptyHand() == DulibaInput.Hand.Left) || (waitor.GetEmptyHand() == DulibaInput.Hand.Both);
			case DulibaInput.Hand.Both:
				return waitor.GetEmptyHand() == DulibaInput.Hand.Both;
			default:
				return false;
		}*/

		if (waitor.GetEmptyHand() == DulibaInput.Hand.Both)
		{
			return true;
		}

		return false;
	}

	public int GetFreeSlotCount()
	{
		int count = 0;

		for (int i = 0; i < itemSlots; i++)
		{
			if (items[i] == null)
			{
				count++;
			}
		}

		return count;
	}

	public bool CanPlaceDown(DulibaWaitor waitor, DulibaInput.Hand hand)
	{
		Carryable[] items = new Carryable[2];

		switch (hand)
		{
			case DulibaInput.Hand.Left:
				items[0] = waitor.GetItemsInHand()[0];

				if (items[0] != null && GetFreeSlotCount() > 0)
					return true;
				return false;
			case DulibaInput.Hand.Right:
				items[0] = waitor.GetItemsInHand()[0];

				if (items[0] != null && GetFreeSlotCount() > 0)
					return true;
				return false;
			case DulibaInput.Hand.Either:
				items = waitor.GetItemsInHand();

				if (items[0] != null && GetFreeSlotCount() > 0)
					return true;
				if (items[1] != null && GetFreeSlotCount() > 0)
					return true;
				return false;
			case DulibaInput.Hand.Both:
				items = waitor.GetItemsInHand();

				if (items[0] != null && items[1] != null && GetFreeSlotCount() > 1)
					return true;
				return false;
			default:
				return false;
		}
	}

	public bool PickUp(DulibaWaitor waitor, DulibaInput.Hand hand, int index)
	{
		if (!CanPickUp(waitor, hand, DulibaInput.ItemChoice.None))
		{
			return false;
		}

		return false;

		/*
		DulibaHand h = null;

		switch (hand)
		{
			case DulibaInput.Hand.Left:
				h = waitor.leftHand;
				break;
			case DulibaInput.Hand.Right:
				h = waitor.rightHand;
				break;
		}

		if (h != null && h.GetItem() == null)
		{
			if (items[index] != null)
			{
				h.SetItem(items[index]);
				items[index] = null;
				RefreshVisuals();
			}
		}
		*/
	}

	public void PlaceDown(DulibaWaitor waitor, DulibaInput.Hand hand)
	{
		Carryable item = waitor.GetItemsInHand()[0];
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

		for (int i = 0; i < itemSlots; i++)
		{
			if (items[i] == null)
			{
				items[i] = h.ClearItem();
				RefreshVisuals();
				return;
			}
		}
	}

	/// <summary>
	/// Sets item on the tray while spawning it. Returns item that was just removed. Null if no item or null input.
	/// </summary>
	/// <param name="newItem"></param>
	/// <returns></returns>
	public Carryable SetItem(ItemScriptableObject newItemData)
	{
		if (newItemData == null)
		{
			Debug.LogError("New item data is null.");
			return null;
		}

		return SetItem(new Carryable(newItemData));
	}

	/// <summary>
	/// Sets item on the tray. Returns item that was just removed. Null if no item.
	/// </summary>
	/// <param name="newItem"></param>
	/// <returns></returns>
	public Carryable SetItem(Carryable newItem)
	{
		Carryable oldItem = ClearItem();

		if (newItem == null)
		{
			Debug.LogWarning("New item is null. Use ClearItem instead.");
			return null;
		}

		//GameObject newItemGO = Instantiate(newItem.data.model.gameObject, itemGraphicsPosition);
		//newItemGO.transform.localPosition = Vector3.zero;

		//item = newItem;

		return oldItem;
	}

	/// <summary>
	/// Clear item from the tray. Returns item that was just cleared. Null if no item.
	/// </summary>
	/// <returns></returns>
	public Carryable ClearItem()
	{/*
		Item currentItem = item;

		if (itemGraphicsPosition.childCount > 0)
		{
			Destroy(itemGraphicsPosition.GetChild(0).gameObject);
		}
		
		item = null;*/

		return /*currentItem;*/ null;
	}

	/// <summary>
	/// Peek on the item carried.
	/// </summary>
	/// <returns></returns>
	public Carryable GetItem()
	{
		return null; /*item;*/
	}
}
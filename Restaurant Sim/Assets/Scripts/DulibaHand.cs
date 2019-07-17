using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DulibaHand
{
	public Transform itemLocation;
	public GameObject hand;
	public UnityEngine.UI.Image radialImg;

	Carryable item;

	/// <summary>
	/// Sets item in hand while spawning it. Returns item that was just removed. Null if no item or null input.
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

		return SetItem(new Item(newItemData));
	}

	public void FinishWork()
	{
		radialImg.gameObject.SetActive(false);
	}

	/// <summary>
	/// Sets item in the hand. Returns item that was just removed. Null if no item.
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

		GameObject newItemGO = GameObject.Instantiate(newItem.data.model.gameObject, itemLocation);
		newItemGO.transform.localPosition = Vector3.zero;

		Show(true);
		item = newItem;

		return oldItem;
	}

	/// <summary>
	/// Clear item from the hand. Returns item that was just removed. Null if no item.
	/// </summary>
	/// <returns></returns>
	public Carryable ClearItem()
	{
		Carryable currentItem = item;

		if (itemLocation.childCount > 0)
		{
			GameObject.Destroy(itemLocation.GetChild(0).gameObject);
		}

		item = null;
		Show(false);

		return currentItem;
	}

	/// <summary>
	/// Peek on the item in hand.
	/// </summary>
	/// <returns></returns>
	public Carryable GetItem()
	{
		return item;
	}

	public void Show(bool show)
	{
		hand.SetActive(show);
	}
}
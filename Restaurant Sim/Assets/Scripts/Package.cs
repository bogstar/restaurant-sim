using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : Carryable
{
	public int maxItems;
	public int itemsCount;

	public Package(ItemScriptableObject unsafeCopy, int maxItems) : base(unsafeCopy)
	{
		ItemScriptableObject item = (ItemScriptableObject)DatabaseManager.GetItem(unsafeCopy.id);

		this.data = item;
		this.data.model = DatabaseManager.Instance.packagePrefab.GetComponent<ModelSetup>();
		this.data.rottable = false;
		this.maxItems = this.itemsCount = maxItems;
	}
}
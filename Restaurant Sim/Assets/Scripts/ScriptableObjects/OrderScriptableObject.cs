using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Order", menuName = "MammaMia/Order")]
public class OrderScriptableObject : DatabaseEntryScriptableObject
{
	[SerializeField]
	ItemScriptableObject[] m_items;
	public ItemScriptableObject[] items
	{
		get
		{
			List<ItemScriptableObject> items = new List<ItemScriptableObject>();
			foreach (var item in m_items)
			{
				items.Add(DatabaseManager.GetItem(item.id) as ItemScriptableObject);
			}
			return items.ToArray();
		}
	}

	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "order_" + newString;
	}
}
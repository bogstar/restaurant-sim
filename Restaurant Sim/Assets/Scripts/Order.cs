using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{
	public ItemScriptableObject[] items;

	public Order(ItemScriptableObject[] items)
	{
		this.items = items;
	}

	public Order(OrderScriptableObject order)
	{
		this.items = order.items;
	}
}
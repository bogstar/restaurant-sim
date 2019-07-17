using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseScreenButton : MonoBehaviour
{
	[SerializeField] new Text name;
	[SerializeField] Text description;
	[SerializeField] Text price;
	[SerializeField] Text quantity;
	[SerializeField] Image image;
	[SerializeField] Button button;

	ItemScriptableObject item;

	public void SetButton(ItemScriptableObject item, string price, string quantity)
	{
		this.item = item;
		this.name.text = item.name;
		this.image.sprite = item.image;
		this.description.text = item.description;
		this.price.text = price;
		this.quantity.text = quantity;
	}

	public void SetOnclickEvent(System.Action<ItemScriptableObject> callback)
	{
		button.onClick.AddListener(() => { callback(item); });
	}
}
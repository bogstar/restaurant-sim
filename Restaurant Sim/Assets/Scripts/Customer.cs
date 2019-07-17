using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer : MonoBehaviour
{
	public Image orderImage;
	public Order order;
	public bool orderSatisfied;

	private void Start()
	{
		orderImage.gameObject.SetActive(false);
	}

	public void SatisfyOrder()
	{
		orderSatisfied = true;
		order = null;
		orderImage.gameObject.SetActive(false);
	}

	public void SetOrder(OrderScriptableObject order)
	{
		this.order = new Order(order);
		orderImage.gameObject.SetActive(true);
		orderImage.sprite = order.items[0].image;
	}

	public void SetColor()
	{
		Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
		transform.GetChild(1).GetComponent<Renderer>().material.color = color;
		transform.GetChild(1).GetChild(2).GetComponent<Renderer>().material.color = color;
		transform.GetChild(1).GetChild(3).GetComponent<Renderer>().material.color = color;
	}
}
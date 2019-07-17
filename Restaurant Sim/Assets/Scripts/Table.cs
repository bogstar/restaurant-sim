using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : WorkArea
{
	public int maxPlaces;
	public OrderStatus orderStatus;

	float timeRemaining;
	float timeStarted;
	float patience;

	public Vector2 timeToDecidePerCustomerMinMax;
	public Vector2 timeToEatPerCustomerMinMax;

	public Customer[] customers;
	public Transform[] sittingPlaces;

	public Dictionary<ItemScriptableObject, Customer> allOrderedItems
	{
		get
		{
			Dictionary<ItemScriptableObject, Customer> allItems = new Dictionary<ItemScriptableObject, Customer>();
			foreach (var kvp in allOrders)
			{
				foreach (var item in kvp.Key.items)
				{
					allItems.Add(item, kvp.Value);
				}
			}
			return allItems;
		}
	}
	public Dictionary<Order, Customer> allOrders
	{
		get
		{
			Dictionary<Order, Customer> orders = new Dictionary<Order, Customer>();
			foreach (var customer in customers)
			{
				if (!customer.orderSatisfied)
				{
					orders.Add(customer.order, customer);
				}
			}
			return orders;
		}
	}

	private void Update()
	{
		switch (orderStatus)
		{
			case OrderStatus.Deciding:
				if (Time.time > timeRemaining)
				{
					orderStatus = OrderStatus.Decided;
					patience = Time.time + Random.Range(40f, 60f);
					OnWorkAreaUpdate?.Invoke(this);
				}
				OnCanvasUpdate?.Invoke();
				break;
			case OrderStatus.Eating:
				if (Time.time > timeRemaining)
				{
					orderStatus = OrderStatus.Paying;
					patience = Time.time + Random.Range(40f, 60f);
					OnWorkAreaUpdate?.Invoke(this);
				}
				OnCanvasUpdate?.Invoke();
				break;
			case OrderStatus.Decided:
			case OrderStatus.Paying:
			case OrderStatus.Arrived:
			case OrderStatus.Taken:
				if (Time.time > patience)
				{
					CustomerManager.Instance.EmptyTable(this, false);
					orderStatus = OrderStatus.Empty;
					OnWorkAreaUpdate?.Invoke(this);
				}
				OnCanvasUpdate?.Invoke();
				break;
		}
	}

	void DropFoodMenu()
	{
		orderStatus = OrderStatus.Deciding;

		timeStarted = Time.time;
		timeRemaining = Time.time + Random.Range(timeToDecidePerCustomerMinMax.x, timeToDecidePerCustomerMinMax.y) * GetCustomerCount();
		OnWorkAreaUpdate?.Invoke(this);
	}

	int foodLeft = 0;

	public override WorkCanvasInfo GetCanvasData()
	{
		WorkCanvasInfo info = new WorkCanvasInfo();

		info.title = name;
		string text = "";

		switch (orderStatus)
		{
			case OrderStatus.Arrived:
				text += "Waiting for food menu.\n\n";
				text += "Patience: " + (patience - Time.time).ToString("##");
				break;
			case OrderStatus.Deciding:
				text += "Still deciding.\n\n";
				text += "Progress: " + (Mathf.InverseLerp(timeStarted, timeRemaining, Time.time) * 100).ToString("###") + "%";
				break;
			case OrderStatus.Eating:
				text += "Eating.\n\n";
				text += "Progress: " + (Mathf.InverseLerp(timeStarted, timeRemaining, Time.time) * 100).ToString("###") + "%";
				break;
			case OrderStatus.Decided:
				text += "Ready to provide order.\n\n";
				text += "Patience: " + (patience - Time.time).ToString("##");
				break;
			case OrderStatus.Taken:
				text += "Customers:\n\n";
				foreach (var customer in customers)
				{
					if (customer != null && !customer.orderSatisfied)
					{
						text += "Wants " + customer.order.items[0].name + ".\n";
					}
				}
				text += "\nPatience: " + (patience - Time.time).ToString("##");
				break;
			case OrderStatus.Paying:
				text += "Ready to pay.\n\n";
				text += "Patience: " + (patience - Time.time).ToString("##");
				break;
		}

		info.text = text;
		return info;
	}

	void TakeOrder()
	{
		foreach (var customer in customers)
		{
			if (customer != null)
			{
				foodLeft++;
				customer.SetOrder(CustomerManager.Instance.possibleOrders[Random.Range(0, CustomerManager.Instance.possibleOrders.Length)]);
			}
		}

		orderStatus = OrderStatus.Taken;
		patience = Time.time + Random.Range(60f, 80f);
		OnWorkAreaUpdate?.Invoke(this);
	}

	void Serve(Carryable food)
	{
		foreach (var kvp in allOrderedItems)
		{
			var item = kvp.Key;
			var customer = kvp.Value;

			if (food.data.id == item.id)
			{
				foodLeft--;
				customer.SatisfyOrder();

				if (foodLeft < 1)
				{
					orderStatus = OrderStatus.Eating;
					timeStarted = Time.time;
					timeRemaining = Time.time + Random.Range(timeToEatPerCustomerMinMax.x, timeToEatPerCustomerMinMax.y) * GetCustomerCount();
				}

				OnWorkAreaUpdate?.Invoke(this);
				break;
			}
		}

		OnWorkAreaUpdate?.Invoke(this);
	}

	void TakeMoney()
	{
		CustomerManager.Instance.EmptyTable(this, true);
		orderStatus = OrderStatus.Empty;
		foodLeft = 0;
		OnWorkAreaUpdate?.Invoke(this);
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		switch (orderStatus)
		{
			// Customers are waiting for the food menu.
			case OrderStatus.Arrived:
				if (item.data.id == "food_menu")
				{
					actions.Add(new DulibaWaitor.Command()
					{
						workTime = 1f,
						actionType = DulibaWaitor.ActionType.Place,
						name = "give food menu",
						image = item.data.image,
						callback = () =>
						{
							DropFoodMenu();
						},
						lateCallback = () =>
						{
							OnWorkAreaUpdate?.Invoke(this);
						}
					});
				}
				break;
			case OrderStatus.Taken:
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 0.5f,
					actionType = DulibaWaitor.ActionType.Place,
					name = "serve " + item.data.name,
					image = item.data.image,
					callback = () =>
					{
						Serve(item);
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
				break;
		}

		return actions;
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		Carryable itemLH = waitor.leftHand.GetItem();
		Carryable itemRH = waitor.rightHand.GetItem();

		switch (orderStatus)
		{
			case OrderStatus.Decided:
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 1f * GetCustomerCount(),
					actionType = DulibaWaitor.ActionType.Action,
					name = "take order",
					callback = () =>
					{
						TakeOrder();
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
				break;
			case OrderStatus.Paying:
				actions.Add(new DulibaWaitor.Command()
				{
					workTime = 1.5f,
					actionType = DulibaWaitor.ActionType.Action,
					name = "take money",
					callback = () =>
					{
						TakeMoney();
					},
					lateCallback = () =>
					{
						OnWorkAreaUpdate?.Invoke(this);
					}
				});
				break;
		}

		return actions;
	}

	public void SetUpTable(Customer[] customers)
	{
		this.customers = customers;
		for (int i = 0; i < this.customers.Length; i++)
		{
			this.customers[i].transform.SetParent(sittingPlaces[i]);
			this.customers[i].transform.localPosition = new Vector3(0f, -.6f, 0f);
			this.customers[i].transform.localRotation = Quaternion.identity;
		}

		patience = Time.time + Random.Range(40f, 60f);

		orderStatus = OrderStatus.Arrived;
	}

	int GetCustomerCount()
	{
		int count = 0;
		foreach (var customer in customers)
		{
			if (customer != null)
			{
				count++;
			}
		}

		return count;
	}

	public enum OrderStatus
	{
		/// <summary>No guests.</summary>
		Empty,

		/// <summary>Waiting for food menu.</summary>
		Arrived,

		/// <summary>Deciding from menu.</summary>
		Deciding,

		/// <summary>Waiting to provide order.</summary>
		Decided,

		/// <summary>Waiting for food.</summary>
		Taken,

		/// <summary>Eating food.</summary>
		Eating,

		/// <summary>Waiting for bill.</summary>
		WaitingBill,

		/// <summary>Paying.</summary>
		Paying
	}
}
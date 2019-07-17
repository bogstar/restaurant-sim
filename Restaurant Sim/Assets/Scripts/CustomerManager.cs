using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
	public GameObject customerPrefab;
	[SerializeField] [System.Obsolete("Use possibleOrders instead.")] OrderScriptableObject[] m_possibleOrders;
	public OrderScriptableObject[] possibleOrders
	{
		get
		{
			List<OrderScriptableObject> items = new List<OrderScriptableObject>();
			foreach (var item in m_possibleOrders)
			{
				items.Add(DatabaseManager.GetItem(item.id) as OrderScriptableObject);
			}
			return items.ToArray();
		}
	}
	public List<Table> tables;

	float guestOfftenty;
	float timerEnd;

	private void Start()
	{
		guestOfftenty = 30f;
		timerEnd = Time.time + Random.Range(guestOfftenty / 3, guestOfftenty);
	}

	private void Update()
	{
		if (GameManager.Instance.time > 14400 && GameManager.Instance.time < 25200)
		{
			guestOfftenty = 10f;
		}
		else if (GameManager.Instance.time > 36600 && GameManager.Instance.time < 46800)
		{
			guestOfftenty = 10f;
		}
		else
		{
			guestOfftenty = 30f;
		}

		if (Time.time > timerEnd)
		{
			OccupyTable();
			timerEnd = Time.time + Random.Range(guestOfftenty / 3, guestOfftenty);
		}
	}

	public void OccupyTable()
	{
		if (tables.Count < 1)
		{
			return;
		}

		Table table = tables[Random.Range(0, tables.Count)];
		tables.Remove(table);
		int customersCount = Random.Range(1, table.maxPlaces + 1);

		List<Customer> customers = new List<Customer>();
		for (int i = 0; i < customersCount; i++)
		{
			Customer customer = Instantiate(customerPrefab).GetComponent<Customer>();
			customer.SetColor();
			customers.Add(customer);
		}

		table.SetUpTable(customers.ToArray());
	}

	public void EmptyTable(Table table, bool happy)
	{
		for (int i = table.customers.Length - 1; i > -1; i--)
		{
			if (table.customers[i] != null)
			{
				Destroy(table.customers[i].gameObject);
				table.customers[i] = null;
			}
		}

		if (happy)
		{
			GameManager.Instance.money += 30;
			GameManager.Instance.moneyText.text = "Money: $" + GameManager.Instance.money.ToString();
		}

		tables.Add(table);
	}
}
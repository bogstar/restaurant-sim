using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public CharacterScriptableObject startingCharacter;

	public int money;
	public int time;

	public UnityEngine.UI.Text moneyText;
	public UnityEngine.UI.Text timeText;

	public ItemScriptableObject[] deliverableItems;
	public Pult[] palettes;

	private void Start()
	{
		time = 0;
	}

	private void Update()
	{
		time += (int)(150 * Time.deltaTime);

		if (time > 57600)
		{
			time = 0;
			money = 200;
			Application.Quit();
		}

		timeText.text = "Time: " + (time / 60 / 60 + 8).ToString("00") + ":" + ((time / 60 % 60)).ToString("00");
	}

	public void DeliverItem(ItemScriptableObject item)
	{
		Deliver(item);
	}

	void Deliver(ItemScriptableObject item)
	{
		if (CanDeliver() && money >= 20)
		{
			money -= 20;

			Pult palette = null;
			int index = -1;

			foreach (var p in palettes)
			{
				for (int i = 0; i < p.items.Length; i++)
				{
					if (p.items[i] == null)
					{
						palette = p;
						index = i;
					}
				}
			}

			if (palette == null)
			{
				return;
			}

			palette.items[index] = new Package(item, 6);
			palette.RefreshVisuals();

			moneyText.text = "Money: $" + money.ToString();
		}
	}

	bool CanDeliver()
	{
		foreach (var palette in palettes)
		{
			foreach (var item in palette.items)
			{
				if (item == null)
				{
					return true;
				}
			}
		}

		return false;
	}
}
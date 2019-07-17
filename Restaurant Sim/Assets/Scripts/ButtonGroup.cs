using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
	public GameObject singleButtonPrefab;
	public RectTransform deathPoint;

	List<SingleButton> buttons = new List<SingleButton>();
	SingleButton eButton;

	List<DulibaHUD.ButtonInfo> newButtons = new List<DulibaHUD.ButtonInfo>();

	DulibaInputKeys inputKeys;

	int count = -1;

	public bool requireUpdate;

	private void Awake()
	{
		/*
		eButton = Instantiate(singleButtonPrefab, transform).GetComponent<SingleButton>();
		eButton.transform.localPosition = new Vector3(0f, -187.5f);
		eButton.SetTargetPosition(new Vector3(0f, -187.5f));
		SetActionButton(null, null, null, null, false);*/
	}

	private void Start()
	{
		inputKeys = FindObjectOfType<DulibaInput>().inputKeys;
	}

	void ClearButtons()
	{
		for (int i = buttons.Count - 1; i > -1; i--)
		{
			buttons[i].SetTargetPosition(new Vector3(deathPoint.localPosition.x, buttons[i].transform.localPosition.y));
			buttons[i].Die();
			buttons.RemoveAt(i);
		}
	}

	public void ClearNewButtons()
	{
		newButtons.Clear();
	}

	public void UpdateButton(DulibaHUD.ButtonInfo newButton)
	{
		foreach (var b in buttons)
		{
			if (b.buttonInfo.button == newButton.button)
			{
				b.SetText(newButton.name, newButton.quantity, newButton.button, newButton.image);
				b.buttonInfo = newButton;

				if (!newButton.enabled)
				{
					b.transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, .75f);
					b.image.color = new Color(1f, 1f, 1f, .75f);
				}
				else
				{
					b.transform.GetChild(0).GetComponent<Image>().color = Color.white;
					b.image.color = Color.white;
				}

				break;
			}
		}
	}

	private void LateUpdate()
	{
		if (!requireUpdate)
			return;

		requireUpdate = false;

		ClearButtons();

		int count = 0;

		foreach (var newButton in newButtons)
		{
			SingleButton button = Instantiate(singleButtonPrefab, transform).GetComponent<SingleButton>();
			button.transform.localPosition = new Vector3(deathPoint.localPosition.x, 125f * count - 187.5f);
			button.SetTargetPosition(new Vector3(0f, 125f * count - 187.5f));

			button.SetText(newButton.name, newButton.quantity, newButton.button, newButton.image);
			button.buttonInfo = newButton;

			if (!newButton.enabled)
			{
				button.transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, .75f);
				button.image.color = new Color(1f, 1f, 1f, .75f);
			}
			else
			{
				button.transform.GetChild(0).GetComponent<Image>().color = Color.white;
				button.image.color = Color.white;

#if UNITY_ANDROID
				Button uiButton = button.gameObject.AddComponent<Button>();
				uiButton.targetGraphic = button.transform.GetChild(0).GetComponent<Image>();

				EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
				var pointerDown = new EventTrigger.Entry();
				pointerDown.eventID = EventTriggerType.PointerDown;
				pointerDown.callback.AddListener((e) => inputKeys.ButtonPressedDown(newButton.button));
				trigger.triggers.Add(pointerDown);

				trigger = button.gameObject.AddComponent<EventTrigger>();
				pointerDown = new EventTrigger.Entry();
				pointerDown.eventID = EventTriggerType.PointerUp;
				pointerDown.callback.AddListener((e) => inputKeys.ButtonPressedUp(newButton.button));
				trigger.triggers.Add(pointerDown);
#endif
			}

			buttons.Add(button);
			count++;
		}

		newButtons.Clear();
	}

	public void DestroyButton(string button)
	{
		for (int i = buttons.Count - 1; i > -1; i--)
		{
			if (buttons[i].buttonInfo.button == button)
			{
				buttons[i].SetTargetPosition(new Vector3(deathPoint.localPosition.x, buttons[i].transform.localPosition.y));
				buttons[i].Die();
				buttons.RemoveAt(i);

				break;
			}
		}
	}

	public void CreateButton(DulibaHUD.ButtonInfo newButton)
	{
		//DestroyButton(newButton.button);
		/*
		SingleButton button = Instantiate(singleButtonPrefab, transform).GetComponent<SingleButton>();
		button.transform.localPosition = new Vector3(deathPoint.localPosition.x, 125f * buttons.Count - 187.5f);
		button.SetTargetPosition(new Vector3(0f, 125f * buttons.Count - 187.5f));

		button.SetText(newButton.name, newButton.quantity, newButton.button, newButton.image);
		button.buttonInfo = newButton;

		if (!newButton.enabled)
		{
			button.transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, .75f);
			button.image.color = new Color(1f, 1f, 1f, .75f);
		}
		else
		{
			button.transform.GetChild(0).GetComponent<Image>().color = Color.white;
			button.image.color = Color.white;
		}*/

		newButtons.Add(newButton);
		//buttons.Add(button);
		//count++;

		/*
		Button uiButton = button.gameObject.AddComponent<Button>();
		uiButton.targetGraphic = button.transform.GetChild(0).GetComponent<Image>();

		EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
		var pointerDown = new EventTrigger.Entry();
		pointerDown.eventID = EventTriggerType.PointerDown;
		pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().ButtonPressedDown(letter == "L" ? "a1" : "n1"));
		trigger.triggers.Add(pointerDown);

		trigger = button.gameObject.AddComponent<EventTrigger>();
		pointerDown = new EventTrigger.Entry();
		pointerDown.eventID = EventTriggerType.PointerUp;
		pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().ButtonPressedUp(letter == "L" ? "a1" : "n1"));
		trigger.triggers.Add(pointerDown);*/

	}

	/*
	public void SetEButton(string button)
	{
		Button uiButton = eButton.gameObject.AddComponent<Button>();
		uiButton.targetGraphic = eButton.transform.GetChild(0).GetComponent<Image>();

		EventTrigger trigger = eButton.gameObject.AddComponent<EventTrigger>();
		var pointerDown = new EventTrigger.Entry();
		pointerDown.eventID = EventTriggerType.PointerDown;
		pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedDown(button));
		trigger.triggers.Add(pointerDown);

		trigger = eButton.gameObject.AddComponent<EventTrigger>();
		pointerDown = new EventTrigger.Entry();
		pointerDown.eventID = EventTriggerType.PointerUp;
		pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedUp(button));
		trigger.triggers.Add(pointerDown);
	}*/

	public void CollapseButtons(int index)
	{
		for (int i = buttons.Count - 1; i > -1; i--)
		{
			if (i == index)
			{
				buttons[i].SetTargetPosition(new Vector3(0f, -187.5f));
				buttons[i].SetButtonPress(null);
				buttons[i].SetQuantity(null);
			}
			else
			{
				buttons[i].SetTargetPosition(new Vector3(deathPoint.localPosition.x, 125f * i - 187.5f + 125f));
			}

			buttons[i].Die();
			buttons.RemoveAt(i);
		}

		count = 1;
	}

	public void SetActionButton(string name, string quantity, string buttonPress, Sprite Image, bool clickable)
	{
		string actualButtonPress = buttonPress;

		if (!clickable)
		{
			eButton.transform.GetChild(0).GetComponent<Image>().color = new Color(.5f, .5f, .5f, .75f);
			eButton.image.color = Color.clear;
			actualButtonPress = null;
		}
		else
		{
			eButton.transform.GetChild(0).GetComponent<Image>().color = Color.white;
			eButton.image.color = Color.white;
		}

		eButton.SetText(name, quantity, actualButtonPress, Image);
	}

	/*
	public void SetButtons(Carryable[] items, string letter)
	{
		if (count == items.Length)
		{
			return;
		}

		for (int i = buttons.Count - 1; i > -1; i--)
		{
			buttons[i].Die();
			buttons[i].SetTargetPosition(new Vector3(deathPoint.localPosition.x, 125f * i - 187.5f + 125f));
			buttons.RemoveAt(i);
		}

		for (int i = 0; i < items.Length; i++)
		{
			SingleButton button = Instantiate(singleButtonPrefab, transform).GetComponent<SingleButton>();
			button.transform.localPosition = new Vector3(deathPoint.localPosition.x, 125f * i - 187.5f + 125f);
			button.SetTargetPosition(new Vector3(0f, 125f * i - 187.5f + 125f));

			button.SetText(items[i].data.name, null, "R" + (i + 1), items[i].data.image);
			buttons.Add(button);

			if (i != 0)
				continue;

			Button uiButton = button.gameObject.AddComponent<Button>();
			uiButton.targetGraphic = button.transform.GetChild(0).GetComponent<Image>();

			EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
			var pointerDown = new EventTrigger.Entry();
			pointerDown.eventID = EventTriggerType.PointerDown;
			pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedDown(letter == "L" ? "a1" : "n1"));
			trigger.triggers.Add(pointerDown);

			trigger = button.gameObject.AddComponent<EventTrigger>();
			pointerDown = new EventTrigger.Entry();
			pointerDown.eventID = EventTriggerType.PointerUp;
			pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedUp(letter == "L" ? "a1" : "n1"));
			trigger.triggers.Add(pointerDown);
		}

		count = items.Length;
	}*/
	/*
	public void SetButtons(WorkArea.ItemDataWithCount[] items, string letter)
	{
		if (count == items.Length)
		{
			return;
		}

		for (int i = buttons.Count - 1; i > -1; i--)
		{
			buttons[i].Die();
			buttons[i].SetTargetPosition(new Vector3(deathPoint.localPosition.x, 125f * i - 187.5f + 125f));
			buttons.RemoveAt(i);
		}

		for (int i = 0; i < items.Length; i++)
		{
			SingleButton button = Instantiate(singleButtonPrefab, transform).GetComponent<SingleButton>();
			button.name = "Button " + i;
			button.transform.localPosition = new Vector3(deathPoint.localPosition.x, 125f * i - 187.5f + 125f);
			button.SetTargetPosition(new Vector3(0f, 125f * i - 187.5f + 125f));

			string itemCount = "∞";
			if (!items[i].infinite)
				itemCount = items[i].count.ToString();

			button.SetText(items[i].item.name, itemCount, letter + (i + 1), items[i].item.image);
			buttons.Add(button);

			if (i != 0)
				continue;

			Button uiButton = button.gameObject.AddComponent<Button>();
			uiButton.targetGraphic = button.transform.GetChild(0).GetComponent<Image>();

			EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();
			var pointerDown = new EventTrigger.Entry();
			pointerDown.eventID = EventTriggerType.PointerDown;
			pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedDown(letter == "L" ? "a1" : "n1"));
			trigger.triggers.Add(pointerDown);

			trigger = button.gameObject.AddComponent<EventTrigger>();
			pointerDown = new EventTrigger.Entry();
			pointerDown.eventID = EventTriggerType.PointerUp;
			pointerDown.callback.AddListener((e) => FindObjectOfType<DulibaInput>().inputKeys.ButtonPressedUp(letter == "L" ? "a1" : "n1"));
			trigger.triggers.Add(pointerDown);
		}

		count = items.Length;
	}*/
}
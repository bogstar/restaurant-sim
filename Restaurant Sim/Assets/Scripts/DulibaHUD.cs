using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DulibaHUD : Singleton<DulibaHUD>
{
	public ButtonGroup leftButtons;
	public ButtonGroup rightButtons;

	DulibaInput dulibaInput;
	DulibaWaitor dulibaWaitor;

	Dictionary<string, DulibaWaitor.Command> currentMap = new Dictionary<string, DulibaWaitor.Command>();

	public WorkAreaCanvas workCanvas;

	private void Start()
	{
		dulibaInput = FindObjectOfType<DulibaInput>();

		if (dulibaInput == null)
		{
			Debug.LogError("No input found.");
			return;
		}

		// After this point, input is not null
		dulibaWaitor = dulibaInput.GetComponent<DulibaWaitor>();

		//leftButtons.SetEButton("q");
		//rightButtons.SetEButton("e");
		dulibaInput.OnMappingChanged += (newMapping) =>
		{
			leftButtons.requireUpdate = true;
			rightButtons.requireUpdate = true;
			OnMappingChanged(newMapping);
		};

		dulibaWaitor.OnWorkAreaChanged += OnWorkAreaChanged;
		dulibaWaitor.OnCanvasUpdate += OnCanvasUpdate;
	}

	public enum Change
	{
		Nothing,
		Create,
		Swap,
		Destroy
	}

	void OnCanvasUpdate()
	{
		if (dulibaWaitor.workArea == null)
		{
			return;
		}

		var data = dulibaWaitor.workArea.GetCanvasData();

		workCanvas.title.text = data.title;
		workCanvas.text.text = data.text;
	}

	void OnWorkAreaChanged(WorkArea newWorkArea)
	{
		if (newWorkArea == null)
		{
			workCanvas.gameObject.SetActive(false);
		}
		else
		{
			workCanvas.gameObject.SetActive(true);

			OnCanvasUpdate();
		}
	}

	void OnMappingChanged(Dictionary<string, DulibaWaitor.Command> newMap)
	{
		// If this happens twice per frame, the second occurance is the new black.
		leftButtons.ClearNewButtons();
		rightButtons.ClearNewButtons();

		foreach (var kvp in newMap)
		{
			string commandKey = kvp.Key;
			DulibaWaitor.Command newCommand = kvp.Value;

			Change change = Change.Create;
			ButtonGroup buttonsToChange = rightButtons;

			if (currentMap.ContainsKey(commandKey))
			{
				DulibaWaitor.Command oldCommand = currentMap[commandKey];

				if (oldCommand.disabled.HasValue != newCommand.disabled.HasValue)
				{
					change = Change.Nothing;
				}
				if (oldCommand.callback != newCommand.callback)
				{
					change = Change.Create;
				}
			}

			if (newCommand.leftHand)
				buttonsToChange = leftButtons;

			string quantity = "";

			if (dulibaWaitor.workArea is Shelf import)
			{
				foreach (var action in dulibaWaitor.actions)
				{
					if (action.command == commandKey.ToLower())
					{
						if (!import.items[action.pickupIndex].infinite)
						{
							quantity = import.items[action.pickupIndex].itemCount.ToString();
						}
						break;
					}
				}
			}
			/*
			switch (change)
			{
				
				case Change.Nothing:
					buttonsToChange.UpdateButton(new ButtonInfo() { name = newCommand.name, button = commandKey, enabled = !newCommand.disabled.HasValue, image = newCommand.image });
					break;
				case Change.Create:*/
					buttonsToChange.CreateButton(new ButtonInfo() { quantity = quantity, name = newCommand.name, button = commandKey, enabled = !newCommand.disabled.HasValue, image = newCommand.image });
					/*break;
			}
			*/
			//buttonsToChange.DisplayButton(new ButtonInfo() { name = command.name, button = commandKey, enabled = !command.disabled.HasValue, image = command.image });
		}
		/*
		foreach (var kvp in currentMap)
		{
			string commandKey = kvp.Key;
			DulibaWaitor.Command command = kvp.Value;
			ButtonGroup buttonsToChange = rightButtons;

			if (command.leftHand)
				buttonsToChange = leftButtons;

			if (!newMap.ContainsKey(kvp.Key))
				buttonsToChange.DestroyButton(kvp.Key);
		}*/

		currentMap = newMap;
	}

	public struct ButtonInfo
	{
		public string button;
		public string name;
		public string quantity;
		public Sprite image;
		public bool enabled;
	}

	void OnHudUpdate()
	{
		/*
		DulibaInput.InputInfo inputInfo = dulibaInput.inputInfo;

		WorkArea workArea = dulibaWaitor.currentWorkArea;
		Item[] items = dulibaWaitor.GetItemsInHand(DulibaInput.Hand.Both);
		Item leftItem = items?[0];
		Item rightItem = items?[1];

		if (inputInfo.leftItemPickedUp.itemPickedUp)
		{
			leftButtons.CollapseButtons(inputInfo.leftItemPickedUp.index);
			dulibaInput.inputInfo.leftItemPickedUp.itemPickedUp = false;
		}
		else if (inputInfo.displayLeftHand)
		{
			leftButtons.SetButtons(new WorkArea.ItemDataWithCount[0], null);

			bool display = false;
			if (leftItem != null)
				display = true;
			
			DisplayOneHand(leftItem, dulibaInput.leftHandPickup.ToString(), leftButtons, display);
		}
		else
		{
			DisplayOneHand(leftItem, dulibaInput.leftHandPickup.ToString(), leftButtons, false);
			if (workArea.shelfable != null)
			{
				leftButtons.SetButtons(inputInfo.leftHandCountableItems, "L");
			}
			if (workArea.pultable != null)
			{
				leftButtons.SetButtons(inputInfo.leftHandItems, "L");
			}
		}

		if (inputInfo.rightItemPickedUp.itemPickedUp)
		{
			rightButtons.CollapseButtons(inputInfo.rightItemPickedUp.index);
			dulibaInput.inputInfo.rightItemPickedUp.itemPickedUp = false;
		}
		else if (inputInfo.displayRightHand)
		{
			rightButtons.SetButtons(new WorkArea.ItemDataWithCount[0], null);

			bool display = false;
			if (rightItem != null)
				display = true;

			DisplayOneHand(rightItem, dulibaInput.rightHandPickup.ToString(), rightButtons, display);
		}
		else
		{
			DisplayOneHand(rightItem, dulibaInput.rightHandPickup.ToString(), rightButtons, false);
			if (workArea.shelfable != null)
			{
				rightButtons.SetButtons(inputInfo.rightHandCountableItems, "R");
			}
			if (workArea.pultable != null)
			{
				rightButtons.SetButtons(inputInfo.rightHandItems, "R");
			}
		}*/
	}

	void DisplayOneHand(Carryable item, string buttonPickup, ButtonGroup buttons, bool active)
	{
		string name = null;
		Sprite image = null;

		if (item != null)
		{
			name = item.data.name;
			image = item.data.image;
		}

		buttons.SetActionButton(name, null, buttonPickup, image, active);
	}
}
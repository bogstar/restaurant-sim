using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DulibaMove))]
[RequireComponent(typeof(DulibaWaitor))]
public class DulibaInput : MonoBehaviour
{
	public DulibaInputKeys inputKeys;

	DulibaMove move;
	DulibaWaitor waitor;

	public event System.Action<Dictionary<string, DulibaWaitor.Command>> OnMappingChanged;

	Dictionary<string, DulibaWaitor.Command> map = new Dictionary<string, DulibaWaitor.Command>();

	List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

	private void Start()
	{
		move = GetComponent<DulibaMove>();
		waitor = GetComponent<DulibaWaitor>();

		inputKeys.RegisterKeyActionCallback(OnKeyAction);

		waitor.OnCommandsChanged += OnCommandsChanged;
		waitor.OnWorkAreaRemoved += OnWorkAreaRemoved;
		waitor.OnWorkAreaChanged += (workArea) =>
		{
			OnWorkAreaChanged(workArea);
			HideRadials();
		};
	}

	private void Update()
	{
		inputKeys.HandleMovementInput();
		move.SetInput(inputKeys.calculatedVector);
		inputKeys.HandleWaitoringInput();
	}

	void OnKeyAction(DulibaInputKeys.ButtonMap buttonMap)
	{
		List<string> lButtons = new List<string>();
		List<string> rButtons = new List<string>();
		List<string> aButtons = new List<string>();
		List<string> buttons = new List<string>();

		foreach (var downButton in buttonMap.downButtons)
		{
			if (downButton.StartsWith("L"))
			{
				lButtons.Add(downButton);
			}
			if (downButton.StartsWith("R"))
			{
				rButtons.Add(downButton);
			}
		}

		if (lButtons.Count > 1)
		{
			var random = lButtons[Random.Range(0, lButtons.Count)];
			lButtons.Remove(random);

			for (int i = buttonMap.downButtons.Count-1; i > -1; i--)
			{
				if (buttonMap.downButtons[i] != random)
				{
					buttonMap.downButtons.RemoveAt(i);
				}
			}
		}
		lButtons.Clear();

		if (rButtons.Count > 1)
		{
			var random = rButtons[Random.Range(0, rButtons.Count)];
			rButtons.Remove(random);

			for (int i = buttonMap.downButtons.Count - 1; i > -1; i--)
			{
				if (buttonMap.downButtons[i] != random)
				{
					buttonMap.downButtons.RemoveAt(i);
				}
			}
		}
		rButtons.Clear();

		foreach (var b in buttonMap.downButtons)
		{

			if (b.StartsWith("L") || b.StartsWith("Q"))
			{/*
				if (waitor.actionMap.GetItemWithSubstring(b.Substring(1), 1) > -1/* && waitor.currentWorkArea is Import import && import.items[waitor.actions[waitor.actionMap.GetItemWithSubstring(b.Substring(1), 1)].pickupIndex].itemCount < 2*//*)
				{
					
				}
				else
				{*/
					lButtons.Add(b);
				//}
			}
			if (b.StartsWith("R") || b.StartsWith("E"))
			{/*
				if (waitor.actionMap.GetItemWithSubstring(b.Substring(1), 1) > -1)
				{

				}
				else
				{*/
					rButtons.Add(b);
				//}
			}
		}

		if (lButtons.Count > 0)
			buttons.Add(lButtons[Random.Range(0, lButtons.Count)]);

		if (rButtons.Count > 0)
			buttons.Add(rButtons[Random.Range(0, rButtons.Count)]);

		if (aButtons.Count > 0)
			KeyAction(aButtons[Random.Range(0, aButtons.Count)], DulibaInputKeys.KeyPress.Down);

		if (buttons.Count == 2)
		{
			if (buttons[0].Substring(1) == buttons[0].Substring(1))
			{
				KeyAction(buttons[Random.Range(0, buttons.Count)], DulibaInputKeys.KeyPress.Down);
			}
			else
			{
				KeyAction(buttons[0], DulibaInputKeys.KeyPress.Down);
				KeyAction(buttons[1], DulibaInputKeys.KeyPress.Down);
			}
		}
		else if (buttons.Count == 1)
		{
			KeyAction(buttons[0], DulibaInputKeys.KeyPress.Down);
		}

		foreach (var b in buttonMap.pressedButtons)
		{
			KeyAction(b, DulibaInputKeys.KeyPress.Pressed);
		}

		foreach (var b in buttonMap.upButtons)
		{
			KeyAction(b, DulibaInputKeys.KeyPress.Up);
		}
	}

	void KeyAction(string button, DulibaInputKeys.KeyPress phase)
	{
		WorkArea currentWorkArea = waitor.workArea;

		foreach (var kvp in map)
		{
			string key = kvp.Key;
			DulibaWaitor.Command command = kvp.Value;

			if (button == key)
			{
				switch (phase)
				{
					case DulibaInputKeys.KeyPress.Down:
						if (command.disabled == null)
						{
							waitor.actionMap.Add(new DulibaWaitor.ActionMap(key, Time.time, Time.time + command.workTime * (1 / waitor.characterData.workTimeMultiplier), command));
							var action = waitor.actionMap.GetByCommandString(key);
							float perc = Mathf.InverseLerp(action.startAction, action.endAction, Time.time);
							switch (command.leftHand)
							{
								case true:
									waitor.leftHand.radialImg.gameObject.SetActive(true);
									waitor.leftHand.radialImg.fillAmount = 0;
									break;
								default:
									waitor.rightHand.radialImg.gameObject.SetActive(true);
									waitor.rightHand.radialImg.fillAmount = 0;
									break;
							}
						}
						break;
					case DulibaInputKeys.KeyPress.Pressed:
						if (waitor.actionMap.Contains(key))
						{
							if (Time.time > waitor.actionMap.GetByCommandString(key).endAction)
							{
								command.callback?.Invoke();
								command.lateCallback?.Invoke();
								waitor.actionMap.Remove(key);
								switch (command.leftHand)
								{
									case true:
										waitor.leftHand.radialImg.gameObject.SetActive(false);
										break;
									default:
										waitor.rightHand.radialImg.gameObject.SetActive(false);
										break;
								}
							}
							else
							{
								var action = waitor.actionMap.GetByCommandString(key);
								float perc = Mathf.InverseLerp(action.startAction, action.endAction, Time.time);
								switch (command.leftHand)
								{
									case true:
										waitor.leftHand.radialImg.fillAmount = perc;
										break;
									default:
										waitor.rightHand.radialImg.fillAmount = perc;
										break;
								}
							}
						}
						break;
					case DulibaInputKeys.KeyPress.Up:
						if (waitor.actionMap.Contains(key))
						{
							waitor.actionMap.Remove(key);
							switch (command.leftHand)
							{
								case true:
									waitor.leftHand.radialImg.gameObject.SetActive(false);
									break;
								default:
									waitor.rightHand.radialImg.gameObject.SetActive(false);
									break;
							}
						}
						break;
				}
			}
		}
	}

	public void HideRadials()
	{
		waitor.leftHand.radialImg.gameObject.SetActive(false);
		waitor.rightHand.radialImg.gameObject.SetActive(false);
	}

	void OnWorkAreaRemoved(WorkArea oldArea)
	{
		waitor.actionMap.Clear();
		oldArea.Highlight(false);
	}

	void OnWorkAreaChanged(WorkArea newArea)
	{
		if (newArea != null)
		{
			newArea.Highlight(true);
		}
	}

	void OnCommandsChanged(List<DulibaWaitor.Command> newCommands)
	{
		actions = new List<DulibaWaitor.Command>(newCommands);
		map = new Dictionary<string, DulibaWaitor.Command>();

		int r = 0;
		int l = 0;
		string[] rightSideKeys = new string[] { "E", "R1", "R2", "R3", "R4" };
		string[] leftSideKeys = new string[] { "Q", "L1", "L2", "L3", "L4" };

		foreach (var action in actions)
		{
			if (action.command.StartsWith("l"))
			{
				map.Add(leftSideKeys[l++], action);
			}
			else if (action.command.StartsWith("r"))
			{
				map.Add(rightSideKeys[r++], action);
			}
		}

		/*
		// First pass to find if there is a drop command. We want to always map it to Q or E.
		// We do it in for loop so we can remove the actions we already mapped.
		for (int i = actions.Count - 1; i > -1; i--)
		{
			DulibaWaitor.Command action = actions[i];
			if (action.name.StartsWith("drop") && action.actionType == DulibaWaitor.ActionType.PickupDrop && action.leftHand)
			{
				map.Add(leftSideKeys[l++], action);
				actions.RemoveAt(i);
			}
			else if (action.name.StartsWith("drop") && action.actionType == DulibaWaitor.ActionType.PickupDrop && !action.leftHand)
			{
				map.Add(rightSideKeys[r++], action);
				actions.RemoveAt(i);
			}
		}

		// Second pass to find if there is any other actions.
		for (int i = actions.Count - 1; i > -1; i--)
		{
			DulibaWaitor.Command action = actions[i];
			if (action.actionType == DulibaWaitor.ActionType.Action)
			{
				map.Add(rightSideKeys[r++], action);
				actions.RemoveAt(i);
			}
		}

		if (r == 0)
			r++;
		if (l == 0)
			l++;

		// Final pass for everything else.
		foreach (var action in actions)
		{
			if (action.actionType == DulibaWaitor.ActionType.PickupDrop && action.leftHand)
			{
				map.Add(leftSideKeys[l++], action);
			}
			else if (action.actionType == DulibaWaitor.ActionType.PickupDrop && !action.leftHand)
			{
				map.Add(rightSideKeys[r++], action);
			}
		}*/

		OnMappingChanged?.Invoke(map);
	}

	public enum Hand
	{
		Neither,
		Either,
		Left,
		Right,
		Both
	}

	public enum ItemChoice
	{
		None,
		All,
		Any,
		Item1,
		Item2,
		Item3,
		Item4
	}
}
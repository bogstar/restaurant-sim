using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DulibaWaitor : MonoBehaviour
{
	public DulibaHand leftHand;
	public DulibaHand rightHand;

	List<WorkArea> allAreas;

	/// <summary>
	/// Gets invoked whenever Work Area changes or becomes null. Passes the new work area (could be null) as an argument.
	/// </summary>
	public event System.Action<WorkArea> OnWorkAreaChanged;
	/// <summary>
	/// Gets invoked whenever Work Area gets removed. Passes the just-removed work area as an argument.
	/// </summary>
	public event System.Action<WorkArea> OnWorkAreaRemoved;
	/// <summary>
	/// Gets invoked whenever Commands change.
	/// </summary>
	public event System.Action<List<Command>> OnCommandsChanged;
	/// <summary>
	/// Gets invoked whenever canvas changes.
	/// </summary>
	public event System.Action OnCanvasUpdate;

	/// <summary>
	/// Current work area.
	/// </summary>
	public WorkArea workArea { get; private set; }
	Collider workAreaCollider;

	public CharacterScriptableObject characterData { get; private set; }

	public ActionMapManager actionMap = new ActionMapManager();

	public struct ActionMap
	{
		public string key;
		public float startAction;
		public float endAction;
		public Command command;

		public ActionMap(string key, float startAction, float endAction, Command command)
		{
			this.key = key;
			this.startAction = startAction;
			this.endAction = endAction;
			this.command = command;
		}
	}

	public class ActionMapManager
	{
		List<ActionMap> actionMap = new List<ActionMap>();

		public ActionMap GetByCommandString(string item)
		{
			foreach (var action in actionMap)
			{
				if (action.key == item)
					return action;
			}

			throw new System.Exception("Key not found.");
		}

		public ActionMap GetByIndex(int index)
		{
			return actionMap[index];
		}

		public bool Contains(string item)
		{
			foreach (var action in actionMap)
			{
				if (action.key == item)
					return true;
			}

			return false;
		}

		public int GetItemWithSubstring(string item, int startIndex)
		{
			foreach (var action in actionMap)
			{
				if (action.key.Substring(startIndex) == item)
					return actionMap.IndexOf(action);
			}

			return -1;
		}

		public void Add(ActionMap map)
		{
			actionMap.Add(map);
		}

		public void Remove(string item)
		{
			for (int i = actionMap.Count-1; i > -1; i--)
			{
				if (actionMap[i].key == item)
				{
					actionMap.RemoveAt(i);
					return;
				}
			}

			throw new System.Exception("Key not found.");
		}

		public void Clear()
		{
			actionMap.Clear();
		}
	}

	private void Start()
	{
		characterData = GameManager.Instance.startingCharacter;

		allAreas = new List<WorkArea>();

		leftHand.ClearItem();
		rightHand.ClearItem();
		leftHand.FinishWork();
		rightHand.FinishWork();

		OnWorkAreaChanged += delegate { _TryWork(); };
	}

	private void Update()
	{
		if (leftHand.GetItem() != null)
		{
			if (leftHand.GetItem().Spoil())
			{
				OnWorkAreaChanged?.Invoke(workArea);
			}

			if (leftHand.GetItem().Unreturn())
			{
				OnWorkAreaChanged?.Invoke(workArea);
			}
		}

		if (rightHand.GetItem() != null)
		{
			if (rightHand.GetItem().Spoil())
			{
				OnWorkAreaChanged?.Invoke(workArea);
			}

			if (rightHand.GetItem().Unreturn())
			{
				OnWorkAreaChanged?.Invoke(workArea);
			}
		}

		CalculateWorkArea();
	}

	public List<Command> actions = new List<Command>();

	public struct Command
	{
		public string name;
		public Disabled? disabled;
		public string command;
		public bool leftHand;
		public Sprite image;
		public ActionType actionType;
		public System.Action callback;
		public System.Action lateCallback;
		public float workTime;
		public int pickupIndex;

		// Experimental
		public bool reflected;
		public WorkArea.ItemDataWithCount shelf;
		public WorkArea.PreRequisite prerequisite;
		public Carryable item;
	}

	public struct Disabled
	{
		public string reason;
	}

	public enum ActionType
	{
		None,
		Action,
		PickupDrop,
		Pickup,
		Place
	}

	void _TryWork()
	{
		actions = new List<Command>();

		Carryable itemLH = leftHand.GetItem();
		Carryable itemRH = rightHand.GetItem();

		// We can't work if area is null.
		if (workArea == null)
		{
			if (itemLH != null)
			{
				if (itemLH is Package package)
				{
					actions.Add(new Command() { command = "l", actionType = ActionType.PickupDrop, name = itemLH.data.name + " package", leftHand = true, image = itemLH.data.image, disabled = new Disabled() { reason = "Nowhere to drop." } });
				}
				else
				{
					actions.Add(new Command() { command = "l", actionType = ActionType.PickupDrop, name = itemLH.data.name, leftHand = true, image = itemLH.data.image, disabled = new Disabled() { reason = "Nowhere to drop." } });
				}
			}
			if (itemRH != null)
			{
				if (itemRH is Package package)
				{
					actions.Add(new Command() { command = "r", actionType = ActionType.PickupDrop, name = itemRH.data.name + " package", image = itemRH.data.image, disabled = new Disabled() { reason = "Nowhere to drop." } });
				}
				else
				{
					actions.Add(new Command() { command = "r", actionType = ActionType.PickupDrop, name = itemRH.data.name, image = itemRH.data.image, disabled = new Disabled() { reason = "Nowhere to drop." } });
				}
			}

			OnCommandsChanged?.Invoke(actions);
			return;
		}

		List<Command> newActions = new List<Command>();

		if (itemRH != null)
		{
			actions = workArea.CanPlaceItem(itemRH);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				action.command = "r";
				action.callback += () =>
				{
					rightHand.ClearItem();
				};
				newActions.Add(action);
			}

			actions = workArea.CanInteractWithItem(itemRH);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				action.command = "r";
				action.callback += () =>
				{
					rightHand.SetItem(action.item);
				};
				newActions.Add(action);
			}
		}

		if (itemLH != null)
		{
			actions = workArea.CanPlaceItem(itemLH);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				action.command = "l";
				action.leftHand = true;
				action.callback += () =>
				{
					leftHand.ClearItem();
				};
				newActions.Add(action);
			}

			actions = workArea.CanInteractWithItem(itemLH);

			for (int i = 0; i < actions.Count; i++)
			{
				var action = actions[i];
				action.command = "l";
				action.callback += () =>
				{
					leftHand.SetItem(action.item);
				};
				newActions.Add(action);
			}
		}

		actions = workArea.GetCommands(this);

		for (int i = 0; i < actions.Count; i++)
		{
			var action = actions[i];

			if (action.actionType == ActionType.Pickup)
			{
				if (action.reflected)
				{
					if (itemLH == null)
					{
						newActions.Add(action);
						var a = newActions[newActions.Count - 1];
						a.command = "l";
						a.leftHand = true;
						if (action.shelf.item == null)
						{
							a.callback += () =>
							{
								leftHand.SetItem(((Item)action.item));
							};
						}
						else
						{
							a.callback += () =>
							{
								leftHand.SetItem(action.shelf.item);
							};
						}
						newActions[newActions.Count - 1] = a;
					}

					if (itemRH == null)
					{
						newActions.Add(action);
						var a = newActions[newActions.Count - 1];
						a.command = "r";
						if (action.shelf.item == null)
						{
							a.callback += () =>
							{
								rightHand.SetItem(action.item);
							};
						}
						else
						{
							a.callback += () =>
							{
								rightHand.SetItem(action.shelf.item);
							};
						}
						newActions[newActions.Count - 1] = a;
					}
				}
			}
			else if (action.actionType == ActionType.Action)
			{
				if (action.prerequisite == WorkArea.PreRequisite.BothHandsEmpty && GetEmptyHand() == DulibaInput.Hand.Both || action.prerequisite == WorkArea.PreRequisite.None)
				{
					newActions.Add(action);
					var a = newActions[newActions.Count - 1];
					a.command = "r";
					newActions[newActions.Count - 1] = a;
				}
			}
		}

		OnCommandsChanged?.Invoke(newActions);
	}

	public void SetWorkArea(WorkArea newWorkArea, Collider collider)
	{
		if (!allAreas.Contains(newWorkArea))
		{
			workAreaCollider = collider;
			allAreas.Add(newWorkArea);
			newWorkArea.OnWorkAreaUpdate += OnWorkAreaChanged;
			newWorkArea.OnCanvasUpdate += OnCanvasUpdate;
		}
	}

	public void RemoveWorkArea(WorkArea newWorkArea)
	{
		if (allAreas.Contains(newWorkArea))
		{
			newWorkArea.OnWorkAreaUpdate -= OnWorkAreaChanged;
			newWorkArea.OnCanvasUpdate -= OnCanvasUpdate;
			allAreas.Remove(newWorkArea);
		}
	}

	void CalculateWorkArea()
	{
		WorkArea calculatedWorkArea = null;

		#region Complicated distance calculation. If successful, returns new work area.
		Dictionary<WorkArea, float> areaDstMap = new Dictionary<WorkArea, float>();
		foreach (var area in allAreas)
		{
			areaDstMap.Add(area, (transform.position - area.transform.position).sqrMagnitude);
		}
		WorkArea[] copy = allAreas.ToArray();
		System.Array.Sort(copy, (x, y) => areaDstMap[x].CompareTo(areaDstMap[y]));

		// Get closest work area. Regardless if it's interactable or not.
		if (copy.Length > 0)
		{
			calculatedWorkArea = copy[0];
		}
		#endregion

		if (workArea != calculatedWorkArea)
		{
			if (workArea != null)
			{
				OnWorkAreaRemoved?.Invoke(workArea);
			}

			workArea = calculatedWorkArea;
			OnWorkAreaChanged?.Invoke(workArea);
		}
	}

	#region Helper Methods
	/// <summary>
	/// Returns hands that are not empty.
	/// </summary>
	/// <returns></returns>
	public DulibaInput.Hand GetFullHand()
	{
		if (leftHand.GetItem() == null && rightHand.GetItem() == null)
			return DulibaInput.Hand.Neither;
		if (leftHand.GetItem() == null)
			return DulibaInput.Hand.Right;
		if (rightHand.GetItem() == null)
			return DulibaInput.Hand.Left;

		return DulibaInput.Hand.Both;
	}

	/// <summary>
	/// Returns hands that are empty.
	/// </summary>
	/// <returns></returns>
	public DulibaInput.Hand GetEmptyHand()
	{
		if (leftHand.GetItem() != null && rightHand.GetItem() != null )
			return DulibaInput.Hand.Neither;
		if (leftHand.GetItem() != null)
			return DulibaInput.Hand.Right;
		if (rightHand.GetItem() != null)
			return DulibaInput.Hand.Left;

		return DulibaInput.Hand.Both;
	}

	/// <summary>
	/// Always returns an array of length 2. Index 1 is content of left, and 2 of right hand. If items are absent, returns null at indices.
	/// </summary>
	/// <param name="hand"></param>
	/// <returns></returns>
	public Carryable[] GetItemsInHand()
	{
		return new Carryable[] { leftHand.GetItem(), rightHand.GetItem() };
	}
	#endregion
}
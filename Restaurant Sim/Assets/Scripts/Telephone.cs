using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telephone : WorkArea
{
	[SerializeField] PurchaseScreen screen;

	protected override void Start()
	{
		base.Start();
	}

	public override List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor)
	{
		List<DulibaWaitor.Command> actions = new List<DulibaWaitor.Command>();

		actions.Add(new DulibaWaitor.Command()
		{
			workTime = 0.25f,
			actionType = DulibaWaitor.ActionType.Action,
			prerequisite = PreRequisite.BothHandsEmpty,
			name = "schedule delivery ",
			callback = () =>
			{
				screen.Display(true);
			},
			lateCallback = () =>
			{
				OnWorkAreaUpdate?.Invoke(this);
			}
		});

		return actions;
	}

	protected override void OnWaitorLeave()
	{
		base.OnWaitorLeave();

		screen.Display(false);
	}

	public override WorkCanvasInfo GetCanvasData()
	{
		return new WorkCanvasInfo() { title = name };
	}

	public override List<DulibaWaitor.Command> CanPlaceItem(Carryable item)
	{
		return new List<DulibaWaitor.Command>();
	}
}
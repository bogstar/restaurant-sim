using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class WorkArea : MonoBehaviour
{
	public new string name;
	public Renderer[] renderers;

	protected List<DulibaWaitor> waitors;

	public System.Action<WorkArea> OnWorkAreaUpdate;
	public System.Action OnCanvasUpdate;

	Color[] colors;

	bool highlighted;

	protected virtual void Start()
	{
		waitors = new List<DulibaWaitor>();
		colors = new Color[renderers.Length];
	}

	public virtual List<DulibaWaitor.Command> GetCommands(DulibaWaitor waitor) { return new List<DulibaWaitor.Command>(); }
	public virtual List<DulibaWaitor.Command> CanPlaceItem(Carryable item) { return new List<DulibaWaitor.Command>(); }
	public virtual List<DulibaWaitor.Command> CanInteractWithItem(Carryable item) { return new List<DulibaWaitor.Command>(); }
	public virtual WorkCanvasInfo GetCanvasData() { return new WorkCanvasInfo() { title = name }; }

	protected virtual void OnWaitorLeave()
	{

	}

	public struct WorkCanvasInfo
	{
		public string title;
		public string text;
	}

	public void Highlight(bool highlight)
	{
		if (highlight && !highlighted)
		{
			colors = new Color[renderers.Length];

			for (int i = 0; i < renderers.Length; i++)
			{
				colors[i] = renderers[i].material.color;
				renderers[i].material.color = Color.yellow;
			}

			highlighted = true;
		}
		else if(!highlight && highlighted)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].material.color = colors[i];
			}

			highlighted = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		DulibaWaitor newWaitor = other.GetComponent<DulibaWaitor>();
		if (newWaitor != null)
		{
			if (!waitors.Contains(newWaitor))
			{
				waitors.Add(newWaitor);
				newWaitor.SetWorkArea(this, GetComponents<Collider>().First((a) => { return a.isTrigger; }));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		DulibaWaitor newWaitor = other.GetComponent<DulibaWaitor>();
		if (newWaitor != null)
		{
			if (waitors.Contains(newWaitor))
			{
				waitors.Remove(newWaitor);
				newWaitor.RemoveWorkArea(this);
				OnWaitorLeave();
			}
		}
	}

	public enum PreRequisite
	{
		None,
		BothHandsEmpty
	}

	public struct ItemDataWithCount
	{
		public ItemScriptableObject item;
		public int index;
		public int count;
		public bool infinite;
		public bool oneItem;

		public ItemDataWithCount(ItemScriptableObject item, int index, int count, bool infinite, bool oneItem)
		{
			this.item = item;
			this.index = index;
			this.count = count;
			this.infinite = infinite;
			this.oneItem = oneItem;
		}
	}

	public struct ItemWithIndex
	{
		public Carryable item;
		public int index;

		public ItemWithIndex(Carryable item, int index)
		{
			this.item = item;
			this.index = index;
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "MammaMia/Item")]
public class ItemScriptableObject : DatabaseEntryScriptableObject
{
	public new string name;
	[SerializeField]
	Sprite m_image;
	public Sprite image { get { return m_image == null ? DatabaseManager.Instance.defaultSprite : m_image; } }
	[SerializeField]
	ModelSetup m_model;
	public ModelSetup model { get { return m_model ?? DatabaseManager.Instance.defaultModel; } set => m_model = value; }
	[TextArea]
	public string description;
	public bool rottable;
	public float rotTime;
	public float rotTimeReturnable;
	public int garbageValue;
	public bool needsFridge;
	public bool cannotShelf;
	public MixOptions[] mixOptions;

	[System.Serializable]
	public class MixOptions
	{
		public ItemScriptableObject otherItem;
		public ItemScriptableObject resultItem;
		public float mixTime;
	}

	public MixOptions GetMixingResult(ItemScriptableObject other)
	{
		foreach (var mixOption in mixOptions)
		{
			if (other.id == mixOption.otherItem.id)
			{
				return mixOption;
			}
		}

		return null;
	}

	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "item_" + newString;
	}
}
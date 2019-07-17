using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Carryable
{
	public float spoilageTime;
	public float returnableTime;

	bool unreturned;

	public Item(ItemScriptableObject unsafeCopy) : base(unsafeCopy)
	{
		if (data.rottable)
		{
			spoilageTime = Time.time + data.rotTime;
			returnableTime = Time.time + data.rotTimeReturnable;
		}
	}

	public override ItemScriptableObject.MixOptions GetMixingResult(Carryable other)
	{
		var dataMixResult = data.GetMixingResult(other.data);
		if (dataMixResult == null)
		{
			return null;
		}

		return dataMixResult;
	}

	public override bool Spoil()
	{
		if (data.rottable && Time.time > spoilageTime)
		{
			int garbage = data.garbageValue;
			ItemScriptableObject item = (ItemScriptableObject)DatabaseManager.GetItem("rot");

			data = item;
			data.garbageValue = garbage;

			return true;
		}

		return false;
	}

	public override bool Unreturn()
	{
		if (!unreturned && data.rottable && Time.time > returnableTime)
		{
			unreturned = true;
			return true;
		}

		return false;
	}

	public override void SetSpoilageTime(float spoilageTime)
	{
		if (data.rottable)
		{
			this.spoilageTime = spoilageTime;
		}
	}

	public override bool IsReturnable
	{
		get => !(data.rottable && Time.time > returnableTime);
	}
}
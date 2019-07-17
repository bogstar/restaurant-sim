using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable
{
	public ItemScriptableObject data;


	public Carryable (ItemScriptableObject unsafeCopy)
	{
		ItemScriptableObject item = (ItemScriptableObject)DatabaseManager.GetItem(unsafeCopy.id);

		data = item;
	}

	/// <summary>
	/// Returns the mix results of the carryable and the other carryable. Returns null if none.
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public virtual ItemScriptableObject.MixOptions GetMixingResult(Carryable other) { return null; }

	/// <summary>
	/// Tries to spoil the carryable. Returns true if the carryable is spoiled. Returns false if carryable can't be spoiled or time hasn't passed yet.
	/// </summary>
	/// <returns></returns>
	public virtual bool Spoil() { return false; }

	/// <summary>
	/// Tries to unreturn the carryable. Returns true if the carryable is unreturned. Returns false if carryable can't be spoiled or time hasn't passed yet.
	/// </summary>
	/// <returns></returns>
	public virtual bool Unreturn() { return false; }

	public virtual bool IsReturnable { get => true; }
	public virtual void SetSpoilageTime(float spoilageTime) { }
}
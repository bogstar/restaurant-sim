using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DatabaseEntryScriptableObject : ScriptableObject
{
	string m_id;
	public string id { get => m_id; }

	public void SetId(string id)
	{
		m_id = id;
	}

	public abstract string GetDefiningFeatures();
}
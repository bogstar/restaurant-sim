using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "MammaMia/Character")]
public class CharacterScriptableObject : DatabaseEntryScriptableObject
{
	public new string name;
	public float movementSpeed;
	public float workTimeMultiplier;

	public override string GetDefiningFeatures()
	{
		var newString = name.Replace(" ", "_");
		newString = newString.ToLower();

		return "character_" + newString;
	}
}
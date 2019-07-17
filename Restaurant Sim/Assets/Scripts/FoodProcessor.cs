using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FoodProcessor : WorkArea
{
	public FoodProcessorScriptableObject foodProcessor;


	protected override void Start()
	{
		base.Start();

		foodProcessor = DatabaseManager.GetItem(foodProcessor.id) as FoodProcessorScriptableObject;
	}
}
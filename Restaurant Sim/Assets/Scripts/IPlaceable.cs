using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaceable
{
	bool CanPlaceDown(DulibaWaitor waitor, DulibaInput.Hand hand);
	void PlaceDown(DulibaWaitor waitor, DulibaInput.Hand hand);
}
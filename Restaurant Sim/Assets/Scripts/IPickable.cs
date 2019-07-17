using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickable
{
	bool CanPickUp(DulibaWaitor waitor, DulibaInput.Hand hand, DulibaInput.ItemChoice itemChoice);
	void PickUp(DulibaWaitor waitor, DulibaInput.Hand hand, int index);
}
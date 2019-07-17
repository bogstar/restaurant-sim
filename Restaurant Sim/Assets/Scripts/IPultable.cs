using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPultable
{
	WorkArea.ItemWithIndex?[] GetAvailableItems();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShelfable
{
	WorkArea.ItemDataWithCount[] GetAvailableItems();
}

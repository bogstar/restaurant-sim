using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseScreen : MonoBehaviour
{
	[SerializeField] ScrollRect scrollRect;
	[SerializeField] GameObject purchasePrefab;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(() => { Display(false); });
	}

	public void Display(bool display)
	{
		gameObject.SetActive(display);
		Refresh();
	}

	void Refresh()
	{
		for (int i = scrollRect.content.childCount-1; i > -1; i--)
		{
			Destroy(scrollRect.content.transform.GetChild(i).gameObject);
		}

		foreach (var item in GameManager.Instance.deliverableItems)
		{
			PurchaseScreenButton button = Instantiate(purchasePrefab, scrollRect.content).GetComponent<PurchaseScreenButton>();
			button.SetButton(item, "$20", "12");
			button.SetOnclickEvent(GameManager.Instance.DeliverItem);
		}
	}
}
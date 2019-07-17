using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0649

public class SingleButton : MonoBehaviour
{
	[SerializeField]
	public Image image;
	[SerializeField]
	Text nameLabel;
	[SerializeField]
	Text buttonPressLabel;
	[SerializeField]
	Text quantityLabel;
	[SerializeField]
	float smooth;

	Vector3 targetPosition;
	bool death;
	Vector3 velocity;

	public DulibaHUD.ButtonInfo buttonInfo;

	private void Update()
	{
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, smooth);
		if (death && Vector3.Distance(targetPosition, transform.localPosition) < 0.5f)
		{
			Destroy(gameObject);
		}
	}

	public void SetQuantity(int quantity)
	{
		SetQuantity(quantity.ToString());
	}

	public void SetQuantity(string quantity)
	{
		if (quantity == "∞")
		{
			quantityLabel.fontStyle = FontStyle.Bold;
			quantityLabel.fontSize = 39;
			quantityLabel.alignment = TextAnchor.LowerCenter;
		}
		else
		{
			quantityLabel.fontStyle = FontStyle.Normal;
			quantityLabel.fontSize = 22;
			quantityLabel.alignment = TextAnchor.MiddleLeft;
		}

		quantityLabel.text = quantity;
	}

	public void SetName(string name)
	{
		nameLabel.text = name;
	}

	public void SetButtonPress(string buttonPress)
	{
#if UNITY_ANDROID
		buttonPressLabel.text = "";
#else
		buttonPressLabel.text = buttonPress;
#endif
	}

	public void SetImage(Sprite image)
	{
		this.image.sprite = image;
	}

	public void SetText(string name, string quantity, string buttonPress, Sprite image)
	{
		SetName(name);
		SetQuantity(quantity);
		SetButtonPress(buttonPress);
		SetImage(image);
	}

	public void SetTargetPosition(Vector3 targetPos)
	{
		targetPosition = targetPos;
	}

	public void Die()
	{
		death = true;
	}
}
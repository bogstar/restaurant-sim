using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Vector2 joystickCenter = Vector2.zero;
	Vector2 referenceRes;

	bool working;

    void Start()
    {
        background.gameObject.SetActive(false);
		referenceRes = FindObjectOfType<UnityEngine.UI.CanvasScaler>().referenceResolution;
    }

    public override void OnDrag(PointerEventData eventData)
    {
		if (working == false)
		{
			return;
		}

        Vector2 direction = eventData.position - joystickCenter;
		float ratio = Screen.width / referenceRes.x;
		inputVector = (direction.magnitude > (background.sizeDelta.x / 2f) * ratio) ? direction.normalized : direction / ((background.sizeDelta.x / 2f) * ratio);
        ClampJoystick();
        handle.anchoredPosition = (inputVector * (background.sizeDelta.x / 2f)) * handleLimit;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
		if (EventSystem.current.IsPointerOverGameObject())
		{
			EventSystem eventSystem = EventSystem.current;

			PointerEventData pointer = new PointerEventData(eventSystem);
			pointer.position = Input.mousePosition;
			List<RaycastResult> raycastResults = new List<RaycastResult>();

			eventSystem.RaycastAll(pointer, raycastResults);

			foreach (var result in raycastResults)
			{
				if (result.gameObject.name != "Floating Joystick")
					return;
			}
		}

		working = true;
        background.gameObject.SetActive(true);
        background.position = eventData.position;
        handle.anchoredPosition = Vector2.zero;
        joystickCenter = eventData.position;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
		working = false;
        background.gameObject.SetActive(false);
        inputVector = Vector2.zero;
    }
}
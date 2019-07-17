using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DulibaInputKeys
{
	public KeyCode leftHandPickup;
	public KeyCode rightHandPickup;
	public KeyCode pickUpItemL1;
	public KeyCode pickUpItemL2;
	public KeyCode pickUpItemL3;
	public KeyCode pickUpItemL4;
	public KeyCode pickUpItemR1;
	public KeyCode pickUpItemR2;
	public KeyCode pickUpItemR3;
	public KeyCode pickUpItemR4;

	Dictionary<string, KeyPress> keyPresses = new Dictionary<string, KeyPress>();

	public Vector2 calculatedVector;

	event System.Action<ButtonMap> OnKeyAction;

	public Joystick joystick;

	public void HandleMovementInput()
	{
		Vector2 inputVector;

#if UNITY_ANDROID
		inputVector = new Vector2(joystick.Horizontal, joystick.Vertical);

		if (inputVector.sqrMagnitude < 0.035f)
			inputVector = Vector2.zero;
#else
		inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		if (inputVector.sqrMagnitude > 1f)
			inputVector.Normalize();
#endif

		calculatedVector = inputVector;
	}

#if UNITY_ANDROID
	public void ButtonPressedDown(string button)
	{
		if (!keyPresses.ContainsKey(button))
			keyPresses.Add(button, KeyPress.Down);
	}

	public void ButtonPressedUp(string button)
	{
		if (keyPresses.ContainsKey(button))
			keyPresses[button] = KeyPress.Up;
	}
#endif

	public class ButtonMap
	{
		public List<string> downButtons;
		public List<string> pressedButtons;
		public List<string> upButtons;

		public ButtonMap()
		{
			downButtons = new List<string>();
			pressedButtons = new List<string>();
			upButtons = new List<string>();
		}
	}

	public ButtonMap buttonMap;

	public void HandleWaitoringInput()
	{
		buttonMap = new ButtonMap();

#if UNITY_ANDROID
		foreach (var kvp in keyPresses)
		{
			string button = kvp.Key;
			KeyPress keyPress = kvp.Value;

			switch (keyPress)
			{
				case KeyPress.Down:
					buttonMap.downButtons.Add(button.ToUpper());
					break;
				case KeyPress.Pressed:
					buttonMap.pressedButtons.Add(button.ToUpper());
					break;
				case KeyPress.Up:
					buttonMap.upButtons.Add(button.ToUpper());
					break;
			}

			//OnKeyAction?.Invoke(button.ToUpper(), keyPress);
		}
#else

		if (Input.GetKeyDown(pickUpItemL1))
		{
			buttonMap.downButtons.Add("L1");
			//OnKeyAction?.Invoke("L1", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemL1))
		{
			buttonMap.pressedButtons.Add("L1");
			//OnKeyAction?.Invoke("L1", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemL1))
		{
			buttonMap.upButtons.Add("L1");
			//OnKeyAction?.Invoke("L1", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemL2))
		{
			buttonMap.downButtons.Add("L2");
			//OnKeyAction?.Invoke("L2", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemL2))
		{
			buttonMap.pressedButtons.Add("L2");
			//OnKeyAction?.Invoke("L2", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemL2))
		{
			buttonMap.upButtons.Add("L2");
			//OnKeyAction?.Invoke("L2", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemL3))
		{
			buttonMap.downButtons.Add("L3");
			//OnKeyAction?.Invoke("L3", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemL3))
		{
			buttonMap.pressedButtons.Add("L3");
			//OnKeyAction?.Invoke("L3", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemL3))
		{
			buttonMap.upButtons.Add("L3");
			//OnKeyAction?.Invoke("L3", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemL4))
		{
			buttonMap.downButtons.Add("L4");
			//OnKeyAction?.Invoke("L4", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemL4))
		{
			buttonMap.pressedButtons.Add("L4");
			//OnKeyAction?.Invoke("L4", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemL4))
		{
			buttonMap.upButtons.Add("L4");
			//OnKeyAction?.Invoke("L4", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemR1))
		{
			buttonMap.downButtons.Add("R1");
			//OnKeyAction?.Invoke("R1", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemR1))
		{
			buttonMap.pressedButtons.Add("R1");
			//OnKeyAction?.Invoke("R1", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemR1))
		{
			buttonMap.upButtons.Add("R1");
			//OnKeyAction?.Invoke("R1", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemR2))
		{
			buttonMap.downButtons.Add("R2");
			//OnKeyAction?.Invoke("R2", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemR2))
		{
			buttonMap.pressedButtons.Add("R2");
			//OnKeyAction?.Invoke("R2", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemR2))
		{
			buttonMap.upButtons.Add("R2");
			//OnKeyAction?.Invoke("R2", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemR3))
		{
			buttonMap.downButtons.Add("R3");
			//OnKeyAction?.Invoke("R3", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemR3))
		{
			buttonMap.pressedButtons.Add("R3");
			//OnKeyAction?.Invoke("R3", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemR3))
		{
			buttonMap.upButtons.Add("R3");
			//OnKeyAction?.Invoke("R3", KeyPress.Up);
		}

		if (Input.GetKeyDown(pickUpItemR4))
		{
			buttonMap.downButtons.Add("R4");
			//OnKeyAction?.Invoke("R4", KeyPress.Down);
		}
		else if (Input.GetKey(pickUpItemR4))
		{
			buttonMap.pressedButtons.Add("R4");
			//OnKeyAction?.Invoke("R4", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(pickUpItemR4))
		{
			buttonMap.upButtons.Add("R4");
			//OnKeyAction?.Invoke("R4", KeyPress.Up);
		}

		if (Input.GetKeyDown(leftHandPickup))
		{
			buttonMap.downButtons.Add("Q");
			//OnKeyAction?.Invoke("Q", KeyPress.Down);
		}
		else if (Input.GetKey(leftHandPickup))
		{
			buttonMap.pressedButtons.Add("Q");
			//OnKeyAction?.Invoke("Q", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(leftHandPickup))
		{
			buttonMap.upButtons.Add("Q");
			//OnKeyAction?.Invoke("Q", KeyPress.Up);
		}

		if (Input.GetKeyDown(rightHandPickup))
		{
			buttonMap.downButtons.Add("E");
			//OnKeyAction?.Invoke("E", KeyPress.Down);
		}
		else if (Input.GetKey(rightHandPickup))
		{
			buttonMap.pressedButtons.Add("E");
			//OnKeyAction?.Invoke("E", KeyPress.Pressed);
		}
		else if (Input.GetKeyUp(rightHandPickup))
		{
			buttonMap.upButtons.Add("E");
			//OnKeyAction?.Invoke("E", KeyPress.Up);
		}
#endif

		OnKeyAction?.Invoke(buttonMap);

		List<string> presses = new List<string>();
		List<string> nones = new List<string>();

		foreach (var kvp in keyPresses)
		{
			string button = kvp.Key;
			KeyPress keyPress = kvp.Value;

			switch (keyPress)
			{
				case KeyPress.Down:
					presses.Add(button);
					break;
				case KeyPress.Up:
					nones.Add(button);
					break;
			}
		}

		foreach (var p in presses)
		{
			keyPresses[p] = KeyPress.Pressed;
		}

		foreach (var p in nones)
		{
			keyPresses.Remove(p);
		}
	}

	public enum KeyPress
	{
		None, Down, Pressed, Up
	}

	#region Callback Registering
	/// <summary>
	/// Register a callback that gets called when a button gets interacted with.
	/// </summary>
	/// <param name="callback"></param>
	public void RegisterKeyActionCallback(System.Action<ButtonMap> callback)
	{
		OnKeyAction += callback;
	}

	/// <summary>
	/// Unregister a callback that gets called when a button gets interacted with.
	/// </summary>
	/// <param name="callback"></param>
	public void UnregisterKeyActionCallback(System.Action<ButtonMap> callback)
	{
		OnKeyAction -= callback;
	}
	#endregion
}

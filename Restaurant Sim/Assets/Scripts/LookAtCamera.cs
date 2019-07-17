using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	new Camera camera;

	private void Start()
	{
		camera = Camera.main;
	}

	private void LateUpdate()
	{
		transform.LookAt(Camera.main.transform);
	}
}

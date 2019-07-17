using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float smoothTime;

	Vector3 velocity;

	private void Update()
	{
		transform.position = Vector3.SmoothDamp(transform.position, target.position + new Vector3(0, 4.43f, -3.75f), ref velocity, smoothTime);
	}
}
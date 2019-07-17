using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DulibaWaitor))]
public class DulibaMove : MonoBehaviour
{
	CharacterController cc;
	DulibaWaitor waitor;

	Vector2 inputVector;

	private void Start()
	{
		waitor = GetComponent<DulibaWaitor>();
		cc = GetComponent<CharacterController>();
	}

	private void Update()
	{
		Vector3 moveVec = new Vector3(inputVector.x, 0f, inputVector.y) * waitor.characterData.movementSpeed * Time.deltaTime;

		if (moveVec.sqrMagnitude > 0)
		{
			transform.LookAt(transform.position + moveVec, Vector3.up);
		}

		cc.Move(moveVec);
	}

	public void SetInput(Vector2 input)
	{
		inputVector = input;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerROam : MonoBehaviour
{
	public UnityEngine.AI.NavMeshAgent agent;
	public Transform target;

	private void Update()
	{
		agent.SetDestination(target.position);
	}
}

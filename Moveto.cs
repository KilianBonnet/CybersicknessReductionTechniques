using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveto : MonoBehaviour {
	public Transform goal;

	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		//agent.destination = goal.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.destination = goal.position;
	}
}

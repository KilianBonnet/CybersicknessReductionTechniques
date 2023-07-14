using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestTrail : MonoBehaviour {
    [SerializeField] private Transform destination;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        agent.destination = destination.position;
    }
}

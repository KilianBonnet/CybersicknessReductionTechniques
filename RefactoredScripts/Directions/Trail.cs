using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Trail : MonoBehaviour {
    [Tooltip("Time between 2 trail updates")]
    [SerializeField] [Range(.1f, 10)] private float updateTime = 3;
    private float timer;

    private Transform playerTransform;
    private NavMeshAgent agent;
    private ParticleSystem particles;
    
    private Vector3 trailHead;
    private Vector3 trailTail;

    private void Start() {
        playerTransform = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        particles = GetComponent<ParticleSystem>();
        
        agent.isStopped = true;
    }

    public void DrawTrail(Vector3 destination) {
        trailHead = destination;
        UpdateTrail();
    }

    public void StopTrail() {
        agent.isStopped = true;
        particles.Clear();
        particles.Stop();
    }
    
    private void Update() {
        if (agent.isStopped) return;
        if((timer += Time.deltaTime) < updateTime) return;
        UpdateTrail();
    }

    private void UpdateTrail() {
        timer = 0;
        
        particles.Stop();
        agent.enabled = false;
        trailTail = playerTransform.position;
        transform.position = trailTail;

        particles.Play();
        agent.enabled = true;
        agent.isStopped = false;
        agent.destination = trailHead;
    }
}

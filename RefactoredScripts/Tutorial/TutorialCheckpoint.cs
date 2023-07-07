using System;
using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{
    public static event Action CheckpointReachEvent;
    
    private void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player"))
            return;
        CheckpointReachEvent?.Invoke();
    }
}

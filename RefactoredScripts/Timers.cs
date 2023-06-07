using UnityEngine;

public class Timers : MonoBehaviour {
    // Checkpoint behaviour
    [HideInInspector]
    public bool checkpointTimeActive;
    public float time { get; private set; }
    private float lastTime;
    
    // Return to departure behaviour
    [HideInInspector]
    public bool timeOfReturnActive;
    public float timeOfReturn { get; private set; }

    public void ResetTimers() {
        checkpointTimeActive = false;
        time = 0;
        lastTime = 0;

        timeOfReturnActive = false;
        timeOfReturn = 0;
    }
    
    private void Update() {
        if(checkpointTimeActive) time += Time.deltaTime;
        if(timeOfReturnActive) timeOfReturn += Time.deltaTime;
    }

    /// <summary>
    /// Return the time elapsed since the last time this method was called.
    /// </summary>
    public float GetElapsedTime() {
        float duration = time - lastTime;
        lastTime = time;
        return duration;
    }
    
}

using System;
using UnityEngine;

public class TutorialLevel : MonoBehaviour {
    /// <summary>
    /// Event invoked when the level is terminated.
    /// </summary>
    public static event Action TutorialEndEvent;
    
    protected OVRPlayerController playerController;

    /// <summary>
    /// Set up elements that are common to every level should go here
    /// </summary>
    protected void SetUp() {
        playerController = FindObjectOfType<OVRPlayerController>();
    }

    /// <summary>
    ///  Function called by inherited class to Invoke TutorialEndEvent event.
    /// </summary>
    protected void InvokeEndEvent() {
        TutorialEndEvent?.Invoke();
    }
}

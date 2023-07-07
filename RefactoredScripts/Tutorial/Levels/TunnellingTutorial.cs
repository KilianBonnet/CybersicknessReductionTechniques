using System;
using UnityEngine;

public class TunnellingTutorial : TutorialLevel {
    [SerializeField]
    private GameObject ui;
    
    private void Awake() {
        SetUp();
        ui.SetActive(true);
    }

    private void Update() {
        // Waiting for Y button to be pressed
        if (!OVRInput.GetDown(OVRInput.Button.Four))
            return;
        ui.SetActive(false);
        InvokeEndEvent();
    }
}

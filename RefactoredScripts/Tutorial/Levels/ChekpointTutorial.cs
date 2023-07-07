using System;
using TMPro;
using UnityEngine;

public class ChekpointTutorial : TutorialLevel
{
    [SerializeField]
    private GameObject ui;

    [SerializeField]
    private TextMeshProUGUI uiContent;

    [SerializeField]
    private GameObject checkpoint;

    private string defaultText;

    private void OnEnable() {
        SetUp();
        
        ui.SetActive(true);

        playerController.CanMove = false;
        playerController.EnableRotation = false;
        
        defaultText = uiContent.text;
        uiContent.text = $"{defaultText}\nPress the \"Y\" button to begin.";
        
        TutorialCheckpoint.CheckpointReachEvent += OnCheckpointReached;
    }

    private void Update() {
        if (!OVRInput.GetDown(OVRInput.Button.Four))
            return;
        uiContent.text = $"{defaultText}\nTry to reach the checkpoint in front of you!";
        checkpoint.SetActive(true);
        
        playerController.CanMove = true;
        playerController.EnableRotation = true;
    }

    private void OnCheckpointReached() {
        checkpoint.SetActive(false);
        uiContent.text = defaultText;
        ui.SetActive(false);
        InvokeEndEvent();
    }
}
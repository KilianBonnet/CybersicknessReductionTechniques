using UnityEngine;

public class YButtonTutorial : TutorialLevel {
    [SerializeField]
    private GameObject ui;
    
    private void OnEnable() {
        SetUp();
        ui.SetActive(true);

        playerController.EnableRotation = false;
        playerController.CanMove = false;
    }

    private void Update() {
        // Waiting for Y button to be pressed
        if (!OVRInput.GetDown(OVRInput.Button.Four))
            return;
        ui.SetActive(false);
        InvokeEndEvent();
    }
}

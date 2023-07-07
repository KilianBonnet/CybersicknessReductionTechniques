using TMPro;
using UnityEngine;

public class RotateTutorial : TutorialLevel
{
    [SerializeField]
    private GameObject ui;

    [SerializeField]
    private TextMeshProUGUI uiContent;

    [SerializeField] 
    private float rotationTimeNeeded;

    private string defaultText;
    
    private float timeRotating;
    
    private void OnEnable() {
        SetUp();
        ui.SetActive(true);
        
        playerController.EnableRotation = true;
        playerController.CanMove = false;

        timeRotating = 0;
        
        defaultText = uiContent.text;
        uiContent.text = $"{defaultText}\n0%";
    }

    
    void Update() {
        // Waiting for rotation input
        if (!(OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
            OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight)))
            return;
        
        timeRotating += Time.deltaTime;
        uiContent.text = $"{defaultText}\n{(int)(timeRotating * 100 / rotationTimeNeeded)}%";

        if (timeRotating > rotationTimeNeeded) {
            uiContent.text = defaultText;
            ui.SetActive(false);
            InvokeEndEvent();
        }
    }
}

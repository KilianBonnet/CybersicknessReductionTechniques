using TMPro;
using UnityEngine;

public class MovingTutorial : TutorialLevel
{
    [SerializeField]
    private GameObject ui;
    
    [SerializeField]
    private TextMeshProUGUI uiContent;
    
    [SerializeField] 
    private float rotationTimeNeeded;

    private string defaultText;
    
    private float timeMoving;
    
    private void OnEnable() {
        SetUp();
        ui.SetActive(true);
        
        playerController.EnableRotation = true;
        playerController.CanMove = true;

        timeMoving = 0;
            
        defaultText = uiContent.text;
        uiContent.text = $"{defaultText}\n0%";
    }
    
    void Update() {
        // Waiting for rotation input
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).magnitude < .1f)
            return;
        
        timeMoving += Time.deltaTime;
        uiContent.text = $"{defaultText}\n{(int)(timeMoving * 100 / rotationTimeNeeded)}%";

        if (timeMoving > rotationTimeNeeded) {
            uiContent.text = defaultText;
            ui.SetActive(false);
            InvokeEndEvent();
        }
    }
}

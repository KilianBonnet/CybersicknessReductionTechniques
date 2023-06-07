using System;
using UnityEngine.UI;
using UnityEngine;

public class MenuInputLoader : MonoBehaviour
{
    [SerializeField] private InputField participantNumberInput;
    [SerializeField] private Dropdown groupsDropdown;
    [SerializeField] private Dropdown daysDropdown;

    private Button startButton;

    private void Start () {
        startButton = GetComponent<Button>();
        startButton.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Method called when the start button ui is clicked
    /// </summary>
    private void OnClick() {
        string userId = participantNumberInput.text.Trim();
        
        if (userId.Length != 19)
        {
            Debug.LogError("Invalid UserID key. It should have exactly 19 digits.");
            return;
        }
        
        try
        {
            /*
             * UserID Key
             * First 2 Digits = Number of degrees of rotation per snap
             * Second 3 Digits = Time of rotation, being 0.00 seconds
             * Third 3 Digits = Distance being 0.00 meters
             * Fourth 3 Digits = Time of translation being 0.00 seconds
             * Fifth 3 Digits = Maximum screen covered by tunneling being 0.00 (between 0 and 1)
             * Sixth 3 Digits = Time taken to tunnel being 0.00
             * Last 2 Digits = unique identifier
             * 19 digits total
             */
            int snappingRotation = int.Parse(userId.Substring(0, 2));
            float snappingRotationSpeed = int.Parse(userId.Substring(2, 3)) / 100f;
            
            float snappingTranslationDistance = float.Parse(userId.Substring(5, 3)) / 100f;
            float snappingTranslationSpeed = float.Parse(userId.Substring(8, 3)) / 100f;
            
            float tunnelingAmount = float.Parse(userId.Substring(11, 3)) / 100f;
            float tunnelingSpeed = float.Parse(userId.Substring(14, 3)) / 100f;
            
            int uniqueIdentifier = int.Parse(userId.Substring(17, 2));
            

            PlayerPrefs.SetString("ParticipantNumber", uniqueIdentifier.ToString());
            PlayerPrefs.SetInt("GroupNumber", groupsDropdown.value + 1);
            PlayerPrefs.SetInt("DayNumber", daysDropdown.value + 1);
            
            PlayerPrefs.SetFloat("SnappingRotation", snappingRotation);
            PlayerPrefs.SetFloat("SnappingRotationSpeed", snappingRotationSpeed);
            
            PlayerPrefs.SetFloat("SnappingTranslationDistance", snappingTranslationDistance);
            PlayerPrefs.SetFloat("SnappingTranslationDistance", snappingTranslationSpeed);
            
            PlayerPrefs.SetFloat("TunnelingAmount", tunnelingAmount);
            PlayerPrefs.SetFloat("TunnelingSpeed", tunnelingSpeed);
        }
        catch (FormatException)
        {
            Debug.Log("Invalid UserID key format. The parameters could not be parsed.");
        }
    }
}

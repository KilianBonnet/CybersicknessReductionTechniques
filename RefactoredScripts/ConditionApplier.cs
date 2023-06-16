using System.Collections;
using Sigtrap.VrTunnellingPro;
using UnityEngine;

public class ConditionApplier : MonoBehaviour
{
    [SerializeField]
    private Tunnelling tunnellingPro;
    
    private OVRPlayerController playerController;

    private void Start() {
        playerController = FindObjectOfType<Player>().GetComponent<OVRPlayerController>();
        OVRPlayerController.SnappingEvent += OnSnappingEvent; // Subscribe to SnappingEvent
    }
    
    /* BUG : At the beginning those 3 parameters where regrouped into ExperimentParameters object
     * However I've got a null pointer exception.
     * TMP FIX: I decided moving on 3 parameters.
     */
    public bool hasRotationSnapping { get; private set; }
    public bool hasTranslationSnapping { get; private set; }
    public bool hasTunneling { get; private set; }

    public void InitializeConditions(ExperimentParameters parameters) {
        // Saving parameters
        hasRotationSnapping = parameters.hasRotationSnapping;
        hasTranslationSnapping = parameters.hasTranslationSnapping;
        hasTunneling = parameters.hasTunneling;
        
        // Setting up experiment parameters to hmd
        InitializeTunneling();
        playerController.SnapRotation = hasRotationSnapping;
        playerController.SnapTranslation = hasTranslationSnapping;
    }
    
    
    /// <summary>
    /// Initialize tunnelings parameters for the given experiment parameters
    /// </summary>
    private void InitializeTunneling() {
        tunnellingPro.enabled = hasTunneling;

        // Tunneling
        if (PlayerPrefs.HasKey("tunnelingAmount"))
            tunnellingPro.effectCoverage = PlayerPrefs.GetFloat("tunnelingAmount");
        if (PlayerPrefs.HasKey("tunnelingSpeed")) {
            tunnellingPro.accelerationStrength = PlayerPrefs.GetFloat("tunnelingSpeed");
            tunnellingPro.angularVelocityStrength = PlayerPrefs.GetFloat("tunnelingSpeed");
        }

        // Rotation
        if (PlayerPrefs.HasKey("SnappingRotation"))
            playerController.RotationRatchet = PlayerPrefs.GetFloat("SnappingRotation");
        if (PlayerPrefs.HasKey("SnappingRotationSpeed"))
            playerController.SnapRotationCooldown = PlayerPrefs.GetFloat("SnappingRotationSpeed");

        // Translation
        if (PlayerPrefs.HasKey("SnappingTranslationSpeed"))
            playerController.SnapTranslationCooldown = PlayerPrefs.GetFloat("SnappingTranslationSpeed");
        if (PlayerPrefs.HasKey("SnappingTranslationDistance"))
            playerController.SnapDistance = PlayerPrefs.GetFloat("SnappingTranslationDistance");
    }
    
    /// <summary>
    /// Called when the player snap : During Translation Snapping ou  Rotation Snapping 
    /// </summary>
    private void OnSnappingEvent(SnappingType snappingType, float cooldown) {
        switch (snappingType) {
            case SnappingType.Rotation:
                tunnellingPro.angularVelocityMax = 0;
                break;
            case SnappingType.Translation:
                tunnellingPro.accelerationMax = 0;
                break;
        }
        StartCoroutine(StopSnappingVignette(snappingType, cooldown));
    }
    
    private IEnumerator StopSnappingVignette(SnappingType snappingType, float cooldown) {
        yield return new WaitForSeconds(cooldown);
        
        switch (snappingType) {
            case SnappingType.Rotation:
                tunnellingPro.angularVelocityMax = 13;
                break;
            case SnappingType.Translation:
                tunnellingPro.accelerationMax = 13;
                break;
        }
    }
}

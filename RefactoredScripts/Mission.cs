using System.Globalization;
using System.Linq;
using Sigtrap.VrTunnellingPro;
using UnityEngine;
using UnityEngine.UI;

enum MissionState {
    WaitingInit,
    StartMenu,
    Playing,
    WaitingForStartPoint,
    NauseaMenu
}

[RequireComponent(typeof(Timers))]
public class Mission : MonoBehaviour
{
    [SerializeField] 
    private Transform playerTransform;
    private Player playerScript;
    private OVRPlayerController playerController;
    private CharacterController characterController;
    private CheckpointCollisionManager checkpointCollisionManager;
    
    private GameManager gameManager;

    [Tooltip("The parent GameObject of all the checkpoints")]
    [SerializeField]
    private Transform checkpoints;

    [SerializeField]
    private Tunnelling tunnellingPro;
    
    [Header("UI Objects")]
    [SerializeField]
    private Text objectiveText;
    
    [SerializeField]
    private GameObject selectables, canvas;
    [SerializeField]
    private Compass compass;

    [Header("Other References")]
    [SerializeField]
    private GameObject sciFiGunLightBlack;

    private Timers timers;
    private MissionState missionState = MissionState.WaitingInit;
    
    // Prevent string format to display float as 1,111 instead of 1.111
    private CultureInfo usCultureInfo = new("en-US"); 

    /* BUG : At the beginning those 3 parameters where regrouped into ExperimentParameters object
     * However I've got a null pointer exception.
     * TMP FIX: I decided moving on 3 parameters.
     */
    private bool hasRotationSnapping;
    private bool hasTranslationSnapping;
    private bool hasTunneling;
    
    private void Start()
    {
        playerController = playerTransform.GetComponent<OVRPlayerController>();
        characterController = playerTransform.GetComponent<CharacterController>();
        playerScript = playerTransform.GetComponent<Player>();
        checkpointCollisionManager = playerTransform.GetComponent<CheckpointCollisionManager>();

        gameManager = FindObjectOfType<GameManager>();
            
        timers = GetComponent<Timers>();
    }

    /// <summary>
    /// Initialize a mission with the given experiment parameters
    /// </summary>
    public void InitializeMission(ExperimentParameters parameters) {
        // Saving parameters
        hasRotationSnapping = parameters.hasRotationSnapping;
        hasTranslationSnapping = parameters.hasTranslationSnapping;
        hasTunneling = parameters.hasTunneling;
        
        // Logs
        print($"Starting {gameObject.name} with parameters:, " +
                  $"hasRotationSnapping: {hasRotationSnapping}, " +
                  $"hasTranslationSnapping: {hasTranslationSnapping}, " +
                  $"hasTunneling: {hasTunneling}"
                  );
        
        // Teleporting player and block its movements
        TeleportPlayer(checkpoints.GetChild(0).position);
        playerController.canMove = false;

        // Setting up experiment parameters to hmd
        InitializeTunneling();
        playerController.SetSnapRotation(hasRotationSnapping);
        playerController.SetSnapTranslation(hasTranslationSnapping);
        
        // Updating ui
        compass.gameObject.SetActive(false);
        objectiveText.text = "Look around and when ready\npress the Y button!";
        objectiveText.color = new Color(255f, 255f, 255f);
        
        missionState = MissionState.StartMenu; // Updating mission state
        checkpointCollisionManager.InitializeMission(this); // Initialize checkpoints
    }

    /// <summary>
    /// After the player press the "Y" button, start the mission
    /// </summary>
    private void StartMission() {
        missionState = MissionState.Playing;
        compass.gameObject.SetActive(true);
        playerController.canMove = true;
        timers.checkpointTimeActive = true;
        
        UpdateCheckpointObjective(0);
    }

    /// <summary>
    /// This method is called after the player has reached the last checkpoint
    /// </summary>
    private void ReturnMission() {
        objectiveText.text = "Go to initial position";
        objectiveText.color = new Color(255f, 0.0f, 0.0f);
        
        timers.timeOfReturnActive = true;
        compass.gameObject.SetActive(false);

        missionState = MissionState.WaitingForStartPoint;
    }

    /// <summary>
    /// This method is called after the player had returned to the departure and trigger the controller
    /// This should start the sickness evaluation screen
    /// </summary>
    private void EvaluateMission() {
        missionState = MissionState.NauseaMenu;
        timers.timeOfReturnActive = false;
        playerController.canMove = false;
        compass.gameObject.SetActive(false);
        
        canvas.SetActive(true);
        selectables.SetActive(true);
        sciFiGunLightBlack.SetActive(true);
    }

    /// <summary>
    /// Teleport the player to the given point
    /// BUG: characterController has to be disabled & enabled to let the player be teleported
    /// </summary>
    private void TeleportPlayer(Vector3 point) {
        characterController.enabled = false;
        playerTransform.position = point;
        characterController.enabled = true;
    }

    /// <summary>
    /// Initialize tunnelings parameters for the given experiment parameters
    /// </summary>
    private void InitializeTunneling() {
        if (!hasTunneling) {
            tunnellingPro.enabled = false;
            return;
        }
            
        tunnellingPro.enabled = hasTunneling;
        
        if (hasRotationSnapping) {
            tunnellingPro.angularVelocityMax = 0;
            tunnellingPro.accelerationMax = 0;
        }
        else {
            tunnellingPro.angularVelocityMax = 13;
            tunnellingPro.accelerationMax = 0.5f;
        }
        
        // Code from the version 2 of the project
        if (PlayerPrefs.HasKey("tunnelingAmount"))
            tunnellingPro.effectCoverage = PlayerPrefs.GetFloat("tunnelingAmount");

        if (PlayerPrefs.HasKey("tunnelingSpeed"))
            tunnellingPro.accelerationStrength = PlayerPrefs.GetFloat("tunnelingSpeed");
    }

    private void Update() {
        if(missionState == MissionState.WaitingInit)
            return;
        
        CheckForBeginMissionInput();
        CheckForEvaluateMissionInput();
    }

    private void CheckForBeginMissionInput() {
        // Check for player to press "Y" button
        if(missionState != MissionState.StartMenu || !OVRInput.GetDown(OVRInput.Button.Four))
            return;
        StartMission();
    }

    private void CheckForEvaluateMissionInput() {
        if(missionState != MissionState.WaitingForStartPoint || !OVRInput.GetDown(OVRInput.Button.Four))
            return;
        EvaluateMission();
    }

    /// <summary>
    /// Called after a checkpoint is reached
    /// </summary>
    public void OnCheckpointReached(int checkpointIndex) {
        UpdatePlayerLogs(checkpointIndex);
        
        // If there no other checkpoints to explore
        if (checkpointIndex >= checkpoints.childCount - 1) 
            ReturnMission();
        else 
            UpdateCheckpointObjective(checkpointIndex);
    }

    /// <summary>
    /// This method is called after the Player select a nausea level
    /// </summary>
    public void OnNauseaScoreSelection(int nauseaScore) {
        canvas.SetActive(false);
        selectables.SetActive(false);
        sciFiGunLightBlack.SetActive(false);
        
        Vector3 initialPosition = checkpoints.GetChild(0).transform.position;
        float distance = Mathf.Sqrt(
            Mathf.Pow(initialPosition.x - playerTransform.position.x, 2) +
            Mathf.Pow(initialPosition.z - playerTransform.transform.position.z, 2)
            );
        UpdatePlayerAverageLogs(distance, nauseaScore);
        timers.ResetTimers();
        gameManager.EndMission();
    }

    /// <summary>
    /// For a given checkpoint index, update the objective to the next checkpoint index
    /// </summary>
    private void UpdateCheckpointObjective(int checkpointIndex) {
        Transform newCheckpoint = checkpoints.GetChild(++checkpointIndex);
        newCheckpoint.gameObject.SetActive(true); // Enable the next checkpoint
        compass.target = newCheckpoint.position;

        // Update objective ui (text & colour)
        objectiveText.text = "Go to checkpoint " + checkpointIndex;
        objectiveText.color = new Color(255f, 255f, 255f);
    }

    /// <summary>
    /// Retrieve the number of the mission
    /// </summary>
    private string GetMissionNumber() {
        return gameObject.name.Split().Last();
    }
    
    private void UpdatePlayerLogs(int checkpointIndex) {
        playerScript.playerData.Append(PlayerPrefs.GetString("ParticipantNumber") + ";");
        playerScript.playerData.Append(PlayerPrefs.GetInt("GroupNumber") + ";");
        playerScript.playerData.Append(PlayerPrefs.GetInt("DayNumber") + ";");
            
        playerScript.playerData.AppendFormat("{0};", 0); // TODO: Adding blockCounter
        playerScript.playerData.AppendFormat("{0};", gameManager.conditionCounter);
        
        playerScript.playerData.AppendFormat("{0};", GetMissionNumber());
        playerScript.playerData.AppendFormat("{0};", checkpointIndex);
        
        playerScript.playerData.AppendFormat("{0};", hasRotationSnapping);
        playerScript.playerData.AppendFormat("{0};", hasTranslationSnapping);
        playerScript.playerData.AppendFormat("{0};", hasTunneling);
        playerScript.playerData.AppendFormat(usCultureInfo, "{0:f6};\n", timers.GetElapsedTime());
    }

    private void UpdatePlayerAverageLogs(float distance, int nauseaScore) {
        playerScript.playerDataAverage.Append(PlayerPrefs.GetString("ParticipantNumber") + ";");
        playerScript.playerDataAverage.Append(PlayerPrefs.GetInt("GroupNumber") + ";");
        playerScript.playerDataAverage.Append(PlayerPrefs.GetInt("DayNumber") + ";");
        
        playerScript.playerDataAverage.AppendFormat("{0};", 0); // TODO: Adding blockCounter
        playerScript.playerDataAverage.AppendFormat("{0};", gameManager.conditionCounter);
        
        playerScript.playerDataAverage.AppendFormat("{0};", GetMissionNumber());
        playerScript.playerDataAverage.AppendFormat(usCultureInfo, "{0:f6};", timers.time / checkpoints.childCount - 1);
        playerScript.playerDataAverage.AppendFormat(usCultureInfo, "{0:f6};", timers.timeOfReturn);
        playerScript.playerDataAverage.AppendFormat("{0};",nauseaScore);
        playerScript.playerDataAverage.AppendLine(distance + ";");
    }
}
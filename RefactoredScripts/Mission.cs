using System.Linq;
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
    private ConditionApplier conditionApplier;

    [Tooltip("The parent GameObject of all the checkpoints")]
    [SerializeField]
    private Transform checkpoints;

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

    private DataLogger dataLogger;
    
    private Timers timers;
    private MissionState missionState = MissionState.WaitingInit;

    private void Start()
    {
        playerController = playerTransform.GetComponent<OVRPlayerController>();
        characterController = playerTransform.GetComponent<CharacterController>();
        playerScript = playerTransform.GetComponent<Player>();
        checkpointCollisionManager = playerTransform.GetComponent<CheckpointCollisionManager>();

        conditionApplier = FindObjectOfType<ConditionApplier>();
        gameManager = FindObjectOfType<GameManager>();
        timers = GetComponent<Timers>();
    }

    /// <summary>
    /// Initialize a mission with the given experiment parameters
    /// </summary>
    public void InitializeMission() {
        // Teleporting player and block its movements
        TeleportPlayer(checkpoints.GetChild(0).position);
        playerController.CanMove = false;
        
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
        playerController.CanMove = true;
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
        playerController.CanMove = false;
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
        DataPayload data = new DataPayload(
            gameManager.blockCounter,
            gameManager.conditionCounter,
            checkpointIndex,
            GetMissionNumber(),
            conditionApplier.hasRotationSnapping,
            conditionApplier.hasTranslationSnapping,
            conditionApplier.hasTunneling,
            timers.GetElapsedTime()
        );
        DataLogger.UpdatePlayerLogs(playerScript, data);
    }

    private void UpdatePlayerAverageLogs(float distance, int nauseaScore) {
        DataPayloadAverage data = new DataPayloadAverage(
            gameManager.blockCounter,
            gameManager.conditionCounter,
            distance,
            nauseaScore,
            GetMissionNumber(),
            timers.time / checkpoints.childCount - 1,
            timers.timeOfReturn
        );
        DataLogger.UpdatePlayerAverageLogs(playerScript, data);
    }
}

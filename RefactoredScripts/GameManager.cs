using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Associate a given list of group numbers to an index matrix
/// </summary>
[Serializable]
public struct Group {
    public int[] groupNumbers;
    public int[] experimentParametersOrder;
    
    public Group(int[] groupNumbers, int[] experimentParametersOrder) {
        this.groupNumbers = groupNumbers;
        this.experimentParametersOrder = experimentParametersOrder;
    }
}

/// <summary>
/// Class representing all the experimental parameters
/// </summary>
public class ExperimentParameters
{
    public readonly bool hasRotationSnapping;
    public readonly bool hasTranslationSnapping;
    public bool hasTunneling;

    public ExperimentParameters(bool hasRotationSnapping, bool hasTranslationSnapping)
    {
        this.hasRotationSnapping = hasRotationSnapping;
        this.hasTranslationSnapping = hasTranslationSnapping;
        hasTunneling = false;
    }

    public override string ToString() {
        StringBuilder o = new ();
        o.AppendLine("hasRotationSnapping: " + hasRotationSnapping);
        o.AppendLine("hasTranslationSnapping: " + hasTranslationSnapping);
        o.AppendLine("hasTunneling: " + hasTunneling);
        return o.ToString();
    }
}

public class GameManager : MonoBehaviour
{   
    [SerializeField]
    [Tooltip("Associate group number to a certain experiment parameters order.")]
    private Group[] groups = {
        new (new [] {1, 5}, new [] { 0, 1, 2, 3 }),
        new (new [] {2, 6}, new [] { 1, 2, 3, 0 }),
        new (new [] {3, 7}, new [] { 2, 3, 0, 1 }),
        new (new [] {4, 8}, new [] { 3, 0, 1, 2 })
    };
    
    private readonly ExperimentParameters[] experimentalParametersArray = {
        new (false, false), // 0
        new (false, true),  // 1
        new (true, false),  // 2
        new (true, true)    // 3
    };
    private int[] playerExperimentParametersOrder;
    public int conditionCounter { get; private set; }
    
    private Player player;
    private MailTo mailTo;

    [SerializeField]
    private Mission[] missions = new Mission[3];
    private int missionCounter;

    [SerializeField]
    private Text trialUI;
    
    private void Start() {
        int playerGroupNumber = PlayerPrefs.GetInt("GroupNumber");
        int playerDayNumber = PlayerPrefs.GetInt("DayNumber");

        player = FindObjectOfType<Player>();
        mailTo = FindObjectOfType<MailTo>();

        InitializePlayerExperimentParametersOrder(playerGroupNumber);
        ArrayHelper.ShuffleArray(missions); // Randomize missions order
        InitializeTunnelingConditions(playerGroupNumber, playerDayNumber);

        StartMission();
    }

    /// <summary>
    /// Starting a mission according to the current Game Manager parameters
    /// </summary>
    private void StartMission() {
        RefreshMissionCounter();
        Mission mission = GetCurrentMission();
        ExperimentParameters parameters = experimentalParametersArray[conditionCounter];

        mission.enabled = true;
        mission.InitializeMission(parameters);
    }
    
    /// <summary>
    /// Retrieve the current mission
    /// </summary>
    public Mission GetCurrentMission() {
        return missions[missionCounter];
    }

    /// <summary>
    /// Initialize the player experiment parameters order based on the groups list and the group number given in PlayerPrefs.
    /// </summary>
    private void InitializePlayerExperimentParametersOrder(int playerGroupNumber) {
        foreach (Group group in groups)
            foreach(int groupNumber in group.groupNumbers)
                if(groupNumber == playerGroupNumber) {
                    playerExperimentParametersOrder = group.experimentParametersOrder;
                    break;
                }

        if (playerExperimentParametersOrder != null) 
            return;
        
        Debug.Log("Group not found");
        SceneManager.LoadScene("End");
    }

    /// <summary>
    /// Update the array of Experiment Parameters to modify hasTunneling based on the group number and the day
    /// </summary>
    private void InitializeTunnelingConditions(int playerGroupNumber, int dayNumber)
    {
        bool hasTunneling = (playerGroupNumber < 5 && dayNumber == 1 || playerGroupNumber > 4 && dayNumber == 2) 
                            && PlayerPrefs.GetFloat("TunnelingAmount") != 0;
        
        //Updating experiment parameters to add hasTunneling information
        foreach (ExperimentParameters parameters in experimentalParametersArray)
            parameters.hasTunneling = hasTunneling;
    }

    /// <summary>
    /// Updating the UI according to the new condition counter and the size of experimental parameters to try.
    /// </summary>
    private void RefreshMissionCounter() {
        trialUI.text = conditionCounter + "/" + experimentalParametersArray.Length;
    }

    /// <summary>
    /// This method is call at the end of a Mission
    /// </summary>
    public void EndMission() {
        Mission currentMission = missions[missionCounter];
        currentMission.enabled = false;
        
        IncreaseCounterVariables();
        // Check if all the experimental parameters were carried out
        if (conditionCounter >= 1) { // TODO : Put experimentalParametersArray.Length after testing
            string[] filePaths = PlayerDataWriter.WritePlayerData(player); // Saving player data
            mailTo.SendEmail(player, filePaths);
            SceneManager.LoadScene("End");
        }
        else StartMission();
    }

    /// <summary>
    /// For the actual state of the game manager, update the state variables to the next iteration
    /// </summary>
    private void IncreaseCounterVariables() {
        missionCounter++; // Going to the next mission
        
        // Going to the next conditions
        if (missionCounter < missions.Length)
            return;
        missionCounter = 0;
        conditionCounter++;
    }
}
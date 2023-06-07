using System.Text;
using UnityEngine;

[RequireComponent(typeof(OVRPlayerController))]
[RequireComponent(typeof(CheckpointCollisionManager))]
public class Player : MonoBehaviour {
    public string playerName {  get; private set; }
    public readonly StringBuilder playerData = new();
    public readonly StringBuilder playerDataAverage = new();

    private void Start() {
        playerName = PlayerPrefs.GetString("ParticipantNumber") != ""
            ? PlayerPrefs.GetString("ParticipantNumber")
            : "Anonymous";
        
        playerData.AppendLine("PID;Group;Day;Block;Condition;Environment;Checkpoint;VS;TS;Tunnel;Time;");
        playerDataAverage.AppendLine("PID;Group;Day;Block;Condition;Environment;Time(Avg);Time of Return;Nausea Score;distance;");
    }
}

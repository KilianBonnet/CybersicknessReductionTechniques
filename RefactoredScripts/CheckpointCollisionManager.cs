using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Collider))]
public class CheckpointCollisionManager : MonoBehaviour {
    private Mission mission;
    private int checkpointIndex;

    /// <summary>
    /// Initialize the object fot a given mission
    /// </summary>
    public void InitializeMission(Mission currentMission) {
        mission = currentMission;
        checkpointIndex = 1;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("checkpoint"))
            return;
        mission.OnCheckpointReached(checkpointIndex++);
        other.gameObject.SetActive(false);
    }
}

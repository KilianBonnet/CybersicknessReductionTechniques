using System.Globalization;
using UnityEngine;

public class DataPayload {
    public readonly int blockCounter;
    public readonly int conditionCounter;
    
    public readonly int checkpointIndex;
    public readonly string  missionNumber;
    
    public readonly bool hasRotationSnapping;
    public readonly bool hasTranslationSnapping;
    public readonly bool hasTunneling;

    public readonly float elapsedTime;

    public DataPayload(int blockCounter,int conditionCounter, int checkpointIndex, string missionNumber, bool hasRotationSnapping, bool hasTranslationSnapping, bool hasTunneling, float elapsedTime) {
        this.blockCounter = blockCounter;
        this.conditionCounter = conditionCounter;
        this.checkpointIndex = checkpointIndex;
        this.missionNumber = missionNumber;
        this.hasRotationSnapping = hasRotationSnapping;
        this.hasTranslationSnapping = hasTranslationSnapping;
        this.hasTunneling = hasTunneling;
        this.elapsedTime = elapsedTime;
    }
}

public class DataPayloadAverage {
    public readonly int blockCounter;
    public readonly int conditionCounter;
    
    public readonly float distance;
    public readonly int nauseaScore;
    
    public readonly string missionNumber;

    public readonly float averageTime;
    public readonly float timeOfReturn;

    public DataPayloadAverage(int blockCounter, int conditionCounter, float distance, int nauseaScore, string missionNumber, float averageTime, float timeOfReturn) {
        this.blockCounter = blockCounter;
        this.conditionCounter = conditionCounter;
        this.distance = distance;
        this.nauseaScore = nauseaScore;
        this.missionNumber = missionNumber;
        this.averageTime = averageTime;
        this.timeOfReturn = timeOfReturn;
    }
}

public class DataLogger {
    // Prevent string format to display float as 1,111 instead of 1.111
    private static CultureInfo usCultureInfo = new("en-US");

    public static void UpdatePlayerLogs(Player playerScript, DataPayload data) {
        playerScript.playerData.Append(PlayerPrefs.GetString("ParticipantNumber") + ";");
        playerScript.playerData.Append(PlayerPrefs.GetInt("GroupNumber") + ";");
        playerScript.playerData.Append(PlayerPrefs.GetInt("DayNumber") + ";");
            
        playerScript.playerData.AppendFormat("{0};", data.blockCounter);
        playerScript.playerData.AppendFormat("{0};", data.conditionCounter);
        
        playerScript.playerData.AppendFormat("{0};", data.missionNumber);
        playerScript.playerData.AppendFormat("{0};", data.checkpointIndex);
        
        playerScript.playerData.AppendFormat("{0};", data.hasRotationSnapping);
        playerScript.playerData.AppendFormat("{0};", data.hasTranslationSnapping);
        playerScript.playerData.AppendFormat("{0};", data.hasTunneling);
        playerScript.playerData.AppendFormat(usCultureInfo, "{0:f6};\n", data.elapsedTime);
    }

    public static void UpdatePlayerAverageLogs(Player playerScript, DataPayloadAverage data) {
        playerScript.playerDataAverage.Append(PlayerPrefs.GetString("ParticipantNumber") + ";");
        playerScript.playerDataAverage.Append(PlayerPrefs.GetInt("GroupNumber") + ";");
        playerScript.playerDataAverage.Append(PlayerPrefs.GetInt("DayNumber") + ";");
        
        playerScript.playerDataAverage.AppendFormat("{0};", data.blockCounter);
        playerScript.playerDataAverage.AppendFormat("{0};", data.conditionCounter);
        
        playerScript.playerDataAverage.AppendFormat("{0};", data.missionNumber);
        playerScript.playerDataAverage.AppendFormat(usCultureInfo, "{0:f6};", data.averageTime);
        playerScript.playerDataAverage.AppendFormat(usCultureInfo, "{0:f6};", data.timeOfReturn);
        playerScript.playerDataAverage.AppendFormat("{0};", data.nauseaScore);
        playerScript.playerDataAverage.AppendLine(data.distance + ";");
    }
}
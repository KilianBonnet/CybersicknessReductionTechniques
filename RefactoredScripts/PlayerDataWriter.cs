using System.IO;
using UnityEngine;

public static class PlayerDataWriter {
    
    public static string[] WritePlayerData(Player player) {
        string[] paths = new string[2];
        
        string destination = Application.persistentDataPath + "/Data";
        if (!File.Exists(destination)) Directory.CreateDirectory(destination);
        
        paths[0] = destination + "/P" + player.playerName + ".txt";
        using (StreamWriter writer = new (paths[0])) {
                   writer.Write(player.playerData);
                   writer.Close();
        }
        
        paths[1] = destination + "/P" + player.playerName + "Avg.txt";
        using (StreamWriter writer = new StreamWriter(paths[1])) {
            writer.Write(player.playerDataAverage);
            writer.Close();
        }

        return paths;
    }
}
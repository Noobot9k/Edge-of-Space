using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager {
    static string path = Application.persistentDataPath + "/space.balls";

    public static void SavePlayerData(int seed, int starIndex, PlayerHandler player) {
        Debug.Log("Saving the player's data.");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(seed, starIndex, player);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayerData(PlayerHandler player) {
        if(File.Exists(path)) {
            Debug.Log("Loading the player's saved data.");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            player.Fuel = data.Fuel;
            player.MaxFuel = data.MaxFuel;
            player.Health = data.Health;
            player.MaxHealth = data.MaxHealth;
            player.FireRate = data.FireRate;
            player.IsFullAuto = data.IsFullAuto;
            player.Metal = data.Metal;
            player.CurrentUpgradeIndex = data.CurrentUpgradeIndex;

            return data;
        } else {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }
    public static void DeletePlayerData() {
        Debug.Log("Deleting the player's saved data.");
        File.Delete(path);
    }
}

[System.Serializable]
public class PlayerData {
    public int SystemSeed;
    public int StarsystemIndex;

    public float Fuel = 0;
    public float MaxFuel = 0;
    public float Health = 100;
    public float MaxHealth = 100;
    public float FireRate = 4;
    public bool IsFullAuto = true;
    public float Metal = 0;
    public int CurrentUpgradeIndex = 0;

    public PlayerData(int seed, int starIndex ) {
        SystemSeed = seed;
        StarsystemIndex = starIndex;
    }
    public PlayerData(int seed, int starIndex, PlayerHandler player) {
        SystemSeed = seed;
        StarsystemIndex = starIndex;

        Fuel = player.Fuel;
        MaxFuel = player.MaxFuel;
        Health = player.Health;
        MaxHealth = player.MaxHealth;
        FireRate = player.FireRate;
        IsFullAuto = player.IsFullAuto;
        Metal = player.Metal;
        CurrentUpgradeIndex = player.CurrentUpgradeIndex;
    }
}

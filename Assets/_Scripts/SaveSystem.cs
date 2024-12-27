using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    private static string GetFilePath() => Application.persistentDataPath + "/gameData.dat";

    public static void Save(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GetFilePath();

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }

        Debug.Log("Data saved to " + path);
    }

    public static GameData Load()
    {
        string path = GetFilePath();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(stream) as GameData;
            }
        }
        else
        {
            Debug.LogWarning("No save file found at " + path);
            return null;
        }
    }

    public static void UpdateHighScore(int newHighScore)
    {
        GameData data = Load() ?? new GameData();
        data.highScore = newHighScore;
        Save(data);
    }

    public static void UpdateTotalCoins(int newTotalCoins)
    {
        GameData data = Load() ?? new GameData();
        data.totalCoins = newTotalCoins;
        Save(data);
    }
    public static void UpdateTotalScore(int newTotalScore)
    {
        GameData data = Load() ?? new GameData();
        data.totalScore = newTotalScore;
        Save(data);
    }
    public static void UpdateCurrentlySelectedSkin(int skin)
    {
        GameData data = Load() ?? new GameData();
        data.currentlySelectedSkinIndex = skin;
        Save(data);
    }

    public static void AddSkinToUnlocked(int newSkinIndex)
    {
        GameData data = Load() ?? new GameData();
        data.skinsUnlocked.Add(newSkinIndex);
        Save(data);
    }
}
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int highScore;
    public int totalCoins;
    public int totalScore;
    public int currentlySelectedSkinIndex;
    public List<int> skinsUnlocked;

    //Default constructor
    public GameData()
    {
        highScore = 0;
        totalCoins = 0;
        totalScore = 0;
        currentlySelectedSkinIndex = 0;
        skinsUnlocked = new List<int>();
        skinsUnlocked.Add(0);
    }
}

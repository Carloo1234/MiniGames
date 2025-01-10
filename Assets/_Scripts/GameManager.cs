// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private UiManager uiManager;
    private int currentScore;
    public int totalPerfectScore;
    private int highScore;
    private int totalScore;
    private int currentCoins;
    private bool isGameOver = false;
    public int multiplyer = 1;
    private int perfectsCount = 1;
    public bool perfectShot = true;
    public bool isPaused = false;
    private bool isNewDay;

    [Header("Audio")]
    [SerializeField] AudioSource buttonClick;

    //public List<Sprite> balls;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        isNewDay = IsNewDay();
        PlayerPrefs.SetString("LastActiveDate", System.DateTime.Now.ToString("yyyy-MM-dd"));
        if (isNewDay) GenerateRandomQuest();
    }

    private bool IsNewDay()
    {
        string lastActiveDate = PlayerPrefs.GetString("LastActiveDate", string.Empty);
        string currentDate = System.DateTime.Now.ToString("yyyy-MM-dd");
        return lastActiveDate != currentDate;
    }

    private void GenerateRandomQuest()
    {
        int objectiveType = Random.Range(0, 2); // 0 for perfect hoopins, 1 for hoopins
        int objectiveCount = Random.Range(20, 61); // Random number between 5 and 20
        int coinReward = Random.Range(20, 61); // Random number between 20 and 50

        PlayerPrefs.SetInt("ObjectiveType", objectiveType);
        PlayerPrefs.SetInt("ObjectiveCount", objectiveCount);
        PlayerPrefs.SetInt("CoinReward", coinReward);
        PlayerPrefs.SetInt("DailyQuestCompleted", 0);
        PlayerPrefs.SetInt("DailyHoopins", 0);
        PlayerPrefs.SetInt("DailyPerfectHoopins", 0);

    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        totalPerfectScore = 0;
        currentScore = 0;
        currentCoins = 0;
        isGameOver = false;
        multiplyer = 1;
        perfectsCount = 1;
        perfectShot = true;
        Time.timeScale = 1;

        // Find UiManager in the newly loaded scene
        uiManager = FindObjectOfType<UiManager>();
        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(currentScore);
            uiManager.HideAllUI();
            uiManager.ShowGameUI();
        }

        // Load high score and total score
        GameData data = SaveSystem.Load() ?? new GameData();
        highScore = data.highScore;
        totalScore = data.totalScore;
    }

    public void AddCoin(GameObject coin)
    {
        coin.GetComponentInChildren<Animator>().SetBool("CoinIsWon", true);
        coin.GetComponentInChildren<Collider2D>().enabled = false;
        GameData data = SaveSystem.Load() ?? new GameData();
        //int coins = PlayerPrefs.GetInt("TotalCoins", 0);
        SaveSystem.UpdateTotalCoins(++data.totalCoins);
        currentCoins++;
    }

    public void UpdateCoinUI()
    {
        uiManager.UpdateCoinsUI(currentCoins);
    }

    public void AddPoint(Vector3 hoopPosition)
    {
        if (isGameOver) return;

        if (perfectShot)
        {
            PlayerPrefs.SetInt("DailyPerfectHoopins", PlayerPrefs.GetInt("DailyPerfectHoopins") + 1);
            totalPerfectScore++;
            if (uiManager) uiManager.ShowPerfectText(hoopPosition);

            perfectsCount++;
            if (perfectsCount > 2)
            {
                multiplyer++;
                if (uiManager) uiManager.ShowComboText(multiplyer);
            }
            GameData data = SaveSystem.Load() ?? new GameData();
            data.totalPerfectScore++;
            SaveSystem.Save(data);
        }
        else
        {
            PlayerPrefs.SetInt("DailyHoopins", PlayerPrefs.GetInt("DailyHoopins") + 1);
            if (uiManager) uiManager.HideComboText();
            multiplyer = 1;
            perfectsCount = 1;
        }

        currentScore += 1 * multiplyer;

        totalScore += 1 * multiplyer;
        SaveSystem.UpdateTotalScore(totalScore);


        if (uiManager) uiManager.UpdateScoreUI(currentScore);
        perfectShot = true;

        PlayerPrefs.Save();
    }

    public void PauseGame()
    {
        if (isGameOver) return;
        isPaused = true;
        Time.timeScale = 0;
        if (uiManager != null)
        {
            uiManager.ShowPauseUI(currentScore);
        }
        PlayerPrefs.Save();
    }

    public void GameOver()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        data.totalGamesPlayed++;
        SaveSystem.Save(data);
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0;
        bool isNewHighScore = currentScore > highScore;
        if (isNewHighScore)
        {
            highScore = currentScore;
            SaveSystem.UpdateHighScore(highScore);
        }

        if (uiManager != null)
        {
            uiManager.ShowGameOverUI(currentScore, isNewHighScore, totalPerfectScore);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); 
    }

    public void ButtonClickSound()
    {
        buttonClick.Stop();
        buttonClick.Play();
    }
    
}
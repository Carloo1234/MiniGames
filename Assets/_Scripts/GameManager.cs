// GameManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private UiManager uiManager;
    private int currentScore;
    private int highScore;
    private int totalScore;
    private bool isGameOver = false;
    public int multiplyer = 1;
    private int perfectsCount = 1;

    public List<Sprite> balls;
    

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        currentScore = 0;
        isGameOver = false;
        multiplyer = 1;
        perfectsCount = 1;
        Time.timeScale = 1;

        // Find UiManager in the newly loaded scene
        uiManager = FindObjectOfType<UiManager>();
        if (uiManager == null)
        {
            //Debug.LogError("UiManager not found in the scene!");
        }
        else
        {
            uiManager.UpdateScoreUI(currentScore);
            uiManager.HideAllUI();
            uiManager.ShowGameUI();
        }

        // Load high score and total score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    public void AddPoint(bool isPerfect, Vector3 hoopPosition)
    {
        if (isGameOver) return;

        if (isPerfect)
        {
            if (uiManager) uiManager.ShowPerfectText(hoopPosition);

            perfectsCount++;
            if (perfectsCount > 2)
            {
                multiplyer++;
                if (uiManager) uiManager.ShowComboText(multiplyer);
            }
        }
        else
        {
            if (uiManager) uiManager.HideComboText();
            multiplyer = 1;
            perfectsCount = 1;
        }

        currentScore += 1 * multiplyer;
        totalScore += 1 * multiplyer;
        PlayerPrefs.SetInt("TotalScore", totalScore);
        PlayerPrefs.Save();

        if (uiManager != null)
        {
            uiManager.UpdateScoreUI(currentScore);
        }
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        Time.timeScale = 0;
        if (uiManager != null)
        {
            uiManager.ShowPauseUI(currentScore);
        }
    }

    public void ResumeGame()
    {
        if (isGameOver) return;

        Time.timeScale = 1;
        if (uiManager != null)
        {
            uiManager.HidePauseUI();
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0;
        bool isNewHighScore = currentScore > highScore;
        if (isNewHighScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        PlayerPrefs.Save();

        if (uiManager != null)
        {
            uiManager.ShowGameOverUI(currentScore, isNewHighScore);
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
}
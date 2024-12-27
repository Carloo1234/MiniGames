// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Stuff")]
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject shopMenuCanvas;
    [SerializeField] private GameObject codeMenuCanvas;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private Image selectedPlayerSprite;
    [Header("Shop stuff")]
    [SerializeField] private GameObject[] inactiveGridSections;
    [SerializeField] private GameObject[] activeGridSections;
    [SerializeField] private TMP_Text shopPointsText;
    [SerializeField] private TMP_Text shopCoinsText;
    [SerializeField] private int[] pointRequirments;

    private int ballsUnlocked = 0;
    private int currentlySelectedBall = 0;

    private void Start()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        currentlySelectedBall = data.currentlySelectedSkinIndex;
        //currentlySelectedBall = PlayerPrefs.GetInt("SelectedBallIndex", 0);
        selectedPlayerSprite.sprite = GameManager.Instance.balls[currentlySelectedBall];
        mainMenuCanvas.SetActive(true);
        shopMenuCanvas.SetActive(false);
        UpdateHighScoreDisplay();
        UpdateTotalScoreDisplay();
    }

    private void UpdateHighScoreDisplay()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        int highScore = data.highScore;
        //int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    private void UpdateTotalScoreDisplay()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        int totalScore = data.totalScore;
        //int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
        if (totalScoreText != null)
        {
            totalScoreText.text = "Total Score: " + totalScore.ToString();
        }
    }

    public void OnShopButtonClicked()
    {
        if (mainMenuCanvas) mainMenuCanvas.SetActive(false);
        if (shopMenuCanvas) shopMenuCanvas.SetActive(true);
        InitializeShop();
    }
    public void OnCodeButtonClicked()
    {
        if (mainMenuCanvas) mainMenuCanvas.SetActive(false);
        if (codeMenuCanvas) codeMenuCanvas.SetActive(true);
    }
    public void OnConfirmButtonClicked()
    {
        string text = codeText.text;
        GameData data = SaveSystem.Load() ?? new GameData();
        int totalScore = data.totalScore;
        if (text == "POOP1000")
        {
            //int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
            SaveSystem.UpdateTotalScore(totalScore += 1000);
            //PlayerPrefs.SetInt("TotalScore", totalScore += 1000);
        }
        else if (text == "POOP-1000")
        {
            SaveSystem.UpdateTotalScore(totalScore -= 1000);
            //PlayerPrefs.SetInt("TotalScore", totalScore -= 1000);
        }
        else if (text == "POOPfreset")
        {
            SaveSystem.UpdateTotalScore(0);
            SaveSystem.UpdateHighScore(0);
        }
        else if (text == "POOPhreset")
        {
            SaveSystem.UpdateHighScore(0);
        }
        else if (text == "POOPtreset")
        {
            SaveSystem.UpdateTotalScore(0);
        }
    }
    public void OnBackButtonClicked()
    {
        if (mainMenuCanvas) mainMenuCanvas.SetActive(true);
        if (shopMenuCanvas) shopMenuCanvas.SetActive(false);
        if (codeMenuCanvas) codeMenuCanvas.SetActive(false);
        selectedPlayerSprite.sprite = GameManager.Instance.balls[currentlySelectedBall];
        UpdateHighScoreDisplay();
        UpdateTotalScoreDisplay();
    }

    private void InitializeShop()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        ballsUnlocked = 0;
        currentlySelectedBall = data.currentlySelectedSkinIndex;
        int currentPoints = data.totalScore;
        int currentCoins = data.totalCoins;

        shopPointsText.text = currentPoints.ToString() + " Points";
        shopCoinsText.text = currentCoins.ToString();

        //calculate balls unlocked
        foreach (var requiredNum in pointRequirments)
        {
            if (currentPoints < requiredNum)
            {
                break;
            }
            ballsUnlocked++;
        }

        //Manage ball slot states.
        for (int i = 0; i < inactiveGridSections.Length; i++)
        {
            if( i < ballsUnlocked) 
            {
                inactiveGridSections[i].transform.localScale = Vector3.zero;
                activeGridSections[i].transform.localScale = Vector3.one;
                if (i == currentlySelectedBall)
                {
                    activeGridSections[i].GetComponent<Image>().color = new Color(0.788f, 0.6f, 0.462f); // selected color
                }
                else
                {
                    activeGridSections[i].GetComponent<Image>().color = new Color(0.729f, 0.510f, 0.357f); // normal color
                }
            }
            else
            {
                inactiveGridSections[i].transform.localScale = Vector3.one;
                activeGridSections[i].transform.localScale = Vector3.zero;
            }
        }
    }

    public void OnActiveShopButtonClicked(int buttonIndex)
    {
        // Done incase ball is removed in update.
        if (currentlySelectedBall < activeGridSections.Length)
        {
            activeGridSections[currentlySelectedBall].GetComponent<Image>().color = new Color(0.729f, 0.510f, 0.357f);
        }

        activeGridSections[buttonIndex].GetComponent<Image>().color = new Color(0.788f, 0.6f, 0.462f);
        currentlySelectedBall = buttonIndex;
        SaveSystem.UpdateCurrentlySelectedSkin(currentlySelectedBall);
        //PlayerPrefs.SetInt("SelectedBallIndex", currentlySelectedBall);
        //PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
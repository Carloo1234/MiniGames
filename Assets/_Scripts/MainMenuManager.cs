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
    [SerializeField] private int[] pointRequirments;

    private int ballsUnlocked = 0;
    private int currentlySelectedBall = 0;

    private void Start()
    {
        currentlySelectedBall = PlayerPrefs.GetInt("SelectedBallIndex", 0);
        selectedPlayerSprite.sprite = GameManager.Instance.balls[currentlySelectedBall];
        mainMenuCanvas.SetActive(true);
        shopMenuCanvas.SetActive(false);
        UpdateHighScoreDisplay();
        UpdateTotalScoreDisplay();
    }

    private void UpdateHighScoreDisplay()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    private void UpdateTotalScoreDisplay()
    {
        int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
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
        if (text == "POOP1000")
        {
            int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
            PlayerPrefs.SetInt("TotalScore", totalScore += 1000);
        }
        else if (text == "POOP-1000")
        {
            int totalScore = PlayerPrefs.GetInt("TotalScore", 0);
            PlayerPrefs.SetInt("TotalScore", totalScore -= 1000);
            if (PlayerPrefs.GetInt("TotalScore", 0) < 0)
            {
                PlayerPrefs.SetInt("TotalScore", 0);
            }
        }
        else if (text == "POOPfreset")
        {
            PlayerPrefs.SetInt("TotalScore", 0);
            PlayerPrefs.SetInt("HighScore", 0);
        }
        else if (text == "POOPhreset")
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }
        else if (text == "POOPtreset")
        {
            PlayerPrefs.SetInt("TotalScore", 0);
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
        ballsUnlocked = 0;
        currentlySelectedBall = PlayerPrefs.GetInt("SelectedBallIndex", 0);
        int currentPoints = PlayerPrefs.GetInt("TotalScore", 0);
        shopPointsText.text = currentPoints.ToString() + " Points";
        foreach (var requiredNum in pointRequirments)
        {
            if (currentPoints < requiredNum)
            {
                break;
            }
            ballsUnlocked++;
        }
        for (int i = 0; i < inactiveGridSections.Length; i++)
        {
            if( i < ballsUnlocked)
            {
                inactiveGridSections[i].transform.localScale = Vector3.zero;
                activeGridSections[i].transform.localScale = Vector3.one;
                if (i == currentlySelectedBall)
                {
                    activeGridSections[i].GetComponent<Image>().color = new Color(0.788f, 0.6f, 0.462f);
                }
                else
                {
                    activeGridSections[i].GetComponent<Image>().color = new Color(0.729f, 0.510f, 0.357f);
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
        PlayerPrefs.SetInt("SelectedBallIndex", currentlySelectedBall);
        PlayerPrefs.Save();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
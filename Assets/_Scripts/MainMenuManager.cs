// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Stuff")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject shopMenuCanvas;
    [SerializeField] private GameObject codeMenuCanvas;
    [SerializeField] private GameObject questMenuCanvas;
    [SerializeField] private GameObject statsMenuCanvas;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private Image selectedPlayerSprite;
    [SerializeField] private TMP_Text menuCoinsText;
    [Header("Shop stuff")]
    [SerializeField] private Transform skinsParent;
    [SerializeField] private TMP_Text shopCoinsText;
    [SerializeField] private SkinDatabase skinData;

    [SerializeField] private AudioSource selectSound;
    [SerializeField] private AudioSource purchaseSound;
    [SerializeField] private AudioSource errorSound;
    [SerializeField] private RectTransform errorLabel;

    [SerializeField] float shakeDuration = 0.5f; 
    [SerializeField] float shakeStrength = 10f; 


    private List<int> skinsUnlocked;
    private int currentlySelectedSkin = 0;

    [Header("Quest Stuff")]
    [SerializeField] private TMP_Text taskDetailText;
    [SerializeField] private TMP_Text taskStatsText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimedButton;
    [SerializeField] private GameObject questCompleteIndicator;

    [Header("Stats")]
    [SerializeField] private TMP_Text totalGamesPlayedText;
    [SerializeField] private TMP_Text totalPerfectScore;
    [SerializeField] private TMP_Text totalScore;
    [SerializeField] private TMP_Text highScoreText;






    public void StatsButtonClicked()
    {
        GameManager.Instance.ButtonClickSound();
        statsMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        GameData data = SaveSystem.Load() ?? new GameData();
        totalGamesPlayedText.text = "Total-Games-Played: " + data.totalGamesPlayed;
        totalPerfectScore.text = "Total-Perfect-Score: " + data.totalPerfectScore;
        totalScore.text = "Total-Score: " + data.totalScore;
        highScoreText.text = "High-Score: " + data.highScore;
    }
    public void QuestButtonClicked()
    {
        GameManager.Instance.ButtonClickSound();
        questMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        int objectiveType = PlayerPrefs.GetInt("ObjectiveType", 0);
        int objectiveCount = PlayerPrefs.GetInt("ObjectiveCount", 0);
        int coinReward = PlayerPrefs.GetInt("CoinReward", 0);
        bool dailyQuestCompleted = PlayerPrefs.GetInt("DailyQuestCompleted", 0) == 1;
        int currentProgress = 0;
        if(objectiveType == 0)
        {
            taskDetailText.text = $"Score {objectiveCount} Hoop-ins";
            currentProgress = PlayerPrefs.GetInt("DailyHoopins", 0);
        }
        else
        {
            currentProgress = PlayerPrefs.GetInt("DailyPerfectHoopins", 0);
            taskDetailText.text = $"Score {objectiveCount} Perfect Hoop-ins";
        }
        
        taskStatsText.text = $"{Mathf.Min(currentProgress, objectiveCount)}/{objectiveCount}";
        rewardText.text = $"Reward: {coinReward}";

        if (dailyQuestCompleted)
        {
            claimButton.SetActive(false);
            rewardText.transform.parent.gameObject.SetActive(false);
            taskDetailText.transform.gameObject.SetActive(false);
            questCompleteIndicator.SetActive(true);


        }
        else if (currentProgress >= objectiveCount)
        {
            claimButton.SetActive(true);
            claimedButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            claimButton.SetActive(true);
            claimButton.GetComponent<Button>().interactable = false;
        }
    }

    public void ClaimReward()
    {
        GameManager.Instance.ButtonClickSound();
        GameData data = SaveSystem.Load() ?? new GameData();
        int coinReward = PlayerPrefs.GetInt("CoinReward", 0);
        int totalCoins = data.totalCoins;
        totalCoins += coinReward;
        SaveSystem.UpdateTotalCoins(totalCoins);
        PlayerPrefs.SetInt("DailyQuestCompleted", 1);
        claimButton.SetActive(false);
        claimedButton.SetActive(true);
        UpdateCoinsDisplay();
    }

    private void Start()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        currentlySelectedSkin = data.currentlySelectedSkinIndex;
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
        mainMenuCanvas.SetActive(true);
        shopMenuCanvas.SetActive(false);
        UpdateCoinsDisplay();
    }


    private void UpdateCoinsDisplay()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        int totalCoins = data.totalCoins;
        if (menuCoinsText != null)
        {
            menuCoinsText.text = totalCoins.ToString();
        }
    }

    public void OnShopButtonClicked()
    {
        GameManager.Instance.ButtonClickSound();
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
        GameManager.Instance.ButtonClickSound();
        if (mainMenuCanvas) mainMenuCanvas.SetActive(true);
        if (shopMenuCanvas) shopMenuCanvas.SetActive(false);
        if (codeMenuCanvas) codeMenuCanvas.SetActive(false);
        if (questMenuCanvas) questMenuCanvas.SetActive(false);
        if (statsMenuCanvas) statsMenuCanvas.SetActive(false);
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
        UpdateCoinsDisplay();
    }

    private void InitializeShop()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        //ballsUnlocked = 0;
        skinsUnlocked = data.skinsUnlocked;
        currentlySelectedSkin = data.currentlySelectedSkinIndex;
        int currentCoins = data.totalCoins;

        for (int i = 0; i < skinData.skins.Count; i++)
        {
            Transform skinContainer = skinsParent.GetChild(i);

            // Get the required child elements
            GameObject priceObj = skinContainer.Find("Price").gameObject;
            GameObject selectButton = skinContainer.Find("SelectButton").gameObject;
            GameObject buyButton = skinContainer.Find("BuyButton").gameObject;
            GameObject selectedButton = skinContainer.Find("SelectedButton").gameObject;
            GameObject ballObj = skinContainer.Find("Ball").gameObject;

            ballObj.GetComponentInChildren<Image>().sprite = skinData.skins[i].sprite;
            // Set the price text
            TMP_Text priceText = priceObj.GetComponentInChildren<TMP_Text>();
            priceText.text = skinData.skins[i].price.ToString();

            // Check if the skin is unlocked
            if (skinsUnlocked.Contains(i))
            {
                buyButton.SetActive(false);
                priceObj.SetActive(false);

                if (i == currentlySelectedSkin)
                {
                    ballObj.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
                    RectTransform ballRect = ballObj.GetComponent<RectTransform>();
                    ballRect.anchoredPosition = new Vector2(0, 15);
                    selectButton.SetActive(false);
                    selectedButton.SetActive(true);
                }
                else
                {
                    ballObj.transform.localScale = Vector3.one;
                    RectTransform ballRect = ballObj.GetComponent<RectTransform>();
                    ballRect.anchoredPosition = new Vector2(0, 25);
                    selectButton.SetActive(true);
                    selectedButton.SetActive(false);
                }
            }
            else
            {
                buyButton.SetActive(true);
                priceObj.SetActive(true);
                selectButton.SetActive(false);
                selectedButton.SetActive(false);
            }
        }


        shopCoinsText.text = currentCoins.ToString();
    }

    public void BuySkin(int skinIndex)
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        // Check if player has enough coins
        if (data.totalCoins >= skinData.skins[skinIndex].price)
        {
            purchaseSound.Stop();
            purchaseSound.Play();
            data.totalCoins -= skinData.skins[skinIndex].price;
            data.skinsUnlocked.Add(skinIndex);
            data.currentlySelectedSkinIndex = skinIndex;

            SaveSystem.Save(data);

            InitializeShop(); // Refresh the UI
        }
        else
        {
            errorSound.Stop();
            errorSound.Play();
            errorLabel.gameObject.SetActive(true);
            Vector3 originalPosition = errorLabel.GetComponent<RectTransform>().anchoredPosition;
            Vector3 to = UnityEngine.Random.insideUnitCircle * shakeStrength;
            to = to + originalPosition;
            LeanTween.move(errorLabel, to, shakeDuration).setEase(LeanTweenType.easeShake).setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    LeanTween.move(errorLabel, originalPosition, 0.2f).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true).setOnComplete(() =>
                    {
                        errorLabel.gameObject.SetActive(false);
                    });
                    
                });


        }
    }

    public void SelectSkin(int skinIndex)
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        // Check if the skin is unlocked
        if (data.skinsUnlocked.Contains(skinIndex))
        {
            selectSound.Stop();
            selectSound.Play();
            data.currentlySelectedSkinIndex = skinIndex;

            SaveSystem.Save(data);

            InitializeShop(); // Refresh the UI
        }
        else
        {
            Debug.Log("Skin is locked!");
        }
    }

    public void StartGame()
    {
        GameManager.Instance.ButtonClickSound();
        SceneManager.LoadScene(gameSceneName);
    }
}
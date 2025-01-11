// MainMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Stuff")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject shopMenuCanvas;
    [SerializeField] private GameObject codeMenuCanvas;
    [SerializeField] private GameObject questMenuCanvas;
    [SerializeField] private GameObject statsMenuCanvas;
    [SerializeField] private GameObject settingsMenuCanvas;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private Image selectedPlayerSprite;
    [SerializeField] private RectTransform globalCoinsPanel;
    [SerializeField] private TMP_Text globalCoinsText;
    [Header("Shop stuff")]
    [SerializeField] private Transform skinsParent;
    [SerializeField] private SkinDatabase skinData;

    [SerializeField] private AudioSource selectSound;
    [SerializeField] private AudioSource purchaseSound;
    [SerializeField] private AudioSource errorSound;
    [SerializeField] private AudioSource coinClaimSound; // Added AudioSource for coin claim effect
    [SerializeField] private RectTransform errorLabel;

    [SerializeField] float shakeDuration = 0.5f;
    [SerializeField] float shakeStrength = 10f;

    private List<int> skinsUnlocked;
    private int currentlySelectedSkin = 0;

    [Header("Quest Stuff")]
    [SerializeField] private GameObject claimIndicator;
    [SerializeField] private TMP_Text taskDetailText;
    [SerializeField] private TMP_Text taskStatsText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject questCompleteIndicator;
    [SerializeField] private GameObject ClockParent;
    [SerializeField] private GameObject clockHandle;

    [Header("Stats")]
    [SerializeField] private TMP_Text totalGamesPlayedText;
    [SerializeField] private TMP_Text totalPerfectScore;
    [SerializeField] private TMP_Text totalScore;
    [SerializeField] private TMP_Text highScoreText;

    [Header("Settings")]
    [SerializeField] private Slider soundToggle;

    [Header("Pooled Coins For Animation")]
    [SerializeField] private List<GameObject> pooledCoins; // 5 coins

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

    public void SettingsButtonClicked()
    {
        GameManager.Instance.ButtonClickSound();
        settingsMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        InitializeSettings();
    }


    private void InitializeSettings()
    {
        InitializeSoundToogleState();
    }

    public void QuestButtonClicked()
    {
        GameManager.Instance.ButtonClickSound();
        questMenuCanvas.SetActive(true);
        mainMenuCanvas.SetActive(false);
        InitializeQuest();
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

        // Coin animation
        if (globalCoinsPanel == null)
        {
            Debug.LogError("globalCoinsPanel is not assigned!");
        }
            LeanTween.scale(globalCoinsPanel.transform.gameObject, new Vector3(1.2f, 1.2f, 1.2f), 0.5f)
                .setEase(LeanTweenType.easeInOutSine)
                .setDelay(0.5f)
                .setIgnoreTimeScale(true);

        Vector3 claimButtonPosition = claimButton.GetComponent<RectTransform>().anchoredPosition;
        for (int i = 0; i < pooledCoins.Count; i++)
        {
            var coin = pooledCoins[i];
            coin.SetActive(true);

            // Move the coin to the position of the claimButton
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            coinRect.anchoredPosition = claimButtonPosition;

            // Add randomness to the delay
            float delay = UnityEngine.Random.Range(0, 0.2f) + i * 0.1f;
            // Animate the coin
            LeanTween.move(coinRect, new Vector3(352, 685, 0), 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setDelay(delay)
                .setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    coin.SetActive(false);
                    if (coin == pooledCoins[pooledCoins.Count - 1])
                    {
                        UpdateCoinsDisplay();
                        LeanTween.scale(globalCoinsPanel.transform.gameObject, Vector3.one, 0.3f)
                .setEase(LeanTweenType.easeInOutSine)
                .setIgnoreTimeScale(true);
                    }
                });
        }


        // Play coin claim sound effect
        coinClaimSound.PlayDelayed(0.5f);

        Debug.Log($"{globalCoinsPanel.anchoredPosition}");

        InitializeQuest();
    }

    private void InitializeQuest()
    {
        int objectiveType = PlayerPrefs.GetInt("ObjectiveType", 0); //0
        int objectiveCount = PlayerPrefs.GetInt("ObjectiveCount", 0); //28
        int coinReward = PlayerPrefs.GetInt("CoinReward", 0); //32
        bool dailyQuestCompleted = PlayerPrefs.GetInt("DailyQuestCompleted", 0) == 1; //false
        int currentProgress = 0;
        if (objectiveType == 0)
        {
            taskDetailText.text = $"Score {objectiveCount} Hoop-ins";
            currentProgress = PlayerPrefs.GetInt("DailyHoopins", 0); //1
        }
        else
        {
            currentProgress = PlayerPrefs.GetInt("DailyPerfectHoopins", 0);
            taskDetailText.text = $"Score {objectiveCount} Perfect Hoop-ins";
        }

        taskStatsText.text = $"{Mathf.Min(currentProgress, objectiveCount)}/{objectiveCount}"; // 1/28
        rewardText.text = $"Reward: {coinReward}"; // 32

        if (dailyQuestCompleted) //Completed quest and claimed.
        {
            Debug.Log("Quest already completed");
            claimButton.SetActive(false);
            rewardText.transform.parent.gameObject.SetActive(false);
            taskDetailText.transform.gameObject.SetActive(true);
            StartCoroutine(UpdateTimeRemaining());
            questCompleteIndicator.SetActive(true);
            ClockParent.SetActive(true);
            RotateClockHandle();
        }
        else if (currentProgress >= objectiveCount) //Can claim but still hasnt claimed.
        {
            Debug.Log("Quest completed");
            claimButton.SetActive(true);
            LeanTween.scale(claimButton, new Vector3(1.2f, 1.2f, 1.2f), 0.5f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong().setIgnoreTimeScale(true);
            questCompleteIndicator.SetActive(false);
            ClockParent.SetActive(false);
        }
        else //Not completed yet.
        {
            Debug.Log("Quest not completed");
            claimButton.SetActive(false);
            questCompleteIndicator.SetActive(false);
            ClockParent.SetActive(false);

        }
    }

    void RotateClockHandle()
    {
        LeanTween.rotateZ(clockHandle, clockHandle.transform.eulerAngles.z - 90, 1f)
                 .setEase(LeanTweenType.easeInExpo)
                 .setIgnoreTimeScale(true)
                 .setOnComplete(RotateClockHandle);
    }

    private IEnumerator UpdateTimeRemaining()
    {
        while (true)
        {
            TimeSpan timeRemaining = GetTimeRemainingForNextQuest();
            taskDetailText.text = $"Next quest in: {timeRemaining.Hours}h {timeRemaining.Minutes}m";
            yield return new WaitForSeconds(60);
        }
    }

    private void Start()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        currentlySelectedSkin = data.currentlySelectedSkinIndex;
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
        mainMenuCanvas.SetActive(true);
        shopMenuCanvas.SetActive(false);
        codeMenuCanvas.SetActive(false);
        questMenuCanvas.SetActive(false);
        statsMenuCanvas.SetActive(false);
        settingsMenuCanvas.SetActive(false);
        UpdateCoinsDisplay();
        ManageClaimIndicatorState();
    }

    private void ManageClaimIndicatorState()
    {
        int objectiveType = PlayerPrefs.GetInt("ObjectiveType", 0);
        int objectiveCount = PlayerPrefs.GetInt("ObjectiveCount", 0);
        int currentProgress = objectiveType == 0 ? PlayerPrefs.GetInt("DailyHoopins", 0) : PlayerPrefs.GetInt("DailyPerfectHoopins", 0);
        if (PlayerPrefs.GetInt("DailyQuestCompleted", 0) == 0 && currentProgress >= objectiveCount)
        {
            claimIndicator.SetActive(true);
            LeanTween.scale(claimIndicator, new Vector3(1.2f, 1.2f, 1.2f), 0.5f).setEase(LeanTweenType.easeInOutSine).setLoopPingPong().setIgnoreTimeScale(true);
        }
        else
        {
            claimIndicator.SetActive(false);
        }
    }

    private void UpdateCoinsDisplay()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        int totalCoins = data.totalCoins;
        if (globalCoinsText != null)
        {
            globalCoinsText.text = totalCoins.ToString();
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
            SaveSystem.UpdateTotalScore(totalScore += 1000);
        }
        else if (text == "POOP-1000")
        {
            SaveSystem.UpdateTotalScore(totalScore -= 1000);
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
        if (settingsMenuCanvas) settingsMenuCanvas.SetActive(false);

        ManageClaimIndicatorState();
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
        UpdateCoinsDisplay();
    }

    private void InitializeShop()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
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

        globalCoinsText.text = currentCoins.ToString();
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
    private TimeSpan GetTimeRemainingForNextQuest()
    {
        DateTime nextQuestTime = DateTime.Today.AddDays(1); // Assuming the next quest is available the next day
        return nextQuestTime - DateTime.Now;
    }

    private void InitializeSoundToogleState()
    {
        soundToggle.value = PlayerPrefs.GetFloat("Sound", 1);
    }

    public void OnSoundToogleChanged(Single volume)
    {
        PlayerPrefs.SetFloat("Sound", volume);
        AudioListener.volume = volume;
    }
}

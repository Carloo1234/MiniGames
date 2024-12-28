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
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject shopMenuCanvas;
    [SerializeField] private GameObject codeMenuCanvas;
    [SerializeField] private TMP_InputField codeText;
    [SerializeField] private Image selectedPlayerSprite;
    [Header("Shop stuff")]
    [SerializeField] private Transform skinsParent;
    [SerializeField] private TMP_Text shopCoinsText;
    [SerializeField] private SkinDatabase skinData;


    private List<int> skinsUnlocked;
    private int currentlySelectedSkin = 0;

    private void Start()
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        currentlySelectedSkin = data.currentlySelectedSkinIndex;
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
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
        selectedPlayerSprite.sprite = skinData.skins[currentlySelectedSkin].sprite;
        UpdateHighScoreDisplay();
        UpdateTotalScoreDisplay();
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
                    selectButton.SetActive(false);
                    selectedButton.SetActive(true);
                }
                else
                {
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
            data.totalCoins -= skinData.skins[skinIndex].price;
            data.skinsUnlocked.Add(skinIndex);
            data.currentlySelectedSkinIndex = skinIndex;

            SaveSystem.Save(data);

            InitializeShop(); // Refresh the UI
        }
        else
        {
            Debug.Log("Not enough coins to buy this skin!");
        }
    }

    public void SelectSkin(int skinIndex)
    {
        GameData data = SaveSystem.Load() ?? new GameData();
        // Check if the skin is unlocked
        if (data.skinsUnlocked.Contains(skinIndex))
        {
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
        SceneManager.LoadScene(gameSceneName);
    }
}
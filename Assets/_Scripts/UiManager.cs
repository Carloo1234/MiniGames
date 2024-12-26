// UiManager.cs
using UnityEngine;
using TMPro;
using System.Collections;
using System.Xml.Linq;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    [Header("Text References")]
    [SerializeField] private TMP_Text gameScoreText;
    [SerializeField] private TMP_Text gameCoinsText;
    [SerializeField] private TMP_Text pauseScoreText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text gameOverPerfectScoreText;
    [SerializeField] private TMP_Text comboText;

    [Header("Other UI Elements")]
    [SerializeField] private AudioSource newHighScoreSound;
    [SerializeField] private GameObject gameOverNewHighscoreObject;
    [SerializeField] private GameObject gameOverMenuButtonObject;
    [SerializeField] private GameObject gameOverRestartButtonObject;
    [SerializeField] private GameObject gameOverScoreObject;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject menuButtonObject;
    [SerializeField] private GameObject resumeButtonObject;
    [SerializeField] private GameObject pauseScoreObject;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseObject;
    [SerializeField] private GameObject comboObject;
    [SerializeField] private GameObject coinPanel;
    [SerializeField] private GameObject gameScoreObject;
    [SerializeField] private GameObject highScoreIndicator;
    [SerializeField] private GameObject perfectText;
    [SerializeField] private Animator perfectTextAnim;

    [Header("Attributes")]
    [SerializeField] Vector3 perfectTextOffset;
    [Header("Animations")]    
    [SerializeField] Vector3 coinPanelPopScale;
    [SerializeField] Vector3 scorePopScale;

    [Header("Clicked Buttons")]
    [SerializeField] Sprite clickedResume;
    public void ShowComboText(int multiplier)
    {
        if (comboText) 
        {
            comboText.enabled = true;
            comboText.text = "Combo\n" + multiplier.ToString() + "x";
            comboText.rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
            comboObject.transform.localScale = Vector3.zero;
            LeanTween.scale(comboObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeOutElastic);
        }
    }
    public void ShowPerfectText(Vector3 position)
    {
        if (perfectText)
        {
            perfectText.SetActive(true);
            perfectText.transform.position = position + perfectTextOffset;
            perfectText.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-20, 21));
            float randomScale = Random.Range(0.15f, 0.265f);
            perfectText.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
            perfectTextAnim.Play("PerfectAnimation");
            StartCoroutine(FadeOutPerfectText());
        }
    }

    public IEnumerator FadeOutPerfectText()
    {
        yield return new WaitForSecondsRealtime(1);
        if (perfectText) perfectText.SetActive(false);
    }

    public void HideComboText()
    {
        if (comboText) comboText.enabled = false;
    }

    public void UpdateScoreUI(int score)
    {
        if (gameScoreText) gameScoreText.text = score.ToString();
        if (gameScoreObject) LeanTween.scale(gameScoreObject, scorePopScale, 0.1f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => 
                {
                    LeanTween.scale(gameScoreObject, Vector3.one, 0.1f)
                                  .setEase(LeanTweenType.easeInQuad);
                });
    }
    
    public void UpdateCoinsUI(int coins)
    {
        if (gameCoinsText) gameCoinsText.text = coins.ToString();
        if (coinPanel) LeanTween.scale(coinPanel, coinPanelPopScale, 0.1f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() => 
                {
                    LeanTween.scale(coinPanel, Vector3.one, 0.1f)
                                  .setEase(LeanTweenType.easeInQuad);
                });
    }

    public void ShowGameUI()
    {

        if (gameCanvas) gameCanvas.SetActive(true);
        //Set up scale for animations
        pauseObject.transform.localScale = Vector3.zero;
        gameScoreObject.transform.localScale = Vector3.zero;
        coinPanel.transform.localScale = Vector3.zero;
        if (pauseObject) LeanTween.scale(pauseObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic);
        if (gameScoreObject) LeanTween.scale(gameScoreObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic).setDelay(0.1f);
        if (coinPanel) LeanTween.scale(coinPanel, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutElastic).setDelay(0.2f);
        HideComboText();
    }

    public void ShowPauseUI(int currentScore)
    {

        if (gameCanvas) gameCanvas.SetActive(false);
        if (pauseCanvas)
        {
            pauseCanvas.SetActive(true);
            if (pauseScoreText) pauseScoreText.text = currentScore.ToString();
            // Set initial positions
            pausePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -800, 0);
            resumeButtonObject.transform.localScale = Vector3.zero;
            menuButtonObject.transform.localScale = Vector3.zero;

            // Animate to the desired Y position
            LeanTween.moveY(pausePanel.GetComponent<RectTransform>(), 0, 0.5f)
                     .setEase(LeanTweenType.easeInOutQuint).setIgnoreTimeScale(true);
            LeanTween.scale(resumeButtonObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(0.5f).setIgnoreTimeScale(true);
            LeanTween.scale(menuButtonObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(0.6f).setIgnoreTimeScale(true);

        }
    }

    public void HidePauseUI()
    {
        LeanTween.scale(resumeButtonObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setIgnoreTimeScale(true);
        LeanTween.scale(menuButtonObject, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInBack).setDelay(0.1f).setIgnoreTimeScale(true);
        LeanTween.moveY(pausePanel.GetComponent<RectTransform>(), -800f, 0.5f)
                     .setEase(LeanTweenType.easeInOutQuint).setIgnoreTimeScale(true).setDelay(0.6f).setOnComplete(() =>
                     {
                         pauseCanvas.SetActive(false);
                         Time.timeScale = 1;
                         ShowGameUI();
                     }
                     );
    }

    public void ShowGameOverUI(int finalScore, bool isNewHighScore, int finalPerfectScore)
    {
        if (gameCanvas) gameCanvas.SetActive(false);
        
        if (pauseCanvas) pauseCanvas.SetActive(false);
        if (gameOverCanvas)
        {
            gameOverCanvas.SetActive(true);
            // Set initial positions
            gameOverPanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -800, 0);
            gameOverRestartButtonObject.transform.localScale = Vector3.zero;
            gameOverMenuButtonObject.transform.localScale = Vector3.zero;

            // Animate to the desired Y position
            LeanTween.moveY(gameOverPanel.GetComponent<RectTransform>(), 0, 0.5f) // Target Y = 0 (centered)
                     .setEase(LeanTweenType.easeInOutQuint).setIgnoreTimeScale(true);

            LeanTween.scale(gameOverRestartButtonObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(0.5f).setIgnoreTimeScale(true);
            LeanTween.scale(gameOverMenuButtonObject, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack).setDelay(0.6f).setIgnoreTimeScale(true);
            if (gameOverScoreText) gameOverScoreText.text = finalScore.ToString();
            if (gameOverPerfectScoreText) gameOverPerfectScoreText.text = finalPerfectScore.ToString();
            if (true)
            {
                highScoreIndicator.SetActive(true);
                gameOverNewHighscoreObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                gameOverNewHighscoreObject.transform.localScale = Vector3.zero;
                LeanTween.scale(gameOverNewHighscoreObject, new Vector3(1.5f, 1.5f, 1.5f), 0.8f).setEase(LeanTweenType.easeOutBack).setDelay(1.2f).setIgnoreTimeScale(true)
                    .setOnStart(() => { newHighScoreSound.Play(); })
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(gameOverNewHighscoreObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeInCirc).setIgnoreTimeScale(true).setDelay(1f);
                        LeanTween.move(gameOverNewHighscoreObject.GetComponent<RectTransform>(), new Vector3(170, 540, 0), 0.4f).setEase(LeanTweenType.easeInCirc).setIgnoreTimeScale(true).setDelay(1f);
                    });
            }
        }
    }

    public void HideAllUI()
    {
        if (gameCanvas) gameCanvas.SetActive(false);
        if (pauseCanvas) pauseCanvas.SetActive(false);
        if (gameOverCanvas) gameOverCanvas.SetActive(false);
        if (perfectText) perfectText.SetActive(false);
    }

    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null!");
        }
    }
    
    public void OnPauseButtonClicked()
    {
        GameManager.Instance.PauseGame();
    }

    public void OnResumeButtonClicked()
    {
        //image.sprite = clickedResume;
        HidePauseUI();
    }

    public void OnMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
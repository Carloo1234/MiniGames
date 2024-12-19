// UiManager.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class UiManager : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    [Header("Text References")]
    [SerializeField] private TMP_Text gameScoreText;
    [SerializeField] private TMP_Text pauseScoreText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text comboText;

    [Header("Other UI Elements")]
    [SerializeField] private GameObject highScoreIndicator;
    [SerializeField] private GameObject perfectText;
    [SerializeField] private Animator perfectTextAnim;

    [Header("Attributes")]
    [SerializeField] Vector3 perfectTextOffset;

    public void ShowComboText(int multiplier)
    {
        if (comboText) 
        {
            comboText.enabled = true;
            comboText.text = "Combo\n" + multiplier.ToString() + "x";
            comboText.rectTransform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
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
    }

    public void ShowGameUI()
    {
        if (gameCanvas) gameCanvas.SetActive(true);
        HideComboText();
    }

    public void ShowPauseUI(int currentScore)
    {
        if (gameCanvas) gameCanvas.SetActive(false);
        if (pauseCanvas)
        {
            pauseCanvas.SetActive(true);
            if (pauseScoreText) pauseScoreText.text = currentScore.ToString();
        }
    }

    public void HidePauseUI()
    {
        if (pauseCanvas) pauseCanvas.SetActive(false);
        if (gameCanvas) gameCanvas.SetActive(true);
    }

    public void ShowGameOverUI(int finalScore, bool isNewHighScore)
    {
        if (gameCanvas) gameCanvas.SetActive(false);
        if (pauseCanvas) pauseCanvas.SetActive(false);
        if (gameOverCanvas)
        {
            gameOverCanvas.SetActive(true);
            if (gameOverScoreText) gameOverScoreText.text = finalScore.ToString();
            if (highScoreIndicator) highScoreIndicator.SetActive(isNewHighScore);
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
        GameManager.Instance.ResumeGame();
    }

    public void OnMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalScore = 0;
    public int roundScore = 0;
    private int highScore;

    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI roundScoreText;
    public TextMeshProUGUI inGameRoundScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI killCountText;

    public GameObject gameOverCanvas;
    public int comboCount = 1;
    public int maxCombo = 2;
    public int killCount = 0;

    public EnemySpawner enemySpawner;
    public Volume postProcessingVolume;

    private readonly Dictionary<TextMeshProUGUI, Coroutine> runningCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();

    private void Awake()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        maxCombo = PlayerPrefs.GetInt("MaxCombo", 2);
        UpdateScoreUI();
        UpdateComboUI();

        if (inGameRoundScoreText != null)
            inGameRoundScoreText.transform.localScale = Vector3.one * 2;

        LoadGraphicsSettings();
    }

    private void LoadGraphicsSettings()
    {
        int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 1);
        if (postProcessingVolume != null)
            postProcessingVolume.enabled = graphicsQuality == 1;
    }

    private void UpdateScoreUI()
    {
        if (totalScoreText != null)
            totalScoreText.text = totalScore.ToString();

        if (roundScoreText != null)
        {
            roundScoreText.text = "+" + roundScore;
            StartEnhancedImpactEffect(roundScoreText);
        }

        if (inGameRoundScoreText != null)
        {
            inGameRoundScoreText.text = roundScore.ToString();
            StartEnhancedImpactEffect(inGameRoundScoreText);
        }
    }

    public void AddScore(int points)
    {
        int scoreToAdd = points * comboCount;
        roundScore += scoreToAdd;
        totalScore += scoreToAdd;
        UpdateScoreUI();
    }

    public void IncreaseCombo()
    {
        if (comboCount < maxCombo)
            comboCount++;
        UpdateComboUI();
    }

    public void ResetCombo()
    {
        comboCount = 1;
        UpdateComboUI();
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = "X" + comboCount;
            StartEnhancedImpactEffect(comboText);
        }
    }

    private void StartEnhancedImpactEffect(TextMeshProUGUI text)
    {
        if (runningCoroutines.TryGetValue(text, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            runningCoroutines.Remove(text);
            text.transform.localScale = Vector3.one;
        }

        Coroutine newCoroutine = StartCoroutine(EnhancedImpactEffect(text));
        runningCoroutines[text] = newCoroutine;
    }

    private IEnumerator EnhancedImpactEffect(TextMeshProUGUI text)
    {
        Vector3 originalScale = text.transform.localScale;
        Vector3 enlargedScale = originalScale * 1.4f;
        float duration = 0.05f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            text.transform.localScale = Vector3.Lerp(originalScale, enlargedScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        text.transform.localScale = enlargedScale;

        elapsedTime = 0f;
        Vector3 slightlyReducedScale = originalScale * 1.2f;
        while (elapsedTime < duration)
        {
            text.transform.localScale = Vector3.Lerp(enlargedScale, slightlyReducedScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        text.transform.localScale = slightlyReducedScale;

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            text.transform.localScale = Vector3.Lerp(slightlyReducedScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        text.transform.localScale = originalScale;
        runningCoroutines.Remove(text);
    }

    public void PlayerDied()
    {
        UpdateHighScore();

        PlayerPrefs.SetInt("TotalScore", PlayerPrefs.GetInt("TotalScore", 0) + roundScore);
        PlayerPrefs.Save();

        gameOverCanvas.SetActive(true);

        if (killCountText != null)
            killCountText.text = killCount.ToString();

        if (enemySpawner != null)
            enemySpawner.enabled = false;

        Time.timeScale = 0.25f;
    }

    private void UpdateHighScore()
    {
        if (roundScore > highScore)
        {
            highScore = roundScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }

    public void PlayAgain()
    {
        roundScore = 0;
        killCount = 0;
        comboCount = 1;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoBackToMainMenu()
    {
        roundScore = 0;
        killCount = 0;
        comboCount = 1;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}

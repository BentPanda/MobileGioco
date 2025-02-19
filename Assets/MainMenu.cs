using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;
    public GameObject customizeCanvas;
    public GameObject shopCanvas;

    [Header("Graphics Settings")]
    public Volume postProcessingVolume;
    public TextMeshProUGUI fastGraphicsText;
    public TextMeshProUGUI goodGraphicsText;

    [Header("Score Display")]
    public TextMeshProUGUI totalScoreText;

    [Header("Color Customization")]
    public List<ColorOption> colorOptions;

    private Color activeTextColor = new Color(1, 1, 1, 1);
    private Color inactiveTextColor = new Color(1, 1, 1, 0.5f);
    private int totalScore;

    private void Start()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        customizeCanvas.SetActive(false);
        shopCanvas.SetActive(false);

        LoadGraphicsSettings();
        LoadTotalScore();
        LoadColorOptions();
        SetupColorOptionEventListeners();
        ResetBoxSpriteAlphas();
        UpdateGraphicsButtons();
        UpdateColorSelection();
    }

    private void ResetBoxSpriteAlphas()
    {
        foreach (ColorOption option in colorOptions)
        {
            if (option.box != null)
            {
                foreach (SpriteRenderer sprite in option.box.GetComponentsInChildren<SpriteRenderer>())
                {
                    Color spriteColor = sprite.color;
                    sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0.5f);
                }
            }
        }
    }

    private void UpdateColorSelection()
    {
        string selectedColorName = LoadPlayerColor();

        foreach (ColorOption option in colorOptions)
        {
            if (option.isUnlocked)
            {
                option.lockOverlay.SetActive(false);
                option.costText.gameObject.SetActive(false);
            }
            else
            {
                option.lockOverlay.SetActive(true);
                option.costText.gameObject.SetActive(true);
                option.costText.text = option.cost.ToString();
            }

            bool isSelected = option.isUnlocked && (selectedColorName == option.colorName);
            SetColorOptionAppearance(option, isSelected);
        }

        if (totalScoreText != null)
            totalScoreText.text = "Score: " + totalScore;
    }

    private void SetColorOptionAppearance(ColorOption option, bool isSelected)
    {
        option.colorText.color = isSelected ? activeTextColor : inactiveTextColor;

        if (option.box != null)
        {
            float alpha = isSelected ? 1f : 0.5f;
            foreach (SpriteRenderer sprite in option.box.GetComponentsInChildren<SpriteRenderer>())
            {
                Color spriteColor = sprite.color;
                sprite.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
            }
        }
    }

    public void OnColorOptionClicked(ColorOption option)
    {
        if (option.isUnlocked)
        {
            SavePlayerColor(option.colorName);
            UpdateColorSelection();
        }
        else
        {
            PurchaseColor(option);
        }
    }

    private void PurchaseColor(ColorOption option)
    {
        if (totalScore >= option.cost)
        {
            totalScore -= option.cost;
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.Save();

            option.isUnlocked = true;
            PlayerPrefs.SetInt(option.colorName + "_Unlocked", 1);
            PlayerPrefs.Save();

            SavePlayerColor(option.colorName);
            UpdateColorSelection();

            Debug.Log(option.colorName + " color unlocked!");
        }
        else
        {
            Debug.Log("Not enough score to purchase " + option.colorName + " color.");
        }
    }

    private void LoadTotalScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    private void LoadColorOptions()
    {
        foreach (ColorOption option in colorOptions)
            option.isUnlocked = PlayerPrefs.GetInt(option.colorName + "_Unlocked", 0) == 1;
    }

    private void SetupColorOptionEventListeners()
    {
        foreach (ColorOption option in colorOptions)
        {
            option.colorButton.onClick.RemoveAllListeners();
            option.colorButton.onClick.AddListener(() => OnColorOptionClicked(option));
        }
    }

    private void SavePlayerColor(string colorName)
    {
        PlayerPrefs.SetString("SelectedColor", colorName);
        PlayerPrefs.Save();
    }

    private string LoadPlayerColor()
    {
        return PlayerPrefs.GetString("SelectedColor", "");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSettings()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void OpenCustomizeMenu()
    {
        mainMenuCanvas.SetActive(false);
        customizeCanvas.SetActive(true);
    }

    public void OpenShopMenu()
    {
        mainMenuCanvas.SetActive(false);
        shopCanvas.SetActive(true);
    }

    public void SetFastGraphics()
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.enabled = false;
            PlayerPrefs.SetInt("GraphicsQuality", 0);
            PlayerPrefs.Save();
            Debug.Log("Fast graphics: Post-processing disabled.");
            UpdateGraphicsButtons();
        }
    }

    public void SetGoodGraphics()
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.enabled = true;
            PlayerPrefs.SetInt("GraphicsQuality", 1);
            PlayerPrefs.Save();
            Debug.Log("Good graphics: Post-processing enabled.");
            UpdateGraphicsButtons();
        }
    }

    private void UpdateGraphicsButtons()
    {
        int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 1);

        if (graphicsQuality == 1)
        {
            goodGraphicsText.color = activeTextColor;
            fastGraphicsText.color = inactiveTextColor;
        }
        else
        {
            fastGraphicsText.color = activeTextColor;
            goodGraphicsText.color = inactiveTextColor;
        }
    }

    public void BackToMainMenuFromSettings()
    {
        settingsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void BackToMainMenuFromCustomize()
    {
        customizeCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void BackToMainMenuFromShop()
    {
        shopCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    private void LoadGraphicsSettings()
    {
        int graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 1);
        if (postProcessingVolume != null)
            postProcessingVolume.enabled = graphicsQuality == 1;
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_ANDROID
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
#endif
    }

    public void ResetPlayerData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Player data has been reset.");
    }
}

[System.Serializable]
public class ColorOption
{
    public string colorName;
    public Color colorValue;
    public int cost;
    public bool isUnlocked;
    public Button colorButton;
    public TextMeshProUGUI colorText;
    public GameObject lockOverlay;
    public TextMeshProUGUI costText;
    public GameObject box;
}

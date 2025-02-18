using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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

    private Color activeTextColor = new Color(1, 1, 1, 1);      // Pe³ny color jak jest wybrane
    private Color inactiveTextColor = new Color(1, 1, 1, 0.5f); // Niepe³ny jak nie jest wybrane

    private int totalScore;

    void Start()
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

    // Zmiana kolorów z customize Canvas
    private void UpdateColorSelection()
    {
        string selectedColorName = LoadPlayerColor();

        foreach (ColorOption option in colorOptions)
        {
            // Sprawdzanie czy kolor jest odblokowany
            if (option.isUnlocked)
            {
                option.lockOverlay.SetActive(false);
                option.costText.gameObject.SetActive(false);
                option.colorButton.interactable = true;
            }
            else
            {
                option.lockOverlay.SetActive(true);
                option.costText.gameObject.SetActive(true);
                option.costText.text = option.cost.ToString();
                option.colorButton.interactable = true;
            }

            // zmienny wygl¹d zale¿ny od tego czy kolor jest odblokowany
            if (option.isUnlocked && selectedColorName == option.colorName)
            {
                SetColorOptionAppearance(option, true);
            }
            else
            {
                SetColorOptionAppearance(option, false);
            }
        }

        if (totalScoreText != null)
        {
            totalScoreText.text = "Score: " + totalScore;
        }
    }

    private void SetColorOptionAppearance(ColorOption option, bool isSelected)
    {
        // ustawienie koloru tektsu
        option.colorText.color = isSelected ? activeTextColor : inactiveTextColor;

        // Zmiana alfy
        if (option.box != null)
        {
            float alpha = isSelected ? 1f : 0.5f; // alfa 1 jesli wybrane, 0.5 jesli niewybrane

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
            // Próba kupienia koloru
            PurchaseColor(option);
        }
    }

    private void PurchaseColor(ColorOption option)
    {
        if (totalScore >= option.cost)
        {
            // odj¹æ cene
            totalScore -= option.cost;
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.Save();

            // odblokowanie koloru
            option.isUnlocked = true;
            PlayerPrefs.SetInt(option.colorName + "_Unlocked", 1);
            PlayerPrefs.Save();

            SavePlayerColor(option.colorName);

            // aktualizuj Ui
            UpdateColorSelection();

            Debug.Log(option.colorName + " color unlocked!");
        }
        else
        {
            Debug.Log("Not enough score to purchase " + option.colorName + " color.");
            // Tutaj moge zamiast tego daæ wiadomoœæ w samej grze jak coœ, ale nie mam ochoty bo po co :D
        }
    }

    private void LoadTotalScore()
    {
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);
    }

    private void LoadColorOptions()
    {
        foreach (ColorOption option in colorOptions)
        {
            option.isUnlocked = PlayerPrefs.GetInt(option.colorName + "_Unlocked", 0) == 1;
        }
    }

    private void SetupColorOptionEventListeners()
    {
        foreach (ColorOption option in colorOptions)
        {
            option.colorButton.onClick.RemoveAllListeners();

            ColorOption currentOption = option;

            option.colorButton.onClick.AddListener(() => OnColorOptionClicked(currentOption));
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
        {
            postProcessingVolume.enabled = graphicsQuality == 1;
        }
    }

    public void ExitGame()
    {
        // zamykanie gry
        Application.Quit();

#if UNITY_ANDROID
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
#endif
    }

    // usuniêcie danych
    public void ResetPlayerData()
    {
        // usuniêcie danych z playerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // za³adowanie ponownie sceny by zmiany siê pokaza³y
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
    public GameObject lockOverlay; // zdjêcie które pokazuje ¿e color jest zablokowany (nie kupiony)
    public TextMeshProUGUI costText;
    public GameObject box;
}

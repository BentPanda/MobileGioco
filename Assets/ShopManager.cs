using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboButtonText;
    public TextMeshProUGUI healthButtonText;

    public Button comboUpgradeButton;
    public Button healthUpgradeButton;
    public List<Image> comboUpgradeIndicators;
    public List<Image> healthUpgradeIndicators;
    public Sprite filledUpgradeSprite;
    public Sprite emptyUpgradeSprite;

    public Button hardEnemyButton;
    public Button bulletEnemyButton;

    public TextMeshProUGUI hardEnemyButtonText;
    public TextMeshProUGUI bulletEnemyButtonText;

    private bool hardEnemyPurchased;
    private bool bulletEnemyPurchased;

    private int hardEnemyCost = 2000;
    private int bulletEnemyCost = 3000; // cena

    private int comboUpgradeLevel;
    private int healthUpgradeLevel;
    private int maxComboUpgrades = 8;
    private int maxHealthUpgrades = 8;
    private int totalScore;
    private int upgradeCost = 1000;

    public Image hardEnemyIndicator;
    public Image bulletEnemyIndicator;

    private void Start()
    {
        // Load upgrade levels and total score from PlayerPrefs
        comboUpgradeLevel = PlayerPrefs.GetInt("ComboUpgradeLevel", 0);
        healthUpgradeLevel = PlayerPrefs.GetInt("HealthUpgradeLevel", 0);
        totalScore = PlayerPrefs.GetInt("TotalScore", 0);

        // Set MaxHP and MaxCombo based on upgrade levels
        PlayerPrefs.SetInt("MaxHP", 2 + healthUpgradeLevel);
        PlayerPrefs.SetInt("MaxCombo", 2 + comboUpgradeLevel);
        PlayerPrefs.Save();

        // Load enemy purchase states with correct keys
        hardEnemyPurchased = PlayerPrefs.GetInt("HardEnemyPurchased", 0) == 1;
        bulletEnemyPurchased = PlayerPrefs.GetInt("BulletEnemyPurchased", 0) == 1;

        UpdateShopUI();
    }

    private void UpdateShopUI()
    {
        scoreText.text = "$" + totalScore;

        UpdateUpgradeIndicators(comboUpgradeIndicators, comboUpgradeLevel);
        UpdateUpgradeIndicators(healthUpgradeIndicators, healthUpgradeLevel);

        if (comboUpgradeLevel < maxComboUpgrades)
        {
            comboButtonText.text = "Combo Upgrade: " + upgradeCost;
            comboUpgradeButton.interactable = totalScore >= upgradeCost;
        }
        else
        {
            comboButtonText.text = "SOLD OUT";
            comboUpgradeButton.interactable = false;
        }

        if (healthUpgradeLevel < maxHealthUpgrades)
        {
            healthButtonText.text = "Health Upgrade: " + upgradeCost;
            healthUpgradeButton.interactable = totalScore >= upgradeCost;
        }
        else
        {
            healthButtonText.text = "SOLD OUT";
            healthUpgradeButton.interactable = false;
        }

        // Update Hard Enemy Purchase Button
        if (!hardEnemyPurchased)
        {
            hardEnemyButtonText.text = "Unlock Hard Enemy: " + hardEnemyCost;
            hardEnemyButton.interactable = totalScore >= hardEnemyCost;
        }
        else
        {
            hardEnemyButtonText.text = "Hard Enemy Unlocked";
            hardEnemyButton.interactable = false;
        }

        // Update Bullet Enemy Purchase Button
        if (!bulletEnemyPurchased)
        {
            bulletEnemyButtonText.text = "Unlock Bullet Enemy: " + bulletEnemyCost;
            bulletEnemyButton.interactable = totalScore >= bulletEnemyCost;
        }
        else
        {
            bulletEnemyButtonText.text = "Bullet Enemy Unlocked";
            bulletEnemyButton.interactable = false;
        }

        // Update enemy indicators
        UpdateEnemyIndicator(hardEnemyIndicator, hardEnemyPurchased);
        UpdateEnemyIndicator(bulletEnemyIndicator, bulletEnemyPurchased);
    }

    private void UpdateUpgradeIndicators(List<Image> indicators, int level)
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            indicators[i].sprite = i < level ? filledUpgradeSprite : emptyUpgradeSprite;
        }
    }

    private void UpdateEnemyIndicator(Image indicator, bool purchased)
    {
        if (indicator != null)
        {
            indicator.sprite = purchased ? filledUpgradeSprite : emptyUpgradeSprite;
        }
    }

    public void BuyComboUpgrade()
    {
        if (comboUpgradeLevel < maxComboUpgrades && totalScore >= upgradeCost)
        {
            comboUpgradeLevel++;
            totalScore -= upgradeCost;

            // Save combo upgrade level and score
            PlayerPrefs.SetInt("ComboUpgradeLevel", comboUpgradeLevel);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.SetInt("MaxCombo", 2 + comboUpgradeLevel);
            PlayerPrefs.Save();

            UpdateShopUI();
        }
    }

    public void BuyHealthUpgrade()
    {
        if (healthUpgradeLevel < maxHealthUpgrades && totalScore >= upgradeCost)
        {
            healthUpgradeLevel++;
            totalScore -= upgradeCost;

            // Save health upgrade level and score
            PlayerPrefs.SetInt("HealthUpgradeLevel", healthUpgradeLevel);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.SetInt("MaxHP", 2 + healthUpgradeLevel);
            PlayerPrefs.Save();

            UpdateShopUI();
        }
    }

    public void BuyHardEnemy()
    {
        if (!hardEnemyPurchased && totalScore >= hardEnemyCost)
        {
            hardEnemyPurchased = true;
            totalScore -= hardEnemyCost;

            // Save purchase state with correct key
            PlayerPrefs.SetInt("HardEnemyPurchased", 1);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.Save();

            UpdateShopUI();
        }
    }

    public void BuyBulletEnemy()
    {
        if (!bulletEnemyPurchased && totalScore >= bulletEnemyCost)
        {
            bulletEnemyPurchased = true;
            totalScore -= bulletEnemyCost;

            // Save purchase state with correct key
            PlayerPrefs.SetInt("BulletEnemyPurchased", 1);
            PlayerPrefs.SetInt("TotalScore", totalScore);
            PlayerPrefs.Save();

            UpdateShopUI();
        }
    }
}

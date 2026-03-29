using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI quotaText;
    public TextMeshProUGUI statsText;
    public Button nextDayButton;
    public Button[] resistanceUpgrades, pickaxeUpgrades, speedUpgrades;
    public Button helmetUpgrade, betterPickaxeUpgrade, promotion;
    
    public int[] resistanceUpgradeValues = { 30, 45, 60, 90, 135 };
    public int[] pickaxeUpgradeValues = { 1, 1, 1 };
    public float[] speedUpgradeValues = { 0.5f, 0.8f, 1.2f, 1.8f, 2.7f };
    public int[] resistanceAndSpeedCosts = { 30, 75, 160, 350, 750 };
    public int[] pickaxeCosts = { 50, 150, 450 };

    
    // ---- THIS THING IS MISSING THE SHOPKEEPER'S DIALOG, AND IT'S SYSTEM, IDEALLY IN A SEPARATE SCRIPT ---- //
    
    
    private void Start()
    {
        nextDayButton.onClick.AddListener(OnNextDayClicked);
        
        foreach (var btn in resistanceUpgrades) btn.onClick.AddListener(UpgradeResistance);
        foreach (var btn in pickaxeUpgrades) btn.onClick.AddListener(UpgradePickaxe);
        foreach (var btn in speedUpgrades) btn.onClick.AddListener(UpgradeSpeed);
        //Need to add in Listeners to special upgrades and also write their functions
        
        // Helper function because why run it on update?
        RefreshUI();
        SetupShopButtons();
    }

    private void RefreshUI()
    {
        dayText.text = $"Day {GameManager.Instance.CurrentDay}";
        quotaText.text = $"This day's Quota: ${GameManager.Instance.CurrentQuota}";
        statsText.text = $"Resistance: \n{GameManager.Instance.playerResistance}\nPickaxe Quality: \n{GameManager.Instance.playerPickaxeQuality}\nMovement Speed: \n{GameManager.Instance.playerMovementSpeed}\nCoins in your bank: \n${GameManager.Instance.playerBank}";
    }

    private void SetupShopButtons()
    {
        for (int i = 0; i < resistanceUpgrades.Length; i++)
        {
            // if it's bought, disable the button
            if (GameManager.Instance.unlockedUpgrades.resistanceUpgrades[i])
            {
                resistanceUpgrades[i].interactable = false;
            }
            // if it isn't, but the previous one isn't bought either, also disable
            else if (i > 0 && !GameManager.Instance.unlockedUpgrades.resistanceUpgrades[i - 1])
            {
                resistanceUpgrades[i].interactable = false;
            }
            // otherwise, this means this upgrade isn't bought but the previous one is, therefore this one should be available
            else
            {
                resistanceUpgrades[i].interactable = true;
            }
        }

        // same as resistance, won't bother to explain again
        for (int i = 0; i < pickaxeUpgrades.Length; i++)
        {
            if (GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i] || i > 0 && !GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i - 1])
                pickaxeUpgrades[i].interactable = false;
            else
                pickaxeUpgrades[i].interactable = true;
        }

        for (int i = 0; i < speedUpgrades.Length; i++)
        {
            if (GameManager.Instance.unlockedUpgrades.speedUpgrades[i] || i > 0 && !GameManager.Instance.unlockedUpgrades.speedUpgrades[i - 1])
                speedUpgrades[i].interactable = false;
            else
                speedUpgrades[i].interactable = true;
        }
    }

    private void OnNextDayClicked()
    {
        nextDayButton.interactable = false;
        GameManager.Instance.EnterMinesFromLobby();
    }

    private void UpgradeResistance()
    {
        for (int i = 0; i < resistanceUpgrades.Length; i++)
        {
            if (GameManager.Instance.unlockedUpgrades.resistanceUpgrades[i]) continue;
            if (GameManager.Instance.playerBank >= resistanceAndSpeedCosts[i])
            {
                GameManager.Instance.playerBank -= resistanceAndSpeedCosts[i];
                GameManager.Instance.playerResistance += resistanceUpgradeValues[i];
                GameManager.Instance.unlockedUpgrades.resistanceUpgrades[i] = true;
                    
                resistanceUpgrades[i].interactable = false; 
                    
                // FIX: Ensure we don't go out of bounds on the final upgrade
                if (i + 1 < resistanceUpgrades.Length)
                {
                    resistanceUpgrades[i + 1].interactable = true; 
                }
                    
                RefreshUI(); // Instantly update the text on screen!
                break; 
            }
        }
    }
    
    private void UpgradePickaxe()
    {
        for (int i = 0; i < pickaxeUpgrades.Length; i++)
        {
            if (GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i]) continue;
            if (GameManager.Instance.playerBank >= pickaxeCosts[i])
            {
                GameManager.Instance.playerBank -= pickaxeCosts[i];
                GameManager.Instance.playerPickaxeQuality += pickaxeUpgradeValues[i];
                GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i] = true;
                    
                pickaxeUpgrades[i].interactable = false; 
                if (i + 1 < pickaxeUpgrades.Length) pickaxeUpgrades[i + 1].interactable = true; 
                    
                RefreshUI();
                break; 
            }
        }
    }

    private void UpgradeSpeed()
    {
        for (int i = 0; i < speedUpgrades.Length; i++)
        {
            if (GameManager.Instance.unlockedUpgrades.speedUpgrades[i]) continue;
            if (GameManager.Instance.playerBank >= resistanceAndSpeedCosts[i])
            {
                GameManager.Instance.playerBank -= resistanceAndSpeedCosts[i];
                GameManager.Instance.playerMovementSpeed += speedUpgradeValues[i];
                GameManager.Instance.unlockedUpgrades.speedUpgrades[i] = true;
                    
                speedUpgrades[i].interactable = false; 
                if (i + 1 < speedUpgrades.Length) speedUpgrades[i + 1].interactable = true; 
                    
                RefreshUI();
                break; 
            }
        }
    }
}
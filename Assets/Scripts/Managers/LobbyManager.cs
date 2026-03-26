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
    public int[] resistanceUpgradeValues = { 30, 45, 60, 90, 135 };
    public int[] pickaxeUpgradeValues = { 1, 1, 1, };
    public float[] speedUpgradeValues = { 0.5f, 0.8f, 1.2f, 1.8f, 2.7f };
    public int[] resistanceAndSpeedCosts = { 30, 75, 160, 350, 750 };
    public int[] pickaxeCosts = { 50, 150, 450 };

    private void Start()
    {
        // Grab the info from the Singleton as soon as the Lobby opens
        dayText.text = $"Day {GameManager.Instance.CurrentDay}";
        quotaText.text = $"This day's Quota: ${GameManager.Instance.CurrentQuota}";
        statsText.text = $"Resistance: \n{GameManager.Instance.playerResistance}\nPickaxe Quality: \n{GameManager.Instance.playerPickaxeQuality}\nMovement Speed: \n{GameManager.Instance.playerMovementSpeed}\nLast Quota: \n${GameManager.Instance.LastQuota}";

        // Dragging the function all the way to the button is boring as shit, so we'll just add the listener here
        nextDayButton.onClick.AddListener(OnNextDayClicked);

        for (int i = 0; i < resistanceUpgrades.Length; i++)
        {
            if (i - 1 >= 0)
            {
                if (!GameManager.Instance.unlockedUpgrades.resistanceUpgrades[i - 1])
                {
                    resistanceUpgrades[i].interactable = false;
                }
            }
        }
        for (int i = 0; i < pickaxeUpgrades.Length; i++)
        {
            if (i - 1 >= 0)
            {
                if (!GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i - 1])
                {
                    pickaxeUpgrades[i].interactable = false;
                }
            }
        }
        for (int i = 0; i < speedUpgrades.Length; i++)
        {
            if (i - 1 >= 0)
            {
                if (!GameManager.Instance.unlockedUpgrades.speedUpgrades[i - 1])
                {
                    speedUpgrades[i].interactable = false;
                }
            }
        }
    }

    private void OnNextDayClicked()
    {
        // Disable the button immediately so they can't double-click it while it transitions
        nextDayButton.interactable = false;

        // Tell the GameManager to actually load the level now
        GameManager.Instance.EnterMinesFromLobby();
    }

    public void UpgradeResistance()
    {
        // 0 is my default value, I'm gonna have to run through the upgrades scriptable object and check how many are active to know which one is the next
        if (GameManager.Instance.Player.Inventory.money >= resistanceAndSpeedCosts[0])
        {
            GameManager.Instance.Player.Inventory.money -= resistanceAndSpeedCosts[0];
            GameManager.Instance.playerResistance += resistanceUpgradeValues[0];
            GameManager.Instance.unlockedUpgrades.resistanceUpgrades[0] = true;
            resistanceUpgrades[0].interactable = false; // Disable the button after purchase
            if (resistanceUpgrades.Length > 1)
            {
                resistanceUpgrades[1].interactable = true; // Unlock the next upgrade
            }
        }
    }
    public void UpgradePickaxe()
    {

    }
    public void UpgradeSpeed()
    {

    }
}
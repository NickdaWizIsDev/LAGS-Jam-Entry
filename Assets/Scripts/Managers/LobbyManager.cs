using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Managers
{
    public class LobbyManager : MonoBehaviour
    {
        [Header("UI References")] public TextMeshProUGUI dayText;
        public TextMeshProUGUI quotaText;
        public TextMeshProUGUI statsText;
        public Button nextDayButton;
        public Button[] resistanceUpgrades, pickaxeUpgrades, speedUpgrades;
        public Button helmetUpgrade, betterPickaxeUpgrade, promotion, doubleValue;

        public int[] resistanceUpgradeValues = { 30, 45, 60, 90, 135 };
        public int[] pickaxeUpgradeValues = { 1, 1, 1 };
        public float[] speedUpgradeValues = { 0.5f, 0.8f, 1.2f, 1.8f, 2.7f };
        public int[] resistanceAndSpeedCosts = { 30, 75, 160, 350, 750 };
        public int[] pickaxeCosts = { 50, 150, 450 };
        public int helmetCost = 150;
        public int betterPickaxeCost = 200;
        public int promotionCost = 500;
        public int doubleCost = 1200;

        public MerchantDialogue merchant;

        private void Start()
        {
            nextDayButton.onClick.AddListener(OnNextDayClicked);

            foreach (var btn in resistanceUpgrades) btn.onClick.AddListener(UpgradeResistance);
            foreach (var btn in pickaxeUpgrades) btn.onClick.AddListener(UpgradePickaxe);
            foreach (var btn in speedUpgrades) btn.onClick.AddListener(UpgradeSpeed);

            // Listeners for special upgrades
            helmetUpgrade.onClick.AddListener(UpgradeHelmet);
            betterPickaxeUpgrade.onClick.AddListener(UpgradeBetterPickaxe);
            promotion.onClick.AddListener(UpgradePromotion);
            doubleValue.onClick.AddListener(UpgradeDoubleValue);

            // Helper function because why run it on update?
            RefreshUI();
            SetupShopButtons();
        }

        private void RefreshUI()
        {
            dayText.text = $"Day {GameManager.Instance.CurrentDay}";
            quotaText.text = $"This day's Quota: ${GameManager.Instance.CurrentQuota}";
            statsText.text =
                $"Resistance: \n{GameManager.Instance.playerResistance}\nPickaxe Quality: \n{GameManager.Instance.playerPickaxeQuality}\nMovement Speed: \n{GameManager.Instance.playerMovementSpeed}\nCoins in your bank: \n${GameManager.Instance.playerBank}";
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

                AddHoverLogic(resistanceUpgrades[i],
                    $"Ah, some more energy for yer expeditions, huh. It'll run ya {resistanceAndSpeedCosts[i]} coins.");
            }

            // same as resistance, won't bother to explain again
            for (int i = 0; i < pickaxeUpgrades.Length; i++)
            {
                if (GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i] ||
                    i > 0 && !GameManager.Instance.unlockedUpgrades.pickaxeUpgrades[i - 1])
                    pickaxeUpgrades[i].interactable = false;
                else
                    pickaxeUpgrades[i].interactable = true;

                AddHoverLogic(pickaxeUpgrades[i], $"A better pickaxe? That'll be {pickaxeCosts[i]} coins.");
            }

            for (int i = 0; i < speedUpgrades.Length; i++)
            {
                if (GameManager.Instance.unlockedUpgrades.speedUpgrades[i] ||
                    i > 0 && !GameManager.Instance.unlockedUpgrades.speedUpgrades[i - 1])
                    speedUpgrades[i].interactable = false;
                else
                    speedUpgrades[i].interactable = true;

                AddHoverLogic(speedUpgrades[i],
                    $"Want some better shoes, friend? They cost {resistanceAndSpeedCosts[i]} coins. Tho I ain't sure the mines are a place for joggin'.");
            }

            // Setup special upgrades
            helmetUpgrade.interactable = !GameManager.Instance.unlockedUpgrades.helmetUpgrade;
            betterPickaxeUpgrade.interactable = !GameManager.Instance.unlockedUpgrades.betterPickaxeUpgrade;
            promotion.interactable = !GameManager.Instance.unlockedUpgrades.promotion;

            AddHoverLogic(helmetUpgrade, $"Keep your head safe. {helmetCost} coins.");
            AddHoverLogic(betterPickaxeUpgrade, $"The real deal. {betterPickaxeCost} coins.");
            AddHoverLogic(promotion, $"Wanna move up in the world? {promotionCost} coins.");
            AddHoverLogic(doubleValue,
                $"Hey. For {doubleCost} coins, I'll tell the manager you're indisposable. He'll pay more for your ores.");
        }

        private void AddHoverLogic(Button btn, string hoverText)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>() ??
                                   btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { merchant.SpeakSpecificLine(hoverText); });
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { merchant.SpeakRandomLine(); });
            trigger.triggers.Add(pointerExit);
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
                    merchant.SpeakRandomLine();
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
                    merchant.SpeakRandomLine();
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
                    merchant.SpeakRandomLine();
                    break;
                }
            }
        }

        private void UpgradeHelmet()
        {
            if (GameManager.Instance.unlockedUpgrades.helmetUpgrade) return;
            if (GameManager.Instance.playerBank >= helmetCost)
            {
                GameManager.Instance.playerBank -= helmetCost;
                GameManager.Instance.unlockedUpgrades.helmetUpgrade = true;
                helmetUpgrade.interactable = false;
                RefreshUI();
                merchant.SpeakRandomLine();
            }
        }

        private void UpgradeBetterPickaxe()
        {
            if (GameManager.Instance.unlockedUpgrades.betterPickaxeUpgrade) return;
            if (GameManager.Instance.playerBank >= betterPickaxeCost)
            {
                GameManager.Instance.playerBank -= betterPickaxeCost;
                GameManager.Instance.unlockedUpgrades.betterPickaxeUpgrade = true;
                betterPickaxeUpgrade.interactable = false;
                RefreshUI();
                merchant.SpeakRandomLine();
            }
        }

        private void UpgradePromotion()
        {
            if (GameManager.Instance.unlockedUpgrades.promotion) return;
            if (GameManager.Instance.playerBank >= promotionCost)
            {
                GameManager.Instance.playerBank -= promotionCost;
                GameManager.Instance.unlockedUpgrades.promotion = true;
                promotion.interactable = false;
                RefreshUI();
                merchant.SpeakRandomLine();
            }
        }

        private void UpgradeDoubleValue()
        {
            if (GameManager.Instance.unlockedUpgrades.doubleValueUpgrade) return;
            if (GameManager.Instance.playerBank >= doubleCost)
            {
                GameManager.Instance.playerBank -= doubleCost;
                GameManager.Instance.unlockedUpgrades.doubleValueUpgrade = true;
                doubleValue.interactable = false;
                RefreshUI();
                merchant.SpeakRandomLine();
            }
        }
    }
}
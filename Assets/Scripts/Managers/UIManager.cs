using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [Header("Player HUD")]
        public Image resistanceMeter;
        public TextMeshProUGUI interactionHUD;
        public TextMeshProUGUI quotaAndDayInfo;

        [Header("Pause Menu")]
        public Canvas pauseUI;

        [Header("Game Over Screen")]
        public CanvasGroup quotaNotMetScreen;
        public TextMeshProUGUI quotaNotMetText;
        public CanvasGroup missingInActionScreen;
        public TextMeshProUGUI missingInActionText;

        void Awake()
        {            
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            pauseUI.gameObject.SetActive(GameManager.Instance.IsGamePaused);

            quotaAndDayInfo .text = $"Day {GameManager.Instance.CurrentDay}\nQuota: {GameManager.Instance.CurrentQuota}";
            resistanceMeter.fillAmount = GameManager.Instance.Player.currentResistance / GameManager.Instance.Player.maxResistance;
        }
    }
}
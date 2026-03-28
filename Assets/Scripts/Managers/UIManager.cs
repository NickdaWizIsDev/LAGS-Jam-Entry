using System;
using PrimeTween;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [Header("Loading Bar")] 
        public Image loadingBarFill;
        public CanvasGroup loadingScreenGroup;

        [Header("Game Over Screen")]
        public CanvasGroup quotaNotMetScreen;
        public CanvasGroup missingInActionScreen;

        void Awake()
        {            
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Ensure the loading screen is fully visible the moment the scene starts
            loadingScreenGroup.alpha = 1f;
            loadingScreenGroup.gameObject.SetActive(true);
            loadingBarFill.fillAmount = 0f;
            
            // Hide Game Over screens
            quotaNotMetScreen.alpha = 0f;
            quotaNotMetScreen.gameObject.SetActive(false);
            missingInActionScreen.alpha = 0f;
            missingInActionScreen.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance is null || GameManager.Instance.Player is null) return;

            if (GameManager.Instance.Player.Inventory != null)
            {
                quotaAndDayInfo.text = $"Day {GameManager.Instance.CurrentDay}\nQuota: {GameManager.Instance.CurrentQuota}\nHolding: ${GameManager.Instance.Player.Inventory.money}";
            }
            resistanceMeter.fillAmount = GameManager.Instance.Player.currentResistance / GameManager.Instance.Player.maxResistance;
        }
        
        public void UpdateLoadingBar(float progress)
        {
            loadingBarFill.fillAmount = progress;
        }

        public void FadeOutLoadingScreen()
        {
            // PrimeTween fades the alpha, then automatically disables the GameObject when finished!
            Tween.Alpha(loadingScreenGroup, 0f, duration: 1f).OnComplete(() => loadingScreenGroup.gameObject.SetActive(false));
        }

        public void TriggerMIA()
        {
            missingInActionScreen.gameObject.SetActive(true);
            Tween.Alpha(missingInActionScreen,0f, 1f, duration: 2f, useUnscaledTime:true);
        }

        public void TriggerFired()
        {
            quotaNotMetScreen.gameObject.SetActive(true);
            Tween.Alpha(quotaNotMetScreen,0f, 1f, duration: 2f, useUnscaledTime:true);
        }

        public void RestartGame()
        {
            GameManager.Instance.RestartGame();
        }

        public void QuitToMainMenu()
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
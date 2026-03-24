using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [Header("Player HUD")]
        public Image resistanceMeter;
        public TextMeshProUGUI interactionText;

        [Header("Pause Menu")]
        public Canvas pauseUI;

        [Header("Game Over Screen")]
        public CanvasGroup cuotaNotMetScreen;
        public TextMeshProUGUI cuotaNotMetText;
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
    }
}
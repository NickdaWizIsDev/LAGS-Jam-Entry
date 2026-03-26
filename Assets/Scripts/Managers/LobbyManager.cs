using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI quotaText;
    public Button nextDayButton;

    private void Start()
    {
        // Grab the info from the Singleton as soon as the Lobby opens
        dayText.text = $"Day {GameManager.Instance.CurrentDay}";
        quotaText.text = $"Tomorrow's Quota: {GameManager.Instance.CurrentQuota} Minerals";

        // Dragging the function all the way to the button is boring as shit, so we'll just add the listener here
        nextDayButton.onClick.AddListener(OnNextDayClicked);
    }

    private void OnNextDayClicked()
    {
        // Disable the button immediately so they can't double-click it while it transitions
        nextDayButton.interactable = false; 
        
        // Tell the GameManager to actually load the level now
        GameManager.Instance.EnterMinesFromLobby();
    }
}
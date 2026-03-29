using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject contractPanel;
    public GameObject mainMenuPanel;

    [Header("Buttons")]
    public Button signContractButton;
    public Button playButton;
    public Button quitButton;

    [Header("Scene Settings")]
    public string lobbySceneName = "LobbyScene"; 

    private void Start()
    {
        // Hook up the buttons
        signContractButton.onClick.AddListener(SignContract);
        playButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);

        // Check if the player has already signed their life away to the Corporation
        if (PlayerPrefs.GetInt("HasSignedContract", 0) == 0)
        {
            ShowContractScreen();
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void ShowContractScreen()
    {
        contractPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void ShowMainMenu()
    {
        contractPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void SignContract()
    {
        // Lock in the contract signature permanently
        PlayerPrefs.SetInt("HasSignedContract", 1);
        PlayerPrefs.Save(); 
        
        // Move straight to the main menu
        ShowMainMenu();
    }

    private void StartGame()
    {
        // Disable the button to prevent double-clicking while the scene loads
        playButton.interactable = false;
        SceneManager.LoadScene(lobbySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Miner has abandoned their post. Quitting game...");
        Application.Quit();
    }
}
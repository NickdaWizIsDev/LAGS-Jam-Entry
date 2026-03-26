using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }
    public void OnPlayButtonClicked()
    {
        GameManager.Instance.EnterMinesFromMainMenu();
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
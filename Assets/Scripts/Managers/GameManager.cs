using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerController Player { get; private set; }
    public CaveDataGenerator Generator { get; private set; }

    [SerializeField] private int _currentDay = 1;
    public int CurrentDay { get => _currentDay; private set => _currentDay = value; }
    [SerializeField] private int _currentQuota;
    public int LastQuota { get; private set; }
    public int CurrentQuota { get => _currentQuota; private set => _currentQuota = value; }
    public int playerBank;
    int basePlayerResistance = 180;
    public int playerResistance;
    int basePlayerPickaxeQuality = 1;
    public int playerPickaxeQuality;
    float basePlayerMovementSpeed = 3;
    public float playerMovementSpeed;
    public PlayerUpgrades unlockedUpgrades;
    [SerializeField] private bool _isGamePaused;
    public bool IsGamePaused { get => _isGamePaused; private set => _isGamePaused = value; }
    public InputActionReference pauseAction;

    // Async ops are weird
    private AsyncOperation backgroundSceneLoad;

#region Your run off the mill unity event functions
    // Remember, execution order is Awake -> OnEnable -> OnSceneLoaded -> Start = Update (first frame)
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        if (Instance != null && Instance != this) return;
        SceneManager.sceneLoaded += OnSceneLoaded;
        pauseAction.action.Enable();
        pauseAction.action.started += TogglePause;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Mines")
        {
            StartNextDay();
        }
        else if (scene.name == "Lobby")
        {
            // As soon as the Lobby loads, start silently loading the Mines in the background, we're cool like that
            StartCoroutine(PreloadMinesAsync());
        }

        CheckCursorState();
    }
    private void Start()
    {
        // Only gets called on the very very first load of the script, i.e the Main Menu
        ResetUpgrades();
        playerResistance = basePlayerResistance;
        playerPickaxeQuality = basePlayerPickaxeQuality;
        playerMovementSpeed = basePlayerMovementSpeed;
        
        CalculateNextQuota(); // Set the Day 1 quota immediately        
        CheckCursorState();
    }
    private void Update()
    {
        
    }

    private void OnDisable()
    {
        if (Instance != null && Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        pauseAction.action.Disable();
        pauseAction.action.started -= TogglePause;
    }
#endregion

    private void StartNextDay()
    {
        // 1. Lock the player in place and hide the cursor
        IsGamePaused = false;
        CheckCursorState();

        // 2. Start the generator, pass the completion action AND the progress action
        StartCoroutine(Generator.GenerateLevelAsync(CurrentDay, 
            (playerPos) => 
            {
                Player.SetEntrancePosition(playerPos);
                // 3. Fade out the black screen!
                UIManager.Instance.FadeOutLoadingScreen();
            },
            (progressAmount) => 
            {
                // 4. Update the UI Loading bar
                UIManager.Instance.UpdateLoadingBar(progressAmount);
            }
        ));
    }

    public void ExitTheMines()
    {
        int playerCut = Mathf.RoundToInt(Player.Inventory.money * 0.30f);
        Player.Inventory.money = 0;
        playerBank += playerCut;

        // THE QUOTA CHECK
        if (playerBank < CurrentQuota)
        {
            GameOver(true); // True = Fired for missing quota!
            return;
        }

        // If they survived and met quota, deduct it and continue
        playerBank -= CurrentQuota;
        CurrentDay++;
        CalculateNextQuota();
        SceneManager.LoadScene("Lobby");
    }

    private void CalculateNextQuota()
    {
        LastQuota = CurrentQuota;
        // Very slight exponential scaling to punish the "Jack of all trades" playstyle and encourage specialization, 
        // but mostly just to make sure the game gets harder as you progress.
        int baseQuota = 125;
        float difficultyMultiplier = Mathf.Pow(CurrentDay, 1.2f);
        var randomVariance = UnityEngine.Random.Range(0.8f, 1.4f); // Add some randomness to the quota so it's not the same every time
        CurrentQuota = Mathf.RoundToInt(baseQuota * difficultyMultiplier * randomVariance);
    }

    private System.Collections.IEnumerator PreloadMinesAsync()
    {
        yield return new WaitForSeconds(0.2f); 

        backgroundSceneLoad = SceneManager.LoadSceneAsync("Mines");
        
        backgroundSceneLoad.allowSceneActivation = false; 
        
        while (!backgroundSceneLoad.isDone)
        {
            yield return null;
        }
    }

    public void EnterMinesFromLobby()
    {
        // Make a button in the lobby that's like "venture down" or smth quirky like that
        if (backgroundSceneLoad != null)
        {
            backgroundSceneLoad.allowSceneActivation = true; // Instantly transition
        }
    }

    public void EnterMinesFromMainMenu()
    {
        SceneManager.LoadSceneAsync("Mines"); // Async loading but let it switch automatically
    }

    private void TogglePause(InputAction.CallbackContext ctx)
    {
        if(SceneManager.GetActiveScene().name != "Mines") { return; } // Don't allow pausing in the lobby or main menu, why the hell would you do that, idiot, stupid
        IsGamePaused = !IsGamePaused;
        Time.timeScale = IsGamePaused ? 0f : 1f;

        UIManager.Instance.pauseUI.gameObject.SetActive(Instance.IsGamePaused);

        CheckCursorState();
    }
    public void StartManually()
    {
        StartNextDay();
    }
    private void CheckCursorState()
    {        
        bool inMines = SceneManager.GetActiveScene().name == "Mines";
        Cursor.lockState = (IsGamePaused || !inMines) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsGamePaused || !inMines;        
    }
    private void ResetUpgrades()
    {
        unlockedUpgrades.ResetAllUpgrades();
    }
    public void GameOver(bool missedQuota)
    {
        IsGamePaused = true; // Pause
        Time.timeScale = 0f; // Freeze the game
        CheckCursorState(); // Unlock the cursor

        if (missedQuota)
        {
            UIManager.Instance.TriggerFired();
        }
        else
        {
            UIManager.Instance.TriggerMIA();
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;
        
        CurrentDay = 1;
        playerBank = 0;
        CalculateNextQuota(); 
        ResetUpgrades();
        
        SceneManager.LoadScene("Mines");
    }
    public void ReturnToMainMenu()
    {
        // Unfreeze the game just in case
        Time.timeScale = 1f;
        IsGamePaused = false;
        
        // Reset everything
        CurrentDay = 1;
        playerBank = 0;
        CalculateNextQuota(); 
        ResetUpgrades();
        
        SceneManager.LoadScene("MainMenu");
    }

#region Reference setups
    public void SetPlayer(PlayerController playerController) { Player = playerController; }
    public void SetGenerator(CaveDataGenerator caveDataGenerator) { Generator = caveDataGenerator; }
#endregion
}
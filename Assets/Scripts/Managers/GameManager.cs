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
    public int CurrentQuota { get => _currentQuota; private set => _currentQuota = value; }
    public bool IsGamePaused { get; internal set; }
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
        pauseAction.action.Enable();
        pauseAction.action.performed += _ => TogglePause();
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

            // Disabling this for debugging, the lobby loads you immediately to the mines for some reason
            StartCoroutine(PreloadMinesAsync());
        }

        CheckCursorState();
    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        CalculateNextQuota(); // Set the Day 1 quota immediately        
        
        CheckCursorState();
    }
    private void Update()
    {
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        pauseAction.action.Disable();
        pauseAction.action.performed -= _ => TogglePause();
    }
#endregion

    private void StartNextDay()
    {
        var playerPos = Generator.GenerateLevel(CurrentDay);
        Player.SetEntrancePosition(playerPos);
    }

    public void ExitTheMines()
    {
        CurrentDay++;
        CalculateNextQuota();
        SceneManager.LoadScene("Lobby");
    }

    private void CalculateNextQuota()
    {
        // Very slight exponential scaling to punish the "Jack of all trades" playstyle and encourage specialization, 
        // but mostly just to make sure the game gets harder as you progress.
        int baseQuota = 250;
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

    public void TogglePause()
    {
        if(SceneManager.GetActiveScene().name != "Mines") { return; } // Don't allow pausing in the lobby or main menu, why the hell would you do that, idiot, stupid
        IsGamePaused = !IsGamePaused;
        Time.timeScale = IsGamePaused ? 0f : 1f;

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

#region Reference setups
    public void SetPlayer(PlayerController playerController) { Player = playerController; }
    public void SetGenerator(CaveDataGenerator caveDataGenerator) { Generator = caveDataGenerator; }
    #endregion
}
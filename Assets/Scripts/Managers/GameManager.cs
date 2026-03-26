using System;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentDay { get; private set; } = 1;
    public PlayerController Player { get; private set; }
    public UIManager UIManager => UIManager.Instance;
    public CaveDataGenerator Generator { get; private set; }
    public bool IsGamePaused { get; internal set; }

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

    private void Start()
    {
        StartNextDay();
    }

    private void Update()
    {
        Cursor.lockState = IsGamePaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsGamePaused;
    }

    private void StartNextDay()
    {
        var playerPos = Generator.GenerateLevel(CurrentDay);
        Player.SetEntrancePosition(playerPos);
    }

    #region Reference setups
    public void SetPlayer(PlayerController playerController)
    {
        Player = playerController;
    }
    public void SetGenerator(CaveDataGenerator caveDataGenerator)
    {
        Generator = caveDataGenerator;
    }
    #endregion
}
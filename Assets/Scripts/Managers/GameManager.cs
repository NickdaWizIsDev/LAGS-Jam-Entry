using System;
using Player;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int CurrentDay { get; private set; } = 1;
    public PlayerController Player { get; private set; }
    public UIManager UIManager { get { return UIManager.Instance; } }
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

    public void StartNextDay()
    {
        
    }

    internal void SetPlayer(PlayerController playerController)
    {
        Player = playerController;
    }
}
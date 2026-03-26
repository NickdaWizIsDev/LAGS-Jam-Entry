using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Upgrades", menuName = "Player/Upgrades")]
public class PlayerUpgrades : ScriptableObject
{
    public bool[] resistanceUpgrades = new bool[5];
    public bool[] pickaxeUpgrades = new bool[5];
    public bool[] speedUpgrades = new bool[5];

    public void ResetAllUpgrades()
    {
        // Syntax: Array.Clear(arrayName, startingIndex, lengthToClear)
        Array.Clear(resistanceUpgrades, 0, resistanceUpgrades.Length);
        Array.Clear(pickaxeUpgrades, 0, pickaxeUpgrades.Length);
        Array.Clear(speedUpgrades, 0, speedUpgrades.Length);
        
        Debug.Log("All upgrades reset to false.");
    }
}
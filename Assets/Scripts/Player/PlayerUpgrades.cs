using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Upgrades", menuName = "Player/Upgrades")]
public class PlayerUpgrades : ScriptableObject
{
    public bool[] resistanceUpgrades = new bool[5];
    public bool[] pickaxeUpgrades = new bool[3];
    public bool[] speedUpgrades = new bool[5];
    public bool helmetUpgrade;
    public bool betterPickaxeUpgrade;
    public bool promotion;

    public void ResetAllUpgrades()
    {
        Array.Clear(resistanceUpgrades, 0, resistanceUpgrades.Length);
        Array.Clear(pickaxeUpgrades, 0, pickaxeUpgrades.Length);
        Array.Clear(speedUpgrades, 0, speedUpgrades.Length);
        helmetUpgrade = false;
        betterPickaxeUpgrade = false;
        promotion = false;
        
        Debug.Log("All upgrades reset to false.");
    }
}
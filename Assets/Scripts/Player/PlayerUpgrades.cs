using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Upgrades", menuName = "Player/Upgrades")]
public class PlayerUpgrades : ScriptableObject
{
    public bool[] resistanceUpgrades = new bool[5];
    public bool[] pickaxeUpgrades = new bool[3];
    public bool[] speedUpgrades = new bool[5];
    public bool helmetUpgrade;                      // Gives a lot more light
    public bool betterPickaxeUpgrade;               // Upgrade your pickaxe type instead of material (mine faster)
    public bool promotion;                          // Make your cut from the quotas a bit better (from 30 > 50%?)
    public bool doubleValueUpgrade;                             // Makes all ores worth double

    public void ResetAllUpgrades()
    {
        Array.Clear(resistanceUpgrades, 0, resistanceUpgrades.Length);
        Array.Clear(pickaxeUpgrades, 0, pickaxeUpgrades.Length);
        Array.Clear(speedUpgrades, 0, speedUpgrades.Length);
        helmetUpgrade = false;
        betterPickaxeUpgrade = false;
        promotion = false;
        doubleValueUpgrade = false;
        
        Debug.Log("All upgrades reset to false.");
    }
}
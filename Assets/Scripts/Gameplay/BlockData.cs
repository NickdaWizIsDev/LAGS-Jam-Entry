using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Data", menuName = "Mining Economy/Block Data")]
public class BlockData : ScriptableObject
{
    public enum BlockType { Rock, Dirt, Slate, Ore }
    public BlockType blockType;
    public string blockName;
    public int coinValue;
    public float timeToBreak;
    public int blockQuality;
}
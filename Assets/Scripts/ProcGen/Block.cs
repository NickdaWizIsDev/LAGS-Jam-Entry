using System;
using System.Collections;
using UnityEngine;
using Managers;
using UnityEngine.Audio;

public class Block : Interactable
{
    public BlockData blockData;
    private bool breaking;
    void Start()
    {
        canInteract = true;
    }

    public override string OnHover()
    {
        if(blockData.blockType == BlockData.BlockType.Slate)
        {
            return "" + blockData.blockName + "\nUnbreakable";
        }
        _outline.enabled = true;
        return "" + blockData.blockName + hoverText + "\nQuality: " + blockData.blockQuality;
    }
    public override void AbortHover()
    {
        // Make sure we abort the breaking action if we were in the middle of it when the player looked away
        _outline.enabled = false;
        StopAllCoroutines();
        breaking = false;
    }

    public override void Interact()
    {
        // Slate is unbreakable, so we just ignore it. Otherwise start breaking the block
        if(blockData.blockType == BlockData.BlockType.Slate) return;
        Debug.Log("Breaking this block...");
        StartCoroutine(BreakBlock());
    }

    public override void OnRelease()
    {
        // Abort the breaking action if we release the button before the block is actually broken
        StopAllCoroutines();
        breaking = false;
        // And reset the texture/shader value that indicates breakage to the base, whatever we end up cooking
    }

    private IEnumerator BreakBlock()
    {
        if (breaking) yield break;
        breaking = true;
        // This avoids the routine starting twice

        float speedMultiplier = 1f + (GameManager.Instance.Player.pickaxePower * 0.15f);
        if (GameManager.Instance.unlockedUpgrades.betterPickaxeUpgrade)
        {
            speedMultiplier *= 2f;
        }
        
        float actualTimeToBreak = blockData.timeToBreak / speedMultiplier;

        // Wait for however long it takes to break this type of block
        yield return new WaitForSeconds(actualTimeToBreak);
        Debug.Log("Block broken!");
        Destroy(gameObject);
        AudioManager.Instance.PlayBreakSFX();
        GameManager.Instance.Player.AddMoney(GameManager.Instance.unlockedUpgrades.doubleValueUpgrade? 2 * blockData.coinValue : blockData.coinValue);
        breaking = false;
    }
}
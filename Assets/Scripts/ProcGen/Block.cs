using System;
using System.Collections;
using UnityEngine;

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

        // Wait for however long it takes to break this type of block
        yield return new WaitForSeconds(blockData.timeToBreak);
        Debug.Log("Block broken!");
        Destroy(gameObject);
        GameManager.Instance.Player.AddMoney(blockData.coinValue);
        breaking = false;
    }
}
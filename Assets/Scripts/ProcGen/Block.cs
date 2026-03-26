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

    public override void Interact()
    {
        if(blockData.blockType == BlockData.BlockType.Slate) return;
        Debug.Log("Breaking this block...");
        StartCoroutine(BreakBlock());
    }

    public override void OnRelease()
    {
        StopAllCoroutines();
        breaking = false;
        // And reset the texture/shader value that indicates breakage to the base, whatever we end up cooking
    }

    private IEnumerator BreakBlock()
    {
        if (breaking) yield break;
        breaking = true;
        yield return new WaitForSeconds(blockData.timeToBreak); // Make you hold down to break the block (if you release before the time is up, the coroutine is cancelled)
        Debug.Log("Block broken!");
        Destroy(gameObject);
        breaking = false;
    }
}
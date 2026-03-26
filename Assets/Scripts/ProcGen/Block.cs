using System;
using System.Collections;
using UnityEngine;

public class Block : Interactable
{
    public enum BlockType { Air, Rock, Dirt, Ore, Slate, Door }
    public BlockType blockType;
    public float timeToBreak;
    private bool breaking;
    void Start()
    {
        canInteract = true;
    }
    public override string OnHover()
    {
        _outline.enabled = true;
        return "" + blockType;
    }

    public override void Interact()
    {
        if(blockType == BlockType.Slate) return;
        Debug.Log("Breaking this block...");
        StartCoroutine(BreakBlock());
    }

    public override void OnRelease()
    {
        StopAllCoroutines();
        breaking = false;
        // And reset the texture/shader value that indicates breakage to the base
    }

    private IEnumerator BreakBlock()
    {
        if (breaking) yield break;
        breaking = true;
        yield return new WaitForSeconds(timeToBreak); // Make you hold down to break the block
        Debug.Log("Block broken!");
        Destroy(gameObject);
        breaking = false;
    }
}
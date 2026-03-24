using System;
using System.Collections;
using UnityEngine;

public class Block : Interactable
{
    public enum BlockType { Air, Rock, Dirt, Ore, Slate, Door }
    public BlockType blockType;
    void Start()
    {
        canInteract = true;
    }
    public override string OnHover()
    {
        return "Press E to interact with the block.";
    }

    public override void Interact()
    {
        
    }

    public override void OnHold()
    {
        Debug.Log("Breaking this block...");
        StartCoroutine(BreakBlock());
    }

    private IEnumerator BreakBlock()
    {
        yield return new WaitForSeconds(1f); // Simulate time taken to break the block
        Debug.Log("Block broken!");
        Destroy(gameObject);
    }
}
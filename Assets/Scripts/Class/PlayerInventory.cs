using UnityEngine;
using System;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerInventory : MonoBehaviour
{
    public TextMeshProUGUI inventoryText;
    public List inventoryList = new List();
    internal void AddItem(int v)
    {
        inventoryList.AddNode(v);
    }

    void Update()
    {
        inventoryText.text = inventoryList.PrintList();
    }
}
public class List
{
    public Node head;
    public List()
    {
        head = null;
    }

    public void AddNode(int itemID)
    {
        Node newNode = new Node(itemID);
        if (head != null)
        {
            Node currentHead = head;

            while (currentHead.nextNode != null)
            {
                currentHead = currentHead.nextNode;
            }
            currentHead.nextNode = newNode;
            newNode.previousNode = currentHead;
        }
        else head = newNode;
    }

    public void RemoveHead()
    {
        if (head != null)
        {
            head = head.nextNode;
            if (head != null)
            {
                head.previousNode = null;
            }
        }
    }

    public void DeleteNode(Node nodeToDelete)
    {
        if (nodeToDelete == head)
        {
            RemoveHead();
            return;
        }

        Node previousNode = nodeToDelete.previousNode;
        Node nextNode = nodeToDelete.nextNode;

        if (previousNode != null)
        {
            previousNode.nextNode = nextNode;
        }

        if (nextNode != null)
        {
            nextNode.previousNode = previousNode;
        }
    }

    public void CountNodes()
    {
        int count = 0;
        Node currentNode = head;
        while (currentNode != null)
        {
            count++;
            currentNode = currentNode.nextNode;
        }
        Debug.Log("Total nodes in the list: " + count);
    }

    public string PrintList()
    {
        string listString = "null -> ";
        Node currentNode = head;
        while (currentNode != null)
        {
            listString += currentNode.itemID + " -> ";
            currentNode = currentNode.nextNode;
        }
        listString += "null";
        return listString;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerInventory))]
public class PlayerInventoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerInventory myScript = (PlayerInventory)target;
        if (GUILayout.Button("Add Item"))
        {
            myScript.AddItem(0);
        }
        if(GUILayout.Button("Count Nodes"))
        {
            myScript.inventoryList.CountNodes();
        }
    }
}
#endif
public class Node
{
    public Node previousNode;
    public int itemID;
    public Node nextNode;    

    public Node(int itemID)
    {
        previousNode = null;
        this.itemID = itemID;
        nextNode = null;
    }
}
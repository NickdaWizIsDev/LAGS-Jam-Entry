using System;
using UnityEngine;
using UnityEngine.Events;

public class  Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public bool canInteract = true;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
    public string hoverText = "";

    public virtual string OnHover()
    {
        return hoverText;
    }
    public virtual void Interact()
    {
        onInteract?.Invoke();
    }
    public virtual void OnHold()
    {
        
    }
    public virtual void OnRelease()
    {
        
    }
}
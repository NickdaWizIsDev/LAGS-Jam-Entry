using System;
using UnityEngine;
using UnityEngine.Events;

public class  Interactable : MonoBehaviour
{
    public UnityEvent onInteract;
    public bool canInteract = true;
    public bool CanInteract { get { return canInteract; } set { canInteract = value; } }
    public string hoverText = "";
    internal Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    public virtual string OnHover()
    {
        _outline.enabled = true;
        return hoverText;
    }

    public virtual void AbortHover()
    {
        _outline.enabled = false;
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
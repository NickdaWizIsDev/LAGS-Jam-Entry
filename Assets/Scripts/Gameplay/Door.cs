using UnityEngine;

namespace Gameplay
{
    public class Door : Interactable
    {
        public override string OnHover()
        {
            _outline.enabled = true;
            return "Exit the mines?";
        }

        public override void Interact()
        {
            GameManager.Instance.ExitTheMines();
        }
    }
}
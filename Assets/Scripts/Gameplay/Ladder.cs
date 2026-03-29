using UnityEngine;
using Managers;

namespace Gameplay
{
    public class Ladder : Interactable
    {
        public override void Interact()
        {
            GameManager.Instance.Player.isClimbing = !GameManager.Instance.Player.isClimbing;
        }
    }
}
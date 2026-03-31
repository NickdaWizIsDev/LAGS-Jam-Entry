using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerPickaxeAudio : MonoBehaviour
    {
        public void PlayPickSFX()
        {
            AudioManager.Instance.PlayPickSFX();
        }
    }
}
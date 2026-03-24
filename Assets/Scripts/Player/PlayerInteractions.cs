using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteractions : MonoBehaviour
    {
        public Transform cameraPosition;
        public bool canInteract;
        public LayerMask interactionLayer;
        public Interactable currentTarget;
        private void Update()
        {
            Physics.Raycast(cameraPosition.transform.position, cameraPosition.transform.forward, out RaycastHit hitInfo, 3f, interactionLayer);
            if (hitInfo.collider != null && hitInfo.collider.TryGetComponent<Interactable>(out Interactable target) && target.canInteract)
            { canInteract = true; currentTarget = target; UIManager.Instance.interactionText.text = target.OnHover(); ; }
            else if(GameManager.Instance.IsGamePaused) { UIManager.Instance.interactionText.text = ""; }
            else { canInteract = false; currentTarget = null; UIManager.Instance.interactionText.text = ""; }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.IsGamePaused) { return; }
            if(canInteract && currentTarget != null && currentTarget.canInteract)
            switch (context.phase)
                {
                    case InputActionPhase.Started:
                        currentTarget.Interact();
                        break;
                    case InputActionPhase.Performed:
                        currentTarget.OnHold();
                        break;
                    case InputActionPhase.Canceled:
                        currentTarget.OnRelease();
                        break;
                }
        }
    }
}
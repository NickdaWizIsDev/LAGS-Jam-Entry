using System;
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
        public float raycastDistance = 4f;
        public InputActionReference interactAction;

        #region Action setup
        private void OnEnable()
        {
            interactAction.action.Enable();
            interactAction.action.started += OnInteract;
            interactAction.action.performed += OnInteract;
            interactAction.action.canceled += OnInteract;
        }

        private void OnDisable()
        {
            interactAction.action.Disable();
            interactAction.action.started -= OnInteract;
            interactAction.action.performed -= OnInteract;
            interactAction.action.canceled -= OnInteract;
        }
        #endregion

        private void Update()
        {
            Physics.Raycast(cameraPosition.transform.position, cameraPosition.transform.forward, out RaycastHit hitInfo, raycastDistance, interactionLayer);
            if (hitInfo.collider is not null && hitInfo.collider.TryGetComponent<Interactable>(out var target) && target.canInteract && !GameManager.Instance.IsGamePaused)
            {
                // If we've just switched targets, abort the hover on the old one and set it up on the current one
                if (currentTarget != null && currentTarget != target) currentTarget.AbortHover();
                currentTarget = target; 
                UIManager.Instance.interactionText.text = target.OnHover();
            }
            else if (currentTarget != null)
            {
                currentTarget.AbortHover();
                currentTarget = null; 
                UIManager.Instance.interactionText.text = ""; 
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.IsGamePaused || !canInteract || currentTarget == null || !currentTarget.canInteract) { return; }
            switch (context.phase)
                {
                    case InputActionPhase.Started:
                        currentTarget.Interact();
                        break;
                    case InputActionPhase.Performed:
                        break;
                    case InputActionPhase.Canceled:
                        currentTarget.OnRelease();
                        break;
                }
        }
    }
}
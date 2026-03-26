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
        public InputActionReference breakAction;
        public InputActionReference interactAction;
        private bool isHoldingBreak;
        internal PlayerController controller;

        #region Action setup
        private void OnEnable()
        {
            breakAction.action.Enable();
            breakAction.action.started += OnBreak;
            breakAction.action.canceled += OnBreak;

            interactAction.action.Enable();
            interactAction.action.started += OnInteract;
        }

        private void OnDisable()
        {
            breakAction.action.Disable();
            breakAction.action.started -= OnBreak;
            breakAction.action.canceled -= OnBreak;

            interactAction.action.Disable();
            interactAction.action.started -= OnInteract;
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
                UIManager.Instance.interactionHUD.text = target.OnHover();

                if (isHoldingBreak && currentTarget is Block block)
                {
                    if (controller.pickaxePower < block.blockData.blockQuality)
                    {
                        return;
                    }
                    currentTarget.Interact();
                }
            }
            else if (currentTarget != null)
            {
                currentTarget.AbortHover();
                currentTarget = null; 
                UIManager.Instance.interactionHUD.text = ""; 
            }
        }

        private void OnBreak(InputAction.CallbackContext context)
        {
            if(context.started) isHoldingBreak = true;
            if(context.canceled) isHoldingBreak = false;
            if(GameManager.Instance.IsGamePaused || !canInteract || currentTarget == null || !currentTarget.canInteract) { return; }
            if (currentTarget is Block block)
            {
                if (controller.pickaxePower < block.blockData.blockQuality)
                {
                    return;
                }
                switch (context.phase)
                {
                    case InputActionPhase.Started:
                        currentTarget.Interact();
                        break;
                    case InputActionPhase.Canceled:
                        currentTarget.OnRelease();
                        break;
                }
            }
        }
        private void OnInteract(InputAction.CallbackContext context)
        {
            if(GameManager.Instance.IsGamePaused || !canInteract || currentTarget == null || !currentTarget.canInteract) { return; }
            if (context.phase == InputActionPhase.Started)
            {
                currentTarget.Interact();
            }
        }
    }
}
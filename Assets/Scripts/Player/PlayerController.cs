using System;
using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody body;
        [SerializeField] private PlayerInteractions interactions;
        [SerializeField] private Transform playerHead;
        [SerializeField] private GameObject basicPickaxe;
        [SerializeField] private GameObject betterPickaxe;
        public PlayerInventory Inventory;
        
        [Header("Actions")]
        [SerializeField] private InputActionReference moveAction;
        private Vector2 movementInput;

        [Header("Look Settings")] 
        [SerializeField] private float sensitivity = 0.5f;
        [SerializeField] private InputActionReference lookAction;
        private Vector2 lookInput;
        [SerializeField] private float moveSpeed = 3f;
        private float xRotation;

        [Header("Player Stats")]
        [SerializeField] public float maxResistance = 100f;
        public float currentResistance;
        [SerializeField] public int pickaxePower = 1;
        
        [Header("Ladder Settings")]
        public float climbSpeed = 5f;
        private bool isClimbing;

        private Transform cam;

        #region Action setup
        void OnEnable()
        {
            moveAction.action.Enable();
            lookAction.action.Enable();
        }
        void OnDisable()
        {
            moveAction.action.Disable();
            lookAction.action.Disable();
        }
        #endregion

        private void Awake()
        {
            GameManager.Instance.SetPlayer(this);
            interactions.controller = this;
            Inventory = new PlayerInventory();
        }
        private void Start()
        {
            maxResistance = GameManager.Instance.playerResistance;
            currentResistance = maxResistance; // Fill the tank
            
            pickaxePower = GameManager.Instance.playerPickaxeQuality;
            moveSpeed = GameManager.Instance.playerMovementSpeed;
            
            cam = Camera.main?.transform;
        }
        private void Update()
        {
            if (currentResistance <= 0) return; // Prevent double-triggering

            movementInput = moveAction.action.ReadValue<Vector2>();
            lookInput = lookAction.action.ReadValue<Vector2>();
            currentResistance -= Time.deltaTime; 

            if (currentResistance <= 0)
            {
                currentResistance = 0;
                GameManager.Instance.GameOver(false); // False = MIA, not fired
            }
        }
        private void FixedUpdate()
        {
            if (isClimbing)
            {
                // Turn off gravity and map the W/S keys to purely vertical movement
                body.useGravity = false;
                body.linearVelocity = new Vector3(0f, movementInput.y * climbSpeed, 0f);
            }
            else
            {
                // Standard walking movement
                body.useGravity = true;
                Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
                body.linearVelocity = new Vector3(moveDirection.x * moveSpeed, body.linearVelocity.y, moveDirection.z * moveSpeed);
            }
        }
        private void LateUpdate()
        {
            if(GameManager.Instance.IsGamePaused) return; // Don't let the player look around if the game is paused
            HandleCameraPitch();
            HandleBodyRotation();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Ladder>(out var ladder)) isClimbing = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Ladder>(out var ladder)) isClimbing = false;
        }

        #region  Input Handling
        public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
        #endregion
    
        #region Helper Functions
        private void HandleBodyRotation()
        {
            if (lookInput.x == 0) return;
            
            // Rotate the player's transform directly around the Y axis
            transform.Rotate(Vector3.up * (lookInput.x * sensitivity));
        }
        private void HandleCameraPitch()
        {
            xRotation -= lookInput.y * sensitivity;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);
            cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        public void SetEntrancePosition(Vector3 playerPos)
        {
            body.useGravity = true;
            body.position = playerPos;
        }

        internal void AddMoney(int coinValue)
        {
            Inventory.money += coinValue;
        }
        #endregion
    }

    [Serializable]public class PlayerInventory
    {
        public int money;
    }
}
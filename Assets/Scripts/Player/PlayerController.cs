using System;
using Gameplay;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody body;
        [SerializeField] private PlayerInteractions interactions;
        [SerializeField] private Animator anim;
        [SerializeField] private Transform playerHead;
        [SerializeField] private GameObject basicPickaxe;
        [SerializeField] private MeshRenderer basicPickaxeRenderer;
        [SerializeField] private GameObject betterPickaxe;
        [SerializeField] private MeshRenderer betterPickaxeRenderer;
        [SerializeField] private Light headLight;
        [SerializeField] private AnimationClip[] pickaxeSwings;
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
        [SerializeField] public float baseLightPower = 1.5f;
        [SerializeField] public float baseLightRange = 6f;
        
        [Header("Ladder Settings")]
        public float climbSpeed = 5f;
        public bool isClimbing;
        private bool canClimb;

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

            var mat = Resources.Load<Material>("Materials/Pebbles");
            switch (pickaxePower)
            {
                case 1:
                    mat = Resources.Load<Material>("Materials/Pebbles");
                    break;
                case 2:
                    mat = Resources.Load<Material>("Materials/Mineral 1");
                    break;
                case 3:
                    mat = Resources.Load<Material>("Materials/Mineral 3");
                    break;
                case 4:
                    mat = Resources.Load<Material>("Materials/Mineral 4");
                    break;
            }
            if (GameManager.Instance.unlockedUpgrades.betterPickaxeUpgrade)
            {
                basicPickaxe.SetActive(false);
                betterPickaxe.SetActive(true);
                betterPickaxeRenderer.materials[1] = mat;
            }
            else
            {
                betterPickaxe.SetActive(false);
                basicPickaxe.SetActive(true);
                basicPickaxeRenderer.materials[1] = mat;
            }
            if (GameManager.Instance.unlockedUpgrades.helmetUpgrade)
            {
                headLight.intensity = 4f;
                headLight.range = 15f;
            }
            else
            {
                headLight.intensity = baseLightPower;
                headLight.range = baseLightRange;
            }

            for (int i = 0; i < pickaxeSwings.Length; i++)
            {
                anim.SetFloat("pickaxeSpeed", 1 + 0.7f * pickaxePower - 1);
            }
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
            if (isClimbing && canClimb)
            {
                // Turn off gravity and map the W/S keys to purely vertical movement
                body.useGravity = false;
                Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
                body.linearVelocity = new Vector3(moveDirection.x * moveSpeed, movementInput.y * climbSpeed, moveDirection.z * moveSpeed);
            }
            else
            {
                // Standard walking movement
                body.useGravity = true;
                Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;
                body.linearVelocity = new Vector3(moveDirection.x * moveSpeed, body.linearVelocity.y, moveDirection.z * moveSpeed);
                anim.SetBool("isMoving", body.linearVelocity.magnitude > 1 );
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
            if (other.TryGetComponent<Ladder>(out var ladder)) canClimb = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Ladder>(out var ladder)) 
            {
                canClimb = false;
                isClimbing = false;
            }
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
            xRotation = Mathf.Clamp(xRotation, 0f, 140f);
            playerHead.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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
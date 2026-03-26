using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody body;
        [SerializeField] private PlayerInteractions interactions;
        [Header("Actions")]
        [SerializeField] private InputActionReference moveAction;
        private Vector2 movementInput;

        [Header("Look Settings")] 
        [SerializeField] private float sensitivity = 0.5f;
        [SerializeField] private InputActionReference lookAction;
        private Vector2 lookInput;
        [SerializeField] private float moveSpeed = 5f;
        private float xRotation;

        [Header("Player Stats")]
        [SerializeField] public float maxResistance = 100f;
        private float currentResistance;
        [SerializeField] public int pickaxePower = 1;

        private Transform cam;

        #region Action setup
        void OnEnable() 
        { 
            // Enable the move action and subscribe to its events
            moveAction.action.Enable(); 
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        
            // Now, do the same for the look action
            lookAction.action.Enable();
            lookAction.action.performed += OnLook;
            lookAction.action.canceled += OnLook;
        }
        void OnDisable() 
        { 
            moveAction.action.Disable(); 
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
        
            lookAction.action.Disable();
            lookAction.action.performed -= OnLook;
            lookAction.action.canceled -= OnLook;
        }
        #endregion

        private void Awake()
        {
            GameManager.Instance.SetPlayer(this);
            interactions.controller = this;
        }
        private void Start()
        {
            cam = Camera.main?.transform;
        }
        private void Update()
        {
        
        }
        private void FixedUpdate()
        {
            HandleBodyRotation();

            // Calculate movement direction based on player orientation
            Vector3 moveDirection = (transform.forward * movementInput.y + transform.right * movementInput.x);

            // Then, move
            body.linearVelocity = new Vector3(moveDirection.x * moveSpeed, body.linearVelocity.y, moveDirection.z * moveSpeed);
        }
        private void LateUpdate()
        {
            HandleCameraPitch();
        }

        #region  Input Handling
        public void OnMove(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
        #endregion
    
        #region Helper Functions
        private void HandleBodyRotation()
        {
            if (lookInput.x == 0) return;
            
            Quaternion delta = Quaternion.Euler(0f, lookInput.x * sensitivity, 0f);
            body.MoveRotation(body.rotation * delta);
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
        #endregion
    }
}
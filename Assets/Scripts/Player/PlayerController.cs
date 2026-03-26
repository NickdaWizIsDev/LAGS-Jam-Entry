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
        public float currentResistance;
        [SerializeField] public int pickaxePower = 1;

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
        }
        private void Start()
        {
            cam = Camera.main?.transform;
        }
        private void Update()
        {
            movementInput = moveAction.action.ReadValue<Vector2>();
            lookInput = lookAction.action.ReadValue<Vector2>();
            currentResistance -= Time.deltaTime; // Resistance drains over time, because we're deep underground and it's hard to breathe and all that jazz
        }
        private void FixedUpdate()
        {
            // Calculate movement direction based on player orientation
            Vector3 moveDirection = transform.forward * movementInput.y + transform.right * movementInput.x;

            // Then, move
            body.linearVelocity = new Vector3(moveDirection.x * moveSpeed, body.linearVelocity.y, moveDirection.z * moveSpeed);
        }
        private void LateUpdate()
        {
            if(GameManager.Instance.IsGamePaused) return; // Don't let the player look around if the game is paused
            HandleCameraPitch();
            HandleBodyRotation();
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
        #endregion
    }
}
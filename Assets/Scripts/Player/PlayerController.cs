using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private InputActionReference moveAction;
    private Vector2 movementInput;

    public float moveSpeed = 5f;

    void OnEnable() 
    { 
        moveAction.action.Enable(); 
        moveAction.action.performed += OnMove;
        moveAction.action.canceled += OnMove;
    }
    void OnDisable() 
    { 
        moveAction.action.Disable(); 
        moveAction.action.performed -= OnMove;
        moveAction.action.canceled -= OnMove;
    }

    private void Start()
    {
        GameManager.Instance.SetPlayer(this);
        rb.useGravity = false; // Disable gravity for now
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Move the player based on input
        rb.linearVelocity = new Vector3(movementInput.x, rb.linearVelocity.y, movementInput.y) * moveSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    

    public void ActivateGravity()
    {
        rb.useGravity = true;
    }
}
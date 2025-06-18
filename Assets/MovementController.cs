using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform groundCheck; // Position to check from (e.g., feet)
    [SerializeField] private LayerMask groundLayer;
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _groundCheckRadius = 5f;



    private InputSystem_Actions _inputActions;
    private Vector2 _moveInput = Vector2.zero;
    private bool _jumpRequested = false;
    private bool _isGrounded = false;
    private void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();
        if (_cameraTransform == null)
            _cameraTransform = GetComponent<Transform>();

        _inputActions = new InputSystem_Actions();

        _inputActions.Player.Enable();
    }
    
    private void OnEnable()
    {
        _inputActions.Player.Move.performed += OnMovePerformed;
        _inputActions.Player.Move.canceled += OnMoveCanceled;
        _inputActions.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnJumpPerformed(InputAction.CallbackContext obj)
    {
        if (_isGrounded == true)
        {
            _jumpRequested = true;
        }
        
        
    }
    
    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMovePerformed;
        _inputActions.Player.Move.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext obj)
    {
        Debug.Log(obj.ReadValue<Vector2>());
        _moveInput = obj.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext obj)
    {
        _moveInput = Vector2.zero;
    }
    private void CheckGrounded()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundCheckRadius, groundLayer);
    }



    private void FixedUpdate()
    {   
        CheckGrounded();
        // Get camera-relative directions
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        
        // Flatten to XZ plane
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        // Calculate direction and apply to rigidbody
        Vector3 moveDirection = forward * _moveInput.y + right * _moveInput.x;
        Vector3 velocity = moveDirection * _moveSpeed;
        // Vector3 moveDirection = new Vector3(_moveInput.y, 0, _moveInput.x).normalized;
        // Vector3 velocity = moveDirection * _moveSpeed;


        _rigidbody.linearVelocity = new Vector3(velocity.x, _rigidbody.linearVelocity.y, velocity.z);
        if (_jumpRequested)
        {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z); // reset vertical velocity
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _jumpRequested = false;
        }
    }
}

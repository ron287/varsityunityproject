using UnityEngine;
using UnityEngine.InputSystem;

public class LookController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraPivotTransform;

    [Header("Settings")]
    [SerializeField] private float sensitivity = 1.0f;
    [SerializeField] private float pitchClamp = 80f; // How far up/down you can look

    private InputSystem_Actions _inputActions;
    private Vector2 _deltaPointer = Vector2.zero;

    private float _currentPitch = 0f; // for clamping vertical rotation

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();

        _inputActions.Player.Enable();
    }

    private void Start()
    {
        _inputActions.Player.Look.performed += Look_performed;
    }

    private void Look_performed(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            // Debug.Log(obj.ReadValue<Vector2>());
            _deltaPointer = obj.ReadValue<Vector2>();
        }
    }

    private void Update()
    {
        // Apply sensitivity
        Vector2 lookDelta = _deltaPointer * sensitivity;

        // Horizontal rotation (body)
        transform.Rotate(Vector3.up * lookDelta.x, Space.World);

        // Vertical rotation (camera pivot)
        _currentPitch -= lookDelta.y;
        _currentPitch = Mathf.Clamp(_currentPitch, -pitchClamp, pitchClamp);

        _cameraPivotTransform.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);

        // Reset for next frame
        _deltaPointer = Vector2.zero;
    }
}

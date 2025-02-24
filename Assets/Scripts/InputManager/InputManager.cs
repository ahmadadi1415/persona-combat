using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static bool InteractPressed;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _interactAction;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _interactAction = _playerInput.actions["Interact"];
    }

    void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        InteractPressed = _interactAction.IsPressed();
    }
}

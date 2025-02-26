using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static bool InteractPressed;
    public static bool AttackPressed;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _interactAction, _attackAction;
    private GameManager _gameManager;

    [Inject]
    private void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _interactAction = _playerInput.actions["Interact"];
        _attackAction = _playerInput.actions["Attack"];
    }

    void Update()
    {
        if (_gameManager.CurrentPlayerState == PlayerState.COMBAT || _gameManager.CurrentGameState != GameState.PLAYING) return;

        Movement = _moveAction.ReadValue<Vector2>();
        InteractPressed = _interactAction.WasPressedThisFrame();
        AttackPressed = _attackAction.WasPressedThisFrame();
    }
}

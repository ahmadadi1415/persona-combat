using UnityEngine;

public class PlayerController : MonoBehaviour {
    private PlayerMovement _playerMovement;
    private PlayerInventory _playerInventory;
    private InteractableDetection _interactableDetection;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerInventory = GetComponent<PlayerInventory>();
        _interactableDetection = GetComponentInChildren<InteractableDetection>();
    }

    private void Update() {
        _playerMovement.Move(InputManager.Movement);

        if (InputManager.InteractPressed) {
            _interactableDetection.InteractObject();
        }
    }
}
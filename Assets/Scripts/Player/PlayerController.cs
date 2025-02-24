using UnityEngine;

public class PlayerController : MonoBehaviour {
    private PlayerMovement _playerMovement;
    private InteractableDetection _interactableDetection;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _interactableDetection = GetComponentInChildren<InteractableDetection>();
    }

    private void Update() {
        _playerMovement.Move(InputManager.Movement);

        if (InputManager.InteractPressed) {
            Debug.Log("Interact pressed");
            _interactableDetection.InteractObject();
        }
    }
}
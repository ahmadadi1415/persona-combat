using UnityEngine;

public class InteractableDetection : MonoBehaviour
{
    public bool IsInteractableNearby { get; private set; } = false;
    public IInteractable Interactable { get; private set; } = null;

    public void InteractObject()
    {
        if (IsInteractableNearby && Interactable == null) {
            Interactable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable))
        {
            // DO: Show bubble box
            IsInteractableNearby = true;
            Interactable = interactable;

            interactable.OnTriggerEnter();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Interactable.OnTriggerExit();
        
        IsInteractableNearby = false;
        Interactable = null;
    }
}
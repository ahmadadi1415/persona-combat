using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Item Item { get; private set; }
    private bool isOpened = false;

    public void Interact()
    {
        // DO: Give item to player
        if (!isOpened)
        {
            // DO: Notify to PlayerController to give an item
            EventManager.Publish<OnItemTakenMessage>(new() { Item = Item });
            isOpened = true;
        }
    }

    public void OnTriggerEnter()
    {
        // DO: Notify
    }

    public void OnTriggerExit()
    {
        // DO: Notify 
    }
}
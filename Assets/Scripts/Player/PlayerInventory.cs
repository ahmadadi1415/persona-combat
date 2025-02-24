using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Item Item = null;

    void OnEnable()
    {
        EventManager.Subscribe<OnItemTakenMessage>(OnChestOpened);
    }
    void OnDisable()
    {
        EventManager.Unsubscribe<OnItemTakenMessage>(OnChestOpened);
    }

    private void OnChestOpened(OnItemTakenMessage message)
    {
        Store(message.Item);
    }

    public void Store(Item item)
    {
        Item = item;

        // DO: Notify UI Manager to update Inventory UI
    }
}
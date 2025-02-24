using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour {
    [SerializeField] private Image _itemSlot;

    private void OnEnable()
    {
        EventManager.Subscribe<OnItemTakenMessage>(OnItemTaken);
    }

    private void OnDisable() {
        EventManager.Unsubscribe<OnItemTakenMessage>(OnItemTaken);
    }

    private void OnItemTaken(OnItemTakenMessage message)
    {
        _itemSlot.sprite = message.Item.ItemSprite;
    }
}
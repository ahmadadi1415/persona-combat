using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _bubble;

    public void Interact()
    {
        // DO: Debug.Log
        Debug.Log("NPC Interacted");
    }

    public void OnTriggerEnter()
    {
        // DO: Show Bubble Box
        _bubble.SetActive(true);
    }

    public void OnTriggerExit()
    {
        // DO: Hide Bubble Box
        _bubble.SetActive(false);
    }
}
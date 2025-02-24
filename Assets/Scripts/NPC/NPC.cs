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
        Debug.Log("NPC is nearby");
        _bubble.SetActive(true);
    }

    public void OnTriggerExit()
    {
        // DO: Hide Bubble Box
        Debug.Log("NPC cant be interacted");
        _bubble.SetActive(false);
    }
}
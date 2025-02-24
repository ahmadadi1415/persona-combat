using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        // DO: Debug.Log

        Debug.Log("NPC Interacted");
    }

    public void OnTriggerEnter()
    {
        // DO: Show Bubble Box
        Debug.Log("NPC is nearby");
    }

    public void OnTriggerExit()
    {
        // DO: Hide Bubble Box
        Debug.Log("NPC cant be interacted");
    }
}
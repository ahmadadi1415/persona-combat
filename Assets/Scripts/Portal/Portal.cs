using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        SceneManager.LoadScene("Combat");
    }

    public void OnTriggerEnter()
    {

    }

    public void OnTriggerExit()
    {
        
    }

}
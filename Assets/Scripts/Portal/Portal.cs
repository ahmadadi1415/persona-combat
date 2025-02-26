using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // DO: Load to Combat scene
        SceneManager.LoadScene("Combat");
    }
}
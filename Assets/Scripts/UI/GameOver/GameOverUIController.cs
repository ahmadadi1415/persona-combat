using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour
{
    [SerializeField] private Button _retryButton, _quitButton;

    private void Start()
    {
        _retryButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();

        _retryButton.onClick.AddListener(OnRetryButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        // DO: Back to exploration scene
        SceneManager.LoadScene("Exploration");
    }

    private void OnQuitButtonClicked()
    {
        // DO: Exit
    }
}
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _explorationCanvas, _combatCanvas, _gameOverCanvas;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _combatCanvas.alpha = 0f;
        _gameOverCanvas.alpha = 0f;
        _gameOverCanvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnCombatBattling);
        EventManager.Subscribe<OnGameStateChangedMessage>(OnGameStateChanged);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnCombatBattling);
        EventManager.Unsubscribe<OnGameStateChangedMessage>(OnGameStateChanged);
    }

    private void OnCombatBattling(OnBattlingCombatMessage message)
    {
        _combatCanvas.alpha = message.State == CombatState.END ? 0f : 1f;
    }

    private void OnGameStateChanged(OnGameStateChangedMessage message)
    {
        switch (message.CurrentGameState)
        {
            case GameState.LOSE:
                _gameOverCanvas.alpha = 1f;
                _gameOverCanvas.gameObject.SetActive(true);
                break;
            case GameState.PLAYING:
                _gameOverCanvas.alpha = 0f;
                _gameOverCanvas.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
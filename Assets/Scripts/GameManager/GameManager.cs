using UnityEngine;

public enum PlayerState { EXPLORATION, COMBAT }
public enum GameState { PLAYING, WIN, LOSE }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [field: SerializeField] public PlayerState CurrentPlayerState { get; private set; } = PlayerState.EXPLORATION;
    [field: SerializeField] public GameState CurrentGameState { get; private set; } = GameState.PLAYING;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void OnCombatFinished(OnCombatFinishedMessage message)
    {
        switch (message.Result)
        {
            case CombatResult.WIN:
                // DO: Check is all enemy defeated
                CurrentGameState = GameState.PLAYING;
                break;
            case CombatResult.LOSE:
                CurrentGameState = GameState.LOSE;
                break;
            case CombatResult.FLEE:
                CurrentGameState = GameState.PLAYING;
                break;
        }

        NotifyCurrentGameStateChanged();
    }

    private void NotifyCurrentGameStateChanged()
    {
        EventManager.Publish<OnGameStateChangedMessage>(new() { CurrentGameState = CurrentGameState });
    }
}
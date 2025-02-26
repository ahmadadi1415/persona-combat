using System;
using UnityEngine;

public enum PlayerState { EXPLORATION, COMBAT }
public enum GameState { PLAYING, WIN, LOSE }

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public PlayerState CurrentPlayerState { get; private set; } = PlayerState.EXPLORATION;
    [field: SerializeField] public GameState CurrentGameState { get; private set; } = GameState.PLAYING;

    private void OnEnable()
    {
        EventManager.Subscribe<OnCombatFinishedMessage>(OnCombatFinished);
        EventManager.Subscribe<OnBattlingCombatMessage>(OnCombatBattling);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnCombatFinishedMessage>(OnCombatFinished);
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnCombatBattling);
    }

    private void OnCombatBattling(OnBattlingCombatMessage message)
    {
        switch (message.State)
        {
            case CombatState.INITIALIZATION | CombatState.BATTLING:
                CurrentPlayerState = PlayerState.COMBAT;
                break;
            case CombatState.END:
                CurrentPlayerState = PlayerState.EXPLORATION;
                break;
        }
    }

    private void OnCombatFinished(OnCombatFinishedMessage message)
    {
        switch (message.Result)
        {
            case CombatResult.PLAYER_WIN:
                // DO: Check is all enemy defeated
                CurrentGameState = GameState.PLAYING;
                break;
            case CombatResult.PLAYER_LOSE:
                CurrentGameState = GameState.LOSE;
                break;
            case CombatResult.PLAYER_FLEE:
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
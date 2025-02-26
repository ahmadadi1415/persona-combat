using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatant : Combatant
{
    private MoveData PlayerMove = null;

    private void OnEnable()
    {
        EventManager.Subscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
    }

    private void OnPlayerMoveChoosen(OnPlayerMoveChoosenMessage message)
    {
        PlayerMove = message.Move;
    }

    public override async UniTask<MoveData> GetMoveDataAsync()
    {
        // DO: Wait from player input
        try
        {
            // DO: Notify PlayerTurnInput to allow player input
            EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = true });

            await UniTask.WaitUntil(() => PlayerMove != null);
            
            // DO: Notify PlayerTurnInput to disallow player input
            EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = false });

            await UniTask.WaitForSeconds(1f);
            return PlayerMove;
        }
        catch (System.Exception)
        {
            Debug.Log("Error UniTask");
            return null;
        }
        finally
        {
            // DO: Reset player move after the move is sent
            PlayerMove = null;
        }
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCombatant : Combatant
{
    private ICombatMove PlayerMove = null;

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Subscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Unsubscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
    }

    private void OnPlayerMoveChoosen(OnPlayerMoveChoosenMessage message)
    {
        PlayerMove = message.Move;
        IsMoveReady = true;
    }

    public override async UniTask<ICombatMove> GetMoveDataAsync()
    {
        // DO: Wait from player input
        try
        {
            await UniTask.WaitUntil(() => PlayerMove != null);
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
            IsMoveReady = false;
        }
    }
}
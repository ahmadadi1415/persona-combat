using Cysharp.Threading.Tasks;

public class EnemyCombatant : Combatant
{
    public override async UniTask<ICombatMove> GetMoveDataAsync()
    {
        // DO: Eligible moves are spell and attack
        await UniTask.WaitForSeconds(2);
        IsMoveReady = true;

        int randomIndex = UnityEngine.Random.Range(0, CombatMoves.Count);
        ICombatMove choosenMove = CombatMoves[randomIndex];

        return choosenMove;
    }

}
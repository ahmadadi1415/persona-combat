using Cysharp.Threading.Tasks;

public class EnemyCombatant : Combatant
{
    private bool alreadyHealed = false;
    public override async UniTask<MoveData> GetMoveDataAsync()
    {
        // DO: Eligible moves are spell and attack
        await UniTask.WaitForSeconds(2);
        IsMoveReady = true;

        MoveData attackMove = new()
        {
            MoveType = MoveType.ATTACK,
            Power = 0.8f
        };

        MoveData spellMove = new()
        {
            MoveType = MoveType.SPELL,
            Power = 20
        };

        if (alreadyHealed)
        {
            return attackMove;
        }

        if (Health < 40)
        {
            alreadyHealed = true;
            return spellMove;
        }

        return attackMove;
    }

}
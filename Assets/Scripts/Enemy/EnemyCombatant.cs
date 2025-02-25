using Cysharp.Threading.Tasks;

public class EnemyCombatant : Combatant
{
    public override UniTask<MoveData> GetMoveDataAsync()
    {
        // DO: Only moves are 
        return base.GetMoveDataAsync();
    }

}
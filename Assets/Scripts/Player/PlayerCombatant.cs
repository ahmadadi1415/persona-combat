using Cysharp.Threading.Tasks;

public class PlayerCombatant : Combatant {
    public override UniTask<MoveData> GetMoveDataAsync()
    {
        // DO: Wait from player input
        return base.GetMoveDataAsync();
    }
}
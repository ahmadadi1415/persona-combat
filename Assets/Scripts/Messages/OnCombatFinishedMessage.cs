using System.Collections.Generic;

public struct OnCombatFinishedMessage
{
    public CombatResult Result;
    public List<ICombatant> Combatants;
}
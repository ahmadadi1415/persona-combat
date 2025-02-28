using System.Collections.Generic;

public struct OnCombatStartedMessage
{
    public ICombatant Player;
    public List<ICombatant> Enemies;
    public CombatType CombatType;
}
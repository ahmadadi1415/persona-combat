using System.Collections.Generic;
using UnityEngine;

public struct OnCombatStartedMessage
{
    public ICombatant player;
    public List<ICombatant> enemies;
    public CombatType combatType;
}
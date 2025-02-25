using System.Collections.Generic;

public enum AttackedCharacter { PLAYER, ENEMY }
public struct OnTriggerCombatMessage
{
    public RelativeDirection AttackedDirection;
    public AttackedCharacter AttackedCharacter;
    public List<Combatant> CombatCharacters;
}
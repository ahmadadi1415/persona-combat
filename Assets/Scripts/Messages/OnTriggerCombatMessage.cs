public enum AttackedCharacter { PLAYER, ENEMY }
public struct OnTriggerCombatMessage
{
    public RelativeDirection AttackedDirection;
    public AttackedCharacter AttackedCharacter;
    public ICombatant AttackedCombatant;
    public ICombatant Attacker;
}
using UnityEngine;

public interface IAttackBehavior
{
    public void OnEnemyHit(Combatant attackedCombatant);
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CombatType { NORMAL, ADVANTAGE, AMBUSH }
public enum CombatState { INITIALIZATION, BATTLING, END }
public class CombatManager : MonoBehaviour
{
    [SerializeField] private List<Combatant> combatants = new();
    [field: SerializeField] public bool IsCombating { get; private set; } = false;
    [field: SerializeField] public CombatType CombatType { get; private set; } = CombatType.NORMAL;
    [field: SerializeField] public CombatState CurrentState { get; private set; } = CombatState.END;
    private int currentTurnIndex = 0;

    private void OnEnable()
    {
        EventManager.Subscribe<OnTriggerCombatMessage>(OnTriggerCombat);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnTriggerCombat);
    }

    public void OnTriggerCombat(OnTriggerCombatMessage message)
    {
        if (IsCombating) return;

        CurrentState = CombatState.INITIALIZATION;

        switch (message.AttackedCharacter)
        {
            case AttackedCharacter.ENEMY:
                bool enemyAttackedFromBehind = message.AttackedDirection == RelativeDirection.BEHIND;
                CombatType = enemyAttackedFromBehind ? CombatType.ADVANTAGE : CombatType.NORMAL;
                break;
            case AttackedCharacter.PLAYER:
                CombatType = CombatType.AMBUSH;
                break;
        }
        
        combatants = message.CombatCharacters.OrderByDescending(combatant => combatant.Speed).ToList();
        
        AdjustCombatStatus();

        IsCombating = true;
    }

    private bool IsCombatOver()
    {
        return combatants.Any(combatant => !combatant.GetComponent<ICombatant>().IsAlive);
    }

    private void AdjustCombatStatus()
    {
        
        ICombatant player = combatants.First(combatant => combatant.Name.Equals("Player"));
        ICombatant enemy = combatants.First(combatant => !combatant.Name.Equals("Player"));

        switch (CombatType)
        {
            case CombatType.NORMAL:
                break;
            case CombatType.ADVANTAGE:
                player.SetSpeedModifier(1.5f);
                enemy.TakeDamage(player.Power);
                break;
            case CombatType.AMBUSH:
                player.SetSpeedModifier(0.5f);
                player.TakeDamage(enemy.Power);
                break;
        }
    }

    private void StartCombat()
    {
        CurrentState = CombatState.BATTLING;

        while (!IsCombatOver())
        {
            ICombatant activeCombatant = combatants[currentTurnIndex];
            Debug.Log($"Active Combatant: {activeCombatant.Name}");

        }

    }
}
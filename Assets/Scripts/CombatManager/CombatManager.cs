using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CombatType { NORMAL, ADVANTAGE, AMBUSH }
public enum CombatState { INITIALIZATION, BATTLING, END }
public class CombatManager : MonoBehaviour
{
    [SerializeField] private List<ICombatant> combatants = new();
    [field: SerializeField] public bool IsCombating { get; private set; } = false;
    [field: SerializeField] public CombatType CombatType { get; private set; } = CombatType.NORMAL;
    [field: SerializeField] public CombatState CurrentState { get; private set; } = CombatState.END;
    private int currentTurnIndex = 0;

    private ICombatant playerCombatant, enemyCombatant;
    public ICombatant ActiveCombatant { get; private set; } = null;

    private void OnEnable()
    {
        EventManager.Subscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Subscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Unsubscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnCombatRun(OnCombatRunMessage message)
    {
        IsCombating = false;
    }

    public void OnTriggerCombat(OnTriggerCombatMessage message)
    {
        if (IsCombating) return;


        CurrentState = CombatState.INITIALIZATION;
        IsCombating = true;

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
        NotifyBattlingCombatState();

        StartCombat().Forget();
    }

    private bool IsCombatOver()
    {
        return !IsCombating || combatants.Any(combatant => !combatant.IsAlive);
    }

    private void AdjustCombatStatus()
    {
        playerCombatant = combatants.First(combatant => combatant.Name.Equals("Player"));
        enemyCombatant = combatants.First(combatant => !combatant.Name.Equals("Player"));

        switch (CombatType)
        {
            case CombatType.NORMAL:
                break;
            case CombatType.ADVANTAGE:
                playerCombatant.BuffSpeed(1.5f);
                enemyCombatant.TakeDamage(playerCombatant.Power);
                break;
            case CombatType.AMBUSH:
                playerCombatant.BuffSpeed(0.5f);
                playerCombatant.TakeDamage(enemyCombatant.Power);
                break;
        }
    }

    private async UniTask StartCombat()
    {
        CurrentState = CombatState.BATTLING;

        while (!IsCombatOver())
        {
            ICombatant activeCombatant = combatants[currentTurnIndex];
            Debug.Log($"Active Combatant: {activeCombatant.Name}");

            ICombatant target = activeCombatant == enemyCombatant ? playerCombatant : enemyCombatant;

            MoveData move = await activeCombatant.GetMoveDataAsync();
            activeCombatant.ExecuteMove(move, target);

            NotifyBattlingCombatState();
            if (IsCombatOver()) break;

            currentTurnIndex = (currentTurnIndex + 1) % combatants.Count;
        }

        EndCombat();
    }

    private void NotifyBattlingCombatState()
    {
        EventManager.Publish<OnBattlingCombatMessage>(new() { PlayerCombatant = playerCombatant, EnemyCombatant = enemyCombatant, State = CurrentState });
    }

    private void EndCombat()
    {
        IsCombating = false;
        CurrentState = CombatState.END;
        NotifyBattlingCombatState();
    }
}
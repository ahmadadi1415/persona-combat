using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CombatType { NORMAL, ADVANTAGE, AMBUSH }
public enum CombatState { INITIALIZATION, BATTLING, END }
public enum CombatResult { PLAYER_WIN, PLAYER_LOSE, PLAYER_FLEE }

public class CombatManager : MonoBehaviour
{
    [SerializeField] private List<ICombatant> combatants = new();
    [field: SerializeField] public bool IsCombating { get; private set; } = false;
    [field: SerializeField] public CombatType CombatType { get; private set; } = CombatType.NORMAL;
    [field: SerializeField] public CombatState CurrentState { get; private set; } = CombatState.END;
    private int currentTurnIndex = 0;
    [SerializeField] private int turnCount = 0;

    private ICombatant playerMoveTarget = null;

    private ICombatant playerCombatant;
    public ICombatant ActiveCombatant { get; private set; } = null;

    private void OnEnable()
    {
        EventManager.Subscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Subscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
        EventManager.Subscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Unsubscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
        EventManager.Unsubscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnPlayerMoveChoosen(OnPlayerMoveChoosenMessage message)
    {
        playerMoveTarget = message.PlayerAttackTarget;
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
                playerCombatant = message.Attacker;
                break;
            case AttackedCharacter.PLAYER:
                CombatType = CombatType.AMBUSH;
                playerCombatant = message.AttackedCombatant;
                break;
        }

        Debug.Log($"Combat triggered! Attacked: {message.AttackedCombatant.Name}, Attacker: {message.Attacker}");
        combatants = FindCombatantAround(message.Attacker, message.AttackedCombatant);
        combatants = combatants.OrderByDescending(combatant => combatant.Speed).ToList();

        foreach (var combatant in combatants)
        {
            Debug.Log($"Turn: {combatant.Name}");
        }

        AdjustCombatStatus(playerCombatant, message.AttackedCombatant);

        List<ICombatant> enemies = combatants.Where(combatant => combatant.Name != "Player").ToList();
        EventManager.Publish<OnCombatStartedMessage>(new() { player = playerCombatant, enemies = enemies });
        NotifyBattlingCombatState();

        StartCombat().Forget();
        return;
    }

    private bool IsCombatOver()
    {
        return !IsCombating || combatants.Any(combatant => !combatant.IsAlive);
    }

    private void AdjustCombatStatus(ICombatant playerCombatant, ICombatant attackedEnemyCombatant)
    {
        switch (CombatType)
        {
            case CombatType.NORMAL:
                break;
            case CombatType.ADVANTAGE:
                playerCombatant.AdjustSpeed(1.5f);
                attackedEnemyCombatant.TakeDamage(playerCombatant.Power);
                break;
            case CombatType.AMBUSH:
                playerCombatant.AdjustSpeed(-0.5f);
                playerCombatant.TakeDamage(attackedEnemyCombatant.Power);
                break;
        }
    }

    private async UniTask StartCombat()
    {
        CurrentState = CombatState.BATTLING;

        while (!IsCombatOver())
        {
            // DO: If all already doing the turn, wait the player input again
            if (turnCount % combatants.Count == 0)
            {
                playerMoveTarget = null;

                // DO: Notify PlayerTurnInput to allow player input
                EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = true });

                await UniTask.WaitUntil(() => playerCombatant.IsMoveReady);

                // DO: Notify PlayerTurnInput to disallow player input
                EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = false });
            }

            ICombatant activeCombatant = combatants[currentTurnIndex];
            Debug.Log($"Active Combatant: {activeCombatant.Name}");

            ICombatMove move = await activeCombatant.GetMoveDataAsync();

            bool isPlayerActiveCombatant = activeCombatant.Name == "Player";
            ICombatant target = isPlayerActiveCombatant ? playerMoveTarget : playerCombatant;

            activeCombatant.ExecuteMove(move, target);

            NotifyBattlingCombatState();

            if (IsCombatOver())
            {
                // DO: Check is player died
                if (!playerCombatant.IsAlive)
                {
                    // DO: Notify GameManager, the game ended, player lose
                    EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_LOSE });
                }
                // DO: Check is all enemy died
                else if (combatants.Where(combatant => combatant.Name != "Player").All(enemy => !enemy.IsAlive))
                {
                    // DO: Back to combat exploring, deactivate the enemy object
                    EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_WIN });
                }
                // DO: Both is alive but the combat over, player flee
                else
                {
                    EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_FLEE });
                }
                break;
            }

            currentTurnIndex = (currentTurnIndex + 1) % combatants.Count;
            turnCount++;
        }

        EndCombat();
    }

    private void NotifyBattlingCombatState()
    {
        EventManager.Publish<OnBattlingCombatMessage>(new() { PlayerCombatant = playerCombatant, State = CurrentState });
    }

    private void EndCombat()
    {
        IsCombating = false;
        CurrentState = CombatState.END;
        NotifyBattlingCombatState();
        turnCount = 0;
        playerCombatant = null;
    }

    private List<ICombatant> FindCombatantAround(ICombatant triggerCombatant, ICombatant attackedCombatant)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(((MonoBehaviour)triggerCombatant).transform.position, 1.5f);

        List<ICombatant> combatants = new();
        foreach (Collider2D col in colliders)
        {
            if (col.TryGetComponent<ICombatant>(out var combatant))
            {
                if (!combatants.Contains(combatant)) combatants.Add(combatant);
            }
        }
        return combatants;
    }
}
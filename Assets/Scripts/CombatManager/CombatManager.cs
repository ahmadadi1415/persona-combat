using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CombatType { NORMAL, ADVANTAGE, AMBUSH }
public enum CombatState { INITIALIZATION, BATTLING, END }
public enum CombatResult { PLAYER_WIN, PLAYER_LOSE, PLAYER_FLEE }

public class CombatManager : MonoBehaviour
{
    [field: SerializeField] public bool IsCombating { get; private set; } = false;
    [field: SerializeField] public CombatType CombatType { get; private set; } = CombatType.NORMAL;
    [field: SerializeField] public CombatState CurrentState { get; private set; } = CombatState.END;
    [SerializeField] private List<ICombatant> _combatants = new();
    private int currentTurnIndex = 0;
    [SerializeField] private int turnCount = 0;

    private ICombatant _playerMoveTarget = null;
    private ICombatant _playerCombatant;
    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new();

        EventManager.Subscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Subscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
        EventManager.Subscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnTriggerCombat);
        EventManager.Unsubscribe<OnPlayerMoveChoosenMessage>(OnPlayerMoveChoosen);
        EventManager.Unsubscribe<OnCombatRunMessage>(OnCombatRun);
    }

    private void OnPlayerMoveChoosen(OnPlayerMoveChoosenMessage message)
    {
        _playerMoveTarget = message.PlayerAttackTarget;
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
                _playerCombatant = message.Attacker;
                break;
            case AttackedCharacter.PLAYER:
                CombatType = CombatType.AMBUSH;
                _playerCombatant = message.AttackedCombatant;
                break;
        }

        Debug.Log($"Combat triggered! Attacked: {message.AttackedCombatant.Name}, Attacker: {message.Attacker}");
        _combatants = FindCombatantAround(message.Attacker, message.AttackedCombatant);
        _combatants = CalculateCombatantTurn();

        AdjustCombatStatus(_playerCombatant, message.AttackedCombatant);

        List<ICombatant> enemies = _combatants.Where(combatant => combatant.Name != "Player").ToList();
        EventManager.Publish<OnCombatStartedMessage>(new() { Player = _playerCombatant, Enemies = enemies });

        if (CheckIsCombatOver())
        {
            EndCombat();
            return;
        }

        NotifyBattlingCombatState();

        StartCombat().Forget();
    }

    private List<ICombatant> CalculateCombatantTurn()
    {
        List<ICombatant> sortedCombatantBySpeed = _combatants.Where(combatant => combatant.IsAlive).OrderByDescending(combatant => combatant.Speed).ToList();
        return sortedCombatantBySpeed;
    }

    private bool IsCombatOver()
    {
        bool playerDied = !_playerCombatant.IsAlive;
        bool allEnemiesDied = _combatants.Where(combatant => combatant != _playerCombatant).All(combatant => !combatant.IsAlive);
        return !IsCombating || playerDied || allEnemiesDied;
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
            if (turnCount % _combatants.Count == 0)
            {
                _playerMoveTarget = null;

                // DO: Notify PlayerTurnInput to allow player input
                EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = true });

                await UniTask.WaitUntil(() => _playerCombatant.IsMoveReady, cancellationToken: _cts.Token);

                // DO: Notify PlayerTurnInput to disallow player input
                EventManager.Publish<OnWaitingPlayerTurnInputMessage>(new() { IsTurnInputAllowed = false });
            }

            ICombatant currentCombatant = _combatants[currentTurnIndex];

            // DO: Only alive combatant that can execute move
            if (currentCombatant.IsAlive)
            {
                ICombatMove move = await currentCombatant.GetMoveDataAsync();

                bool isPlayerActiveCombatant = currentCombatant.Name == "Player";
                ICombatant target = isPlayerActiveCombatant ? _playerMoveTarget : _playerCombatant;

                currentCombatant.ExecuteMove(move, target);
            }

            NotifyBattlingCombatState();

            if (CheckIsCombatOver()) break;

            currentTurnIndex = (currentTurnIndex + 1) % _combatants.Count;
            turnCount++;
        }

        EndCombat();
    }

    private bool CheckIsCombatOver()
    {
        if (IsCombatOver())
        {
            // DO: Check is player died
            if (!_playerCombatant.IsAlive)
            {
                // DO: Notify GameManager, the game ended, player lose
                EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_LOSE, Combatants = _combatants });
            }
            // DO: Check is all enemy died
            else if (_combatants.Where(combatant => combatant != _playerCombatant).All(enemy => !enemy.IsAlive))
            {
                // DO: Back to combat exploring, deactivate the enemy object
                EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_WIN, Combatants = _combatants });
            }
            // DO: Both is alive but the combat over, player flee
            else
            {
                EventManager.Publish<OnCombatFinishedMessage>(new() { Result = CombatResult.PLAYER_FLEE, Combatants = _combatants });
            }
            return true;
        }
        return false;
    }

    private void NotifyBattlingCombatState()
    {
        EventManager.Publish<OnBattlingCombatMessage>(new() { PlayerCombatant = _playerCombatant, State = CurrentState });
    }

    private void EndCombat()
    {
        IsCombating = false;
        CurrentState = CombatState.END;
        NotifyBattlingCombatState();
        currentTurnIndex = 0;
        turnCount = 0;
        _playerCombatant = null;
        _combatants.Clear();
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
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour, IAttackBehavior
{
    private Combatant _enemyCombatant;
    private AttackArea _enemyAttackArea;
    private Animator _animator;

    public bool IsCombating { get; private set; } = false;
    public bool CanAttack { get; private set; } = true;
    public bool CanMove { get; private set; } = true;
    public bool IsAttacking { get; private set; } = false;
    [field: SerializeField] public Vector2 FacingDirection { get; private set; } = Vector2.down;

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float triggerRadius = 0.5f;

    private CancellationTokenSource _cts;

    private void Awake()
    {
        _enemyCombatant = GetComponent<EnemyCombatant>();
        _animator = GetComponent<Animator>();
        _enemyAttackArea = GetComponentInChildren<AttackArea>();

        _enemyAttackArea.TargetCombatantTag = "Player";
    }

    private void OnEnable()
    {
        _cts = new();

        EventManager.Subscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Subscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void OnDisable()
    {
        _cts?.Cancel();
        _cts?.Dispose();

        EventManager.Unsubscribe<OnCombatStartedMessage>(OnCombatStarted);
        EventManager.Unsubscribe<OnCombatFinishedMessage>(OnCombatFinished);
    }

    private void Start()
    {
        RandomizeFacingDirection().Forget();
    }

    private void OnCombatStarted(OnCombatStartedMessage message)
    {
        if (!message.enemies.Contains(_enemyCombatant)) return;

        IsCombating = true;
    }

    private void OnCombatFinished(OnCombatFinishedMessage message)
    {
        if (!message.Combatants.Contains(_enemyCombatant)) return;

        IsCombating = false;

        switch (message.Result)
        {
            case CombatResult.PLAYER_WIN:
                gameObject.SetActive(false);
                break;
            case CombatResult.PLAYER_FLEE:
                RandomizeFacingDirection().Forget();
                break;
        }
    }

    private async UniTask RandomizeFacingDirection()
    {
        try
        {
            while (true && !IsCombating)
            {
                await UniTask.WaitForSeconds(3f, cancellationToken: _cts.Token);

                if (_animator == null) break;
                ChangeDirection();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Enemy canceled");
        }

    }

    private void ChangeDirection()
    {
        Vector2[] possibleDirections = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        FacingDirection = possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Length)];

        _animator.SetFloat(AnimationStrings.ANIM_MOVEMENT_LAST_HORIZONTAL, FacingDirection.x);
        _animator.SetFloat(AnimationStrings.ANIM_MOVEMENT_LAST_VERTICAL, FacingDirection.y);
    }

    private RelativeDirection GetAttackDirectionFromPlayer(Combatant playerCombatant)
    {
        PlayerController player = playerCombatant.GetComponent<PlayerController>();
        Vector3 playerPosition = playerCombatant.gameObject.transform.position;
        return CharacterDirection.GetRelativePosition(playerPosition, transform.position, player.FacingDirection);
    }

    public void OnEnemyHit(Combatant attackedCombatant)
    {
        // DO: Enemy only trigger battle if player is ahead
        if (GetAttackDirectionFromPlayer(attackedCombatant) != RelativeDirection.AHEAD) return;

        // DO: Trigger battle
        Debug.Log("Trigger battle from enemy");

        // DO: Start attack animation
        _animator.SetTrigger(AnimationStrings.ANIM_ATTACK);

        // DO: Notify CombatManager to switch to combat state
        EventManager.Publish<OnTriggerCombatMessage>(new()
        {
            AttackedCharacter = AttackedCharacter.PLAYER,
            AttackedDirection = GetAttackDirectionFromPlayer(attackedCombatant),
            AttackedCombatant = attackedCombatant,
            Attacker = _enemyCombatant
        });
    }
}
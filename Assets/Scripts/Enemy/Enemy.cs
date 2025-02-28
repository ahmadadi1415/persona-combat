using System.Collections.Generic;
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

    private void Awake()
    {
        _enemyCombatant = GetComponent<EnemyCombatant>();
        _animator = GetComponent<Animator>();
        _enemyAttackArea = GetComponentInChildren<AttackArea>();

        _enemyAttackArea.TargetCombatantTag = "Player";
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnTriggerCombatMessage>(OnCombatTriggered);
        EventManager.Subscribe<OnCombatFinishedMessage>(OnBattlingCombat);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnCombatTriggered);
        EventManager.Unsubscribe<OnCombatFinishedMessage>(OnBattlingCombat);
    }


    private void Start()
    {
        RandomizeFacingDirection().Forget();
    }

    void Update()
    {
        if (CanAttack && !IsCombating && IsPlayerAhead())
        {
            Attack().Forget();
        }
    }

    private void OnCombatTriggered(OnTriggerCombatMessage message)
    {
        // if (!message.CombatCharacters.Contains(_enemyCombatant)) return;

        IsCombating = true;
    }

    private void OnBattlingCombat(OnCombatFinishedMessage message)
    {
        if (!IsCombating) return;

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

    private async UniTaskVoid RandomizeFacingDirection()
    {
        while (true && !IsCombating)
        {
            await UniTask.WaitForSeconds(3f);
            ChangeDirection();
        }
    }

    private bool IsPlayerAhead()
    {
        bool isPlayerDetected = _enemyAttackArea.OtherCharacterDetected;

        if (!isPlayerDetected) return false;

        Vector3 playerPosition = _enemyAttackArea.AttackedCombatant.gameObject.transform.position;
        bool IsPlayerAhead = CharacterDirection.GetRelativePosition(transform.position, playerPosition, FacingDirection) == RelativeDirection.AHEAD;
        return IsPlayerAhead;
    }

    private void ChangeDirection()
    {
        Vector2[] possibleDirections = { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
        FacingDirection = possibleDirections[Random.Range(0, possibleDirections.Length)];

        _animator.SetFloat(AnimationStrings.ANIM_MOVEMENT_LAST_HORIZONTAL, FacingDirection.x);
        _animator.SetFloat(AnimationStrings.ANIM_MOVEMENT_LAST_VERTICAL, FacingDirection.y);
    }

    private async UniTaskVoid Attack()
    {
        CanAttack = false;

        bool playerDetected = _enemyAttackArea.OtherCharacterDetected;
        if (playerDetected)
        {
            // List<ICombatant> combatants = CheckCombatantAround();
            // if (combatants != null && combatants.Count > 0)
            // {
            //     EventManager.Publish<OnTriggerCombatMessage>(new()
            //     {
            //         AttackedCharacter = AttackedCharacter.PLAYER,
            //         AttackedDirection = GetAttackDirectionFromPlayer(),
            //         CombatCharacters = combatants
            //     });
            // }
            _animator.SetTrigger(AnimationStrings.ANIM_ATTACK);

            await UniTask.WaitForSeconds(_enemyCombatant.AttackCooldown);
            CanAttack = true;
        }
    }


    private List<ICombatant> CheckCombatantAround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f, playerLayer);
        if (colliders.Length > 0)
        {
            List<ICombatant> combatants = new() { _enemyCombatant };
            foreach (var col in colliders)
            {
                if (col.TryGetComponent<ICombatant>(out var enemy))
                {
                    combatants.Add(enemy);
                }
            }

            return combatants;
        }
        return null;
    }

    private RelativeDirection GetAttackDirectionFromPlayer(Combatant playerCombatant)
    {
        PlayerController player = playerCombatant.GetComponent<PlayerController>();
        Vector3 playerPosition = playerCombatant.gameObject.transform.position;
        return CharacterDirection.GetRelativePosition(playerPosition, transform.position, player.FacingDirection);
    }

    public void OnEnemyHit(Combatant attackedCombatant)
    {
        // DO: Trigger battle
        Debug.Log("Trigger battle from enemy");

        // DO: Notify CombatManager to switch to combat state
        EventManager.Publish<OnTriggerCombatMessage>(new()
        {
            AttackedCharacter = AttackedCharacter.ENEMY,
            AttackedDirection = GetAttackDirectionFromPlayer(attackedCombatant),
            AttackedCombatant = attackedCombatant,
            Attacker = _enemyCombatant
        });
    }
}
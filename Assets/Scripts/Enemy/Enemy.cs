using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Combatant _enemyCombatant;
    private AttackArea _enemyAttackArea;
    private Animator _animator;

    public bool IsCombating { get; private set; } = false;
    public bool CanAttack { get; private set; } = true;
    public bool CanMove { get; private set; } = true;
    public bool IsAttacking { get; private set; } = false;
    [field: SerializeField] public Vector2 FacingDirection { get; private set; } = Vector2.down;

    private void Awake()
    {
        _enemyCombatant = GetComponent<EnemyCombatant>();
        _animator = GetComponent<Animator>();
        _enemyAttackArea = GetComponentInChildren<AttackArea>();
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
        if (!message.CombatCharacters.Contains(_enemyCombatant)) return;

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
        bool isPlayerDetected = _enemyAttackArea.OtherCharacterDetected && _enemyAttackArea.DetectedCharacter.gameObject.CompareTag("Player");

        if (!isPlayerDetected) return false;

        Vector3 playerPosition = _enemyAttackArea.DetectedCharacter.gameObject.transform.position;
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

        bool playerDetected = _enemyAttackArea.DetectedCharacter != null;
        if (playerDetected)
        {
            EventManager.Publish<OnTriggerCombatMessage>(new()
            {
                AttackedCharacter = AttackedCharacter.PLAYER,
                AttackedDirection = GetAttackDirectionFromPlayer(),
                CombatCharacters = new() { _enemyAttackArea.DetectedCharacter, _enemyCombatant }
            });
        }
        _animator.SetTrigger(AnimationStrings.ANIM_ATTACK);

        await UniTask.WaitForSeconds(_enemyCombatant.AttackCooldown);
        CanAttack = true;
    }

    private RelativeDirection GetAttackDirectionFromPlayer()
    {
        Combatant playerCharacter = _enemyAttackArea.DetectedCharacter;
        PlayerController player = playerCharacter.GetComponent<PlayerController>();
        Vector3 playerPosition = playerCharacter.gameObject.transform.position;
        return CharacterDirection.GetRelativePosition(playerPosition, transform.position, player.FacingDirection);
    }
}
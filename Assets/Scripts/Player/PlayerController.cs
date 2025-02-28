using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour, IAttackBehavior
{
    private Combatant _playerCombatant;
    private PlayerMovement _playerMovement;
    private PlayerInventory _playerInventory;
    private AttackArea[] _playerAttackArea;
    private InteractableDetection _interactableDetection;
    private Animator _animator;

    public bool CanAttack { get; private set; } = true;
    public bool CanMove { get; private set; } = true;
    public bool IsAttacking { get; private set; } = false;
    public Vector2 FacingDirection => _playerMovement.FacingDirection;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerInventory = GetComponent<PlayerInventory>();
        _playerCombatant = GetComponent<PlayerCombatant>();
        _animator = GetComponent<Animator>();
        _interactableDetection = GetComponentInChildren<InteractableDetection>();
        _playerAttackArea = GetComponentsInChildren<AttackArea>();
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnTriggerCombatMessage>(OnCombatTriggered);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnTriggerCombatMessage>(OnCombatTriggered);
    }

    private void Update()
    {
        _playerMovement.Move(CanMove ? InputManager.Movement : Vector2.zero);

        if (InputManager.InteractPressed)
        {
            _interactableDetection.InteractObject();
        }

        if (InputManager.AttackPressed && CanAttack)
        {
            Attack().Forget();
        }
    }

    private void OnCombatTriggered(OnTriggerCombatMessage message)
    {
        // if (!message.CombatCharacters.Contains(_playerCombatant)) return;
    }


    private async UniTaskVoid Attack()
    {
        CanAttack = false;
        CanMove = false;

        _animator.SetTrigger(AnimationStrings.ANIM_ATTACK);

        await UniTask.WaitForSeconds(0.5f);
        CanMove = true;

        await UniTask.WaitForSeconds(_playerCombatant.AttackCooldown);
        CanAttack = true;
    }

    // private List<ICombatant> CheckCombatantAround()
    // {
    //     Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, triggerRadius, enemyLayer);
    //     if (colliders.Length > 0)
    //     {
    //         List<ICombatant> combatants = new() { _playerCombatant };
    //         foreach (var col in colliders)
    //         {
    //             if (col.TryGetComponent<ICombatant>(out var enemy))
    //             {
    //                 combatants.Add(enemy);
    //             }
    //         }

    //         return combatants;
    //     }
    //     return null;
    // }

    private RelativeDirection GetAttackDirectionFromEnemy(Combatant enemyCombatant)
    {
        Combatant enemyCharacter = enemyCombatant;
        Enemy enemy = enemyCharacter.gameObject.GetComponent<Enemy>();
        Vector3 enemyPosition = enemyCharacter.gameObject.transform.position;
        return CharacterDirection.GetRelativePosition(enemyPosition, transform.position, enemy.FacingDirection);
    }

    public void OnEnemyHit(Combatant attackedCombatant)
    {
        // DO: Trigger Combat
        // Note: Combat Manager should check is any combating happened or not
        Debug.Log("Trigger battle from player");

        // DO: Notify CombatManager to switch to combat state
        EventManager.Publish<OnTriggerCombatMessage>(new()
        {
            AttackedCharacter = AttackedCharacter.ENEMY,
            AttackedDirection = GetAttackDirectionFromEnemy(attackedCombatant),
            AttackedCombatant = attackedCombatant,
            Attacker = _playerCombatant
        });
    }
}
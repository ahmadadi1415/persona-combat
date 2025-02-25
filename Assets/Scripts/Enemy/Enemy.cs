using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Character _character;
    private AttackArea _enemyAttackArea;
    private Animator _animator;

    public bool CanAttack { get; private set; } = true;
    public bool CanMove { get; private set; } = true;
    public bool IsAttacking { get; private set; } = false;
    [field: SerializeField] public Vector2 FacingDirection { get; private set; } = Vector2.down;
    private readonly int _attackAnim = Animator.StringToHash("Attack");
    private readonly int _lastVerticalAnim = Animator.StringToHash("LastVertical");
    private readonly int _lastHorizontalAnim = Animator.StringToHash("LastHorizontal");


    private void Awake()
    {
        _character = GetComponent<Character>();
        _animator = GetComponent<Animator>();
        _enemyAttackArea = GetComponentInChildren<AttackArea>();
    }

    private void Start()
    {
        RandomizeFacingDirection().Forget();
    }

    void Update()
    {
        if (CanAttack && IsPlayerAhead())
        {
            Attack().Forget();
        }
    }

    private async UniTaskVoid RandomizeFacingDirection()
    {
        while (true)
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

        _animator.SetFloat(_lastHorizontalAnim, FacingDirection.x);
        _animator.SetFloat(_lastVerticalAnim, FacingDirection.y);
    }

    private async UniTaskVoid Attack()
    {
        CanAttack = false;
        CanMove = false;

        _enemyAttackArea.DetectedCharacter?.TakeDamage(_character.Model.BasePower);
        _animator.SetTrigger(_attackAnim);

        await UniTask.WaitForSeconds(0.5f);
        CanMove = true;

        await UniTask.WaitForSeconds(_character.AttackCooldown);
        CanAttack = true;
    }
}
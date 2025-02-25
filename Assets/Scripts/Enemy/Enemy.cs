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
        if (CanAttack && _enemyAttackArea.OtherCharacterDetected && _enemyAttackArea.DetectedCharacter.gameObject.CompareTag("Player"))
        {
            Attack().Forget();
        }
    }

    private async UniTaskVoid RandomizeFacingDirection()
    {
        while (true)
        {
            await UniTask.WaitForSeconds(2f);
            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        int randomX = Random.Range(-1, 2);
        int randomY = Random.Range(-1, 2);

        _animator.SetFloat(_lastHorizontalAnim, randomX);
        _animator.SetFloat(_lastVerticalAnim, randomY);
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
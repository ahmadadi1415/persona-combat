using Cysharp.Threading.Tasks;
using UnityEngine;

public class Combatant : MonoBehaviour, ICombatant
{
    [field: SerializeField] public float AttackCooldown { get; private set; } = 1.5f;
    [SerializeField] private CharacterStats stats;

    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int Speed { get; private set; }
    [field: SerializeField] public int Power { get; private set; }
    [field: SerializeField] public int Defense { get; private set; }
    [field: SerializeField] public float SpeedModifier { get; private set; } = 1;

    protected Animator _animator;
    private readonly int _attackAnim = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Health = stats.BaseHealth;
        Name = stats.Name;
        ResetAttributes();
    }

    private void OnEnable() {
        EventManager.Subscribe<OnCombatEndedMessage>(OnCombatEnded);
    }

    private void OnDisable() {
        EventManager.Unsubscribe<OnCombatEndedMessage>(OnCombatEnded);
    }

    private void OnCombatEnded(OnCombatEndedMessage message)
    {
        // DO: Reset speed modifier
        SpeedModifier = 1;
    }

    private void ResetAttributes()
    {
        Speed = stats.BaseSpeed;
        Power = stats.BasePower;
        Defense = stats.BaseDefense;
        SpeedModifier = 1;
    }

    public void PerformAttack()
    {
        _animator.SetTrigger(_attackAnim);
    }

    public void BuffSpeed(float speedModifier)
    {
        if (speedModifier == 0) return;
        SpeedModifier = speedModifier;
    }

    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;
        int damageTaken = Mathf.Clamp(damage - Defense, 1, damage);
        Health -= damageTaken;
    }

    public void Heal(int heal)
    {
        if (Health == stats.BaseHealth || Health <= 0) return;

        Health += heal;
    }

    public void BuffDefense(float buffDefensePercentage)
    {
        Defense += Mathf.FloorToInt(stats.BaseDefense * buffDefensePercentage);
    }

    public virtual async UniTask<MoveData> GetMoveDataAsync() { return null; }

    public void ExecuteMove(MoveData moveData, ICombatant target)
    {
        switch (moveData.MoveType)
        {
            case MoveType.ATTACK:
                PerformAttack();
                target.TakeDamage((int)(moveData.Power * Power));
                break;
            case MoveType.DEFEND:
                BuffDefense(moveData.Power);
                break;
            case MoveType.SPELL:
                Heal((int)moveData.Power);
                break;
            case MoveType.RUN:
                // DO: Notify CombatManager to stop combating
                NotifyRunFromCombat();
                break;
        }
    }

    private void NotifyRunFromCombat()
    {
        EventManager.Publish<OnCombatRunMessage>(new());
    }
}
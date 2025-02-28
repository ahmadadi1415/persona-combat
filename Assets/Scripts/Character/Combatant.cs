using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
    [field: SerializeField] public CombatantState State { get; private set; } = CombatantState.NORMAL;
    [field: SerializeField] public bool IsMoveReady { get; protected set; } = false;
    [field: SerializeField] public List<ScriptableObject> CombatMoveObjects = new();
    [field: SerializeField] public List<ICombatMove> CombatMoves { get; private set; } = new();

    protected Animator _animator;
    private readonly int _attackAnim = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Health = stats.BaseHealth;
        Name = stats.Name;
        ResetAttributes();

        foreach (ScriptableObject moveObject in CombatMoveObjects)
        {
            ICombatMove combatMove = moveObject as ICombatMove;
            CombatMoves.Add(combatMove);
        }
    }

    private void OnEnable()
    {
        EventManager.Subscribe<OnBattlingCombatMessage>(OnCombatEnded);
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<OnBattlingCombatMessage>(OnCombatEnded);
    }

    private void OnCombatEnded(OnBattlingCombatMessage message)
    {
        // DO: Reset speed modifier
        if (message.State == CombatState.END)
        {
            SpeedModifier = 1;
        }
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

    public void PerformDefend()
    {
        LeanTween.scaleX(gameObject, 2f, 0.5f).setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.scaleX(gameObject, 1f, 0.5f).setEase(LeanTweenType.easeInOutSine);
            });
    }

    public void PerformSpell()
    {
        LeanTween.rotateY(gameObject, 180f, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.rotateY(gameObject, 360, 0.5f).setEase(LeanTweenType.easeInOutSine);
            });
    }

    public void BuffSpeed(float speedModifier)
    {
        if (speedModifier == 0) return;
        SpeedModifier += speedModifier;
    }

    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;

        int damageTaken = damage;
        if (State == CombatantState.DEFEND)
        {
            damageTaken = (100 - Defense) / 100 * damageTaken;
        }

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

    public virtual async UniTask<ICombatMove> GetMoveDataAsync() { return null; }

    public void ExecuteMove(ICombatMove move, ICombatant target)
    {
        switch (move.MoveType)
        {
            case MoveType.RUN:
                // DO: Notify CombatManager to stop combating
                NotifyRunFromCombat();
                break;
            default:
                move.Execute(this, target);
                break;
        }
    }

    private void NotifyRunFromCombat()
    {
        EventManager.Publish<OnCombatRunMessage>(new());
    }

    public override bool Equals(object obj)
    {
        if (obj is ICombatant other)
            return Name == other.Name; // Compare by Name (or another unique property)
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}
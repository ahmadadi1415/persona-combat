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
    [field: SerializeField] public float SpeedModifier { get; private set; }

    private void Awake()
    {
        Health = stats.BaseHealth;
        Name = stats.Name;
        ResetAttributes();
    }

    private void ResetAttributes()
    {
        Speed = stats.BaseSpeed;
        Power = stats.BasePower;
        Defense = stats.BaseDefense;
    }

    public void SetSpeedModifier(float speedModifier)
    {
        if (speedModifier == 0) return;
        SpeedModifier = speedModifier;
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = damage - Defense;
        Health -= damageTaken;
    }

    public void BuffDefense(int defense)
    {
        Defense += defense;
    }

    public virtual async UniTask<MoveData> GetMoveDataAsync()
    {
        return new();
    }

    public void ExecuteMove(MoveData moveData, ICombatant target)
    {
        throw new System.NotImplementedException();
    }
}
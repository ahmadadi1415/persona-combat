
using Cysharp.Threading.Tasks;

public interface ICombatant
{
    public string Name { get; }
    public int Health { get; }
    public bool IsAlive => Health > 0;
    public int Speed { get; }
    public int Power { get; }
    public int Defense { get; }
    public float SpeedModifier { get; }

    public float CurrentSpeed => Speed * SpeedModifier;
    public void TakeDamage(int damage);
    public void SetSpeedModifier(float speedModifier);

    public UniTask<MoveData> GetMoveDataAsync();
    public void ExecuteMove(MoveData moveData, ICombatant target);
}
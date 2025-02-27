
using Cysharp.Threading.Tasks;

public enum CombatantState { NORMAL, DEFEND }

public interface ICombatant
{
    public string Name { get; }
    public int Health { get; }
    public bool IsAlive => Health > 0;
    public int Speed { get; }
    public int Power { get; }
    public int Defense { get; }
    public float SpeedModifier { get; }
    public CombatantState State { get; }
    public bool IsMoveReady { get; }

    public float CurrentSpeed => Speed * SpeedModifier;
    public void TakeDamage(int damage);
    public void Heal(int heal);
    public void BuffSpeed(float buffSpeedPercentage);
    public void BuffDefense(float buffDefensePercentage);

    public UniTask<MoveData> GetMoveDataAsync();
    public void ExecuteMove(MoveData moveData, ICombatant target);
}
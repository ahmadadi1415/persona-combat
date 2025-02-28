public enum TargetType { SELF, ALLY, ENEMY, ANY }
public interface ICombatMove
{
    public string MoveName { get; }
    public string Description { get; }
    public MoveType MoveType { get; }
    public TargetType TargetType { get; }

    public void Execute(ICombatant source, ICombatant target);
}
using UnityEngine;

[CreateAssetMenu(menuName = "Combat Moves/Attack Move")]
public class AttackCombatMove : ScriptableObject, ICombatMove
{
    [field: SerializeField] public string MoveName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField][field: Range(0, 2)] public float AttackPower { get; private set; }
    [field: SerializeField][field: Range(0, 2)] public float SpeedBoost { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }
    public MoveType MoveType { get; protected set; } = MoveType.ATTACK;

    public void Execute(ICombatant source, ICombatant target)
    {
        target.TakeDamage((int)(AttackPower * source.Power));
        source.AdjustSpeed(SpeedBoost);
    }
}
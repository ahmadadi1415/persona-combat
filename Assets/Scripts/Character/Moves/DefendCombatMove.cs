using UnityEngine;

[CreateAssetMenu(menuName = "Combat Moves/Defend Move")]
public class DefendCombatMove : ScriptableObject, ICombatMove
{
    [field: SerializeField] public string MoveName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField][field: Range(0, 1)] public float DamageReduction { get; private set; }
    [field: SerializeField] public int DefenseBoost { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }
    public MoveType MoveType { get; protected set; } = MoveType.DEFEND;

    public void Execute(ICombatant source, ICombatant target)
    {
        target.BuffDefense(DefenseBoost);
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Combat Moves/Heal Move")]
public class HealCombatMove : ScriptableObject, ICombatMove
{
    [field: SerializeField] public string MoveName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField][field: Min(0)] public int HealAmount { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }
    public MoveType MoveType { get; protected set; } = MoveType.SPELL;

    public void Execute(ICombatant source, ICombatant target)
    {
        target.Heal(HealAmount);
    }
}
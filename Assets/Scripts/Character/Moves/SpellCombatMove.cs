using UnityEngine;

[CreateAssetMenu(menuName = "Combat Moves/Spell Move")]
public class SpellCombatMove : ScriptableObject, ICombatMove
{
    [field: SerializeField] public string MoveName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField][field: Range(0, 1)] public float MagicPower { get; private set; }
    [field: SerializeField][field: Range(0, 2)] public int SpeedBoost { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }
    public MoveType MoveType { get; protected set; } = MoveType.SPELL;

    public void Execute(ICombatant source, ICombatant target)
    {
        // DO: Target take damage, Source gain speed
        target.TakeDamage((int)(MagicPower * source.Power));
        source.AdjustSpeed(SpeedBoost);
    }
}
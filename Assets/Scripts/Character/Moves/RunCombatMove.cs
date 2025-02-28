using UnityEngine;

[CreateAssetMenu(menuName = "Combat Moves/Run Move")]
public class RunCombatMove : ScriptableObject, ICombatMove
{
    [field: SerializeField] public string MoveName { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public TargetType TargetType { get; private set; }
    public MoveType MoveType { get; private set; } = MoveType.RUN;

    public void Execute(ICombatant source, ICombatant target)
    {
        throw new System.NotImplementedException();
    }
}
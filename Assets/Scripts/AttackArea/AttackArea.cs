using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [field: SerializeField] public Combatant AttackedCombatant { get; private set; }
    [field: SerializeField] public string TargetCombatantTag { get; set; } = string.Empty;
    public bool OtherCharacterDetected => AttackedCombatant != null;
    private IAttackBehavior _attackBehavior;

    private void Awake() {
        _attackBehavior = GetComponentInParent<IAttackBehavior>();
    }

    private void OnDisable()
    {
        AttackedCombatant = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(TargetCombatantTag)) return;

        if (collision.TryGetComponent(out Combatant combatant))
        {
            AttackedCombatant = combatant;
            _attackBehavior.OnEnemyHit(AttackedCombatant);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(TargetCombatantTag)) return;

        AttackedCombatant = null;
    }
}
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [field: SerializeField] public Combatant DetectedCharacter { get; private set; }
    public bool OtherCharacterDetected => DetectedCharacter != null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Combatant character))
        {
            DetectedCharacter = character;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        DetectedCharacter = null;
    }
}
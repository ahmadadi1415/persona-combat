using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [field: SerializeField] public Character DetectedCharacter { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Character>(out Character character))
        {
            DetectedCharacter = character;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        DetectedCharacter = null;
    }
}
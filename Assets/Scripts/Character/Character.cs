using UnityEngine;

public class Character : MonoBehaviour
{
    [field: SerializeField] public CharacterModel Model { get; private set; } = new();
    [field: SerializeField] public float AttackCooldown { get; private set; } = 1.5f;

    public void TakeDamage(int damage, RelativeDirection attackedDirection)
    {
        Model.TakeDamage(damage);

        Debug.Log($"{gameObject.name} take damage from {attackedDirection}");
    }

    public void SetSpeedModifier(float speedModifier)
    {
        Model.SetSpeedModifier(speedModifier);
    }

}
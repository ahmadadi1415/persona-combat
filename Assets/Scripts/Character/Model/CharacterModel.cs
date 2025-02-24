[System.Serializable]
public class CharacterModel
{
    public int Health { get; private set; }
    public bool IsAlive => Health > 0;
    public int BaseSpeed { get; private set; }
    public int BasePower { get; private set; }
    public int BaseDefense { get; private set; }

    public float SpeedModifier { get; private set; } = 1f;

    public float CurrentSpeed => BaseSpeed * SpeedModifier;
    public void TakeDamage(int damage)
    {
        Health -= damage;
    }

    public void SetSpeedModifier(float speedModifier)
    {
        if (speedModifier == 0f) return;
        SpeedModifier = speedModifier;
    }
}
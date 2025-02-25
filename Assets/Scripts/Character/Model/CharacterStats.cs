using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public string Name;
    public int BaseHealth;
    public int BaseSpeed;
    public int BasePower;
    public int BaseDefense;
}
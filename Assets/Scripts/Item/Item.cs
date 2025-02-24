using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Object")]
public class Item : ScriptableObject
{
    public Sprite ItemSprite;
    public string Name;
}
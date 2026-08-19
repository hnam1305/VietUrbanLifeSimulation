using UnityEngine;

public enum ItemRarity
{
    Normal,
    Good,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [TextArea(3, 5)]
    public string description;

    public ItemRarity rarity;

    public bool isStackable;
    public int maxStack;
}
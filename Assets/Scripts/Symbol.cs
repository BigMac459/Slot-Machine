using UnityEngine;

public enum SymbolType
{
    Jackpot, Bell, BAR, Fruit
}

[CreateAssetMenu(menuName = "Symbol")]
public class Symbol : ScriptableObject
{
    public Sprite sprite;
    public SymbolType type = SymbolType.Jackpot;

    // Number of bars for Bar symbols. For non-Bar symbols, this value is ignored.
    public int noOfBars = 0;
}

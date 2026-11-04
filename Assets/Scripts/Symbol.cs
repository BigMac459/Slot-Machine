using UnityEngine;

public enum SymbolType
{
    Jackpot, Bell, Bar, Fruit
}

[CreateAssetMenu(menuName = "Symbol")]
public class Symbol : ScriptableObject
{
    public Sprite sprite;
    public SymbolType type = SymbolType.Jackpot;

    // Number of bars for Bar symbols. For non-Bar symbols, this value is ignored.
    public int noOfBars = 0;

    [Min(1)] public int noOfStops = 10;
}

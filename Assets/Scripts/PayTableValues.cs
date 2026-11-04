using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SymbolMultipliers
{
    public Symbol symbol;
    public float multiplier;
}

[CreateAssetMenu(menuName = "Pay Table")]
public class PayTableValues : ScriptableObject
{
    [Header("Multipliers")]
    public List<SymbolMultipliers> threeMatches;
    public float twoBells;
    public float twoTripleBars;
    public float mixedBars;
    public float twoDoubleBars;
    public float twoSingleBars;
    public float twoIdenticalFruits;
    public float mixedFruits;

    public float GetPayoutMultiplier(List<SlotReel> reels)
    {
        Symbol[] results = new Symbol[3];
        for (int i = 0; i < reels.Count; i++)
            results[i] = reels[i].MiddleSymbol;
        Symbol first = results[0], second = results[1], third = results[2];

        // Three identical symbols
        if (first == second && second == third)
            return threeMatches.Find(x => x.symbol == first).multiplier;

        int bells = 0, bars = 0, fruits = 0;
        int singles = 0, doubles = 0, triples = 0;

        foreach (var s in results)
            switch (s.type)
            {
                case SymbolType.Bell: bells++; break;
                case SymbolType.Bar:
                    bars++;
                    switch (s.noOfBars)
                    {
                        case 3: triples++; break;
                        case 2: doubles++; break;
                        case 1: singles++; break;
                    }
                    break;
                case SymbolType.Fruit: fruits++; break;
            }

        // Two bells
        if (bells == 2) return twoBells;

        // Mixed bars
        if (bars == 3) return mixedBars;
        else if (bars == 2) {
            if (triples == 2) return twoTripleBars;
            if (doubles == 2) return twoDoubleBars;
            if (singles == 2) return twoSingleBars;
        }

        // Mixed fruits
        if (fruits == 3) return mixedFruits;
        else if (fruits == 2)
            if (first == second || second == third || first == third)
                return twoIdenticalFruits;
        return 0f;
    }
}

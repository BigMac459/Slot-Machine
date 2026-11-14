using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotSymbol
{
    public Symbol symbol;
    [Min(1)] public int noOfStops = 10;
    [Min(0)] public float multiplier;
}

[CreateAssetMenu(menuName = "Slot Machine Settings")]
public class SlotMachineSettings : ScriptableObject
{
    public string presetName;

    [Header("Multipliers")]
    public List<SlotSymbol> threeMatches;
    public float twoBells;
    public float twoTripleBars;
    public float mixedBars;
    public float twoDoubleBars;
    public float twoSingleBars;
    public float twoIdenticalFruits;
    public float mixedFruits;

    private readonly Dictionary<string, int> resultDict = new();

    public float GetPayoutMultiplier(List<SlotReel> reels, out Symbol winningSymbol)
    {
        Symbol[] results = new Symbol[3];
        for (int i = 0; i < reels.Count; i++)
            results[i] = reels[i].MiddleSymbol;
        Symbol first = results[0], second = results[1], third = results[2];

        // Three identical symbols (WIN!)
        if (first == second && second == third)
        {
            winningSymbol = first;
            return threeMatches.Find(x => x.symbol == first).multiplier;
        }

        // Consolations
        winningSymbol = null;

        resultDict.Clear();
        foreach (var s in results)
        {
            switch (s.type)
            {
                case SymbolType.Bell: resultDict["Bell"] = ResultKey("Bell") + 1; break;
                case SymbolType.BAR:
                    resultDict["BAR"] = ResultKey("BAR") + 1; ;
                    switch (s.noOfBars)
                    {
                        case 3: resultDict["Triple"] = ResultKey("Triple") + 1; break;
                        case 2: resultDict["Double"] = ResultKey("Double") + 1; break;
                        case 1: resultDict["Single"] = ResultKey("Single") + 1; break;
                    } break;
                case SymbolType.Fruit: resultDict["Fruit"] = ResultKey("Fruit") + 1; break;
            }
        }

        // Two bells
        if (ResultKey("Bell") == 2) return twoBells;

        // Mixed bars
        if (ResultKey("BAR") == 3) return mixedBars;
        else if (ResultKey("BAR") == 2) {
            if (ResultKey("Triple") == 2) return twoTripleBars;
            if (ResultKey("Double") == 2) return twoDoubleBars;
            if (ResultKey("Single") == 2) return twoSingleBars;
        }

        // Mixed fruits
        if (ResultKey("Fruit") == 3) return mixedFruits;
        else if (ResultKey("Fruit") == 2)
            if (first == second || second == third || first == third)
                return twoIdenticalFruits;
        return 0f;
    }

    public int ResultKey(string key)
    {
        return resultDict.GetValueOrDefault(key, 0);
    }
}

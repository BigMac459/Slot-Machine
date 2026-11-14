using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ThreeMatchRow : PayTableRow
{
    [SerializeField] private List<Image> symbolImages;

    public void SetRow(Symbol symbol, float multiplier)
    {
        foreach (var image in symbolImages)
            image.sprite = symbol.sprite;
        SetMultiplier(multiplier);
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ReelStop : MonoBehaviour
{
    public Symbol CurrentSymbol { get; private set; }

    public void SetSymbol(Symbol symbol)
    {
        CurrentSymbol = symbol;
        GetComponent<SpriteRenderer>().sprite = symbol.sprite;
    }    
}

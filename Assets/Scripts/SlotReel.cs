using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotReel : MonoBehaviour
{
    // Speed range for each reel. Actual speed is random for each reel between this range.
    [SerializeField, Min(0.1f)] private float minSpeed;
    [SerializeField, Min(0.1f)] private float maxSpeed;

    [SerializeField] private Symbol startingSymbol;

    private readonly List<ReelStop> stops = new();
    private readonly List<Symbol> sequence = new();
    private Vector3[] stopPositions;

    private int nextSymbolIndex;
    private int middleIndex;

    public bool IsSpinning { get; private set; }

    public Symbol MiddleSymbol => stops[stops.Count / 2].CurrentSymbol;

    public void Awake()
    {
        foreach (Transform child in transform)
            stops.Add(child.gameObject.GetComponent<ReelStop>());
    }

    public void Initialize(List<SlotSymbol> slotSymbols)
    {
        // Fixed positions of the stops on the reel
        stopPositions = new Vector3[stops.Count];
        for (int i = 0; i < stops.Count; i++)
            stopPositions[i] = stops[i].transform.localPosition;

        IsSpinning = false;

        // Newly-shuffled symbol sequence
        CreateSequence(slotSymbols);

        nextSymbolIndex = 0;
        middleIndex = stops.Count / 2;

        // Fill bottom to top using the stored sequence.
        foreach (var stop in stops) AssignNextSymbol(stop);
    }

    // At the start of every game, the sequence of symbols on the reel is randomized
    private void CreateSequence(List<SlotSymbol> slotSymbols)
    {
        sequence.Clear();
        foreach (SlotSymbol slotSymbol in slotSymbols)
            for (int i = 0; i < slotSymbol.noOfStops; i++)
                sequence.Add(slotSymbol.symbol);

        // Shuffles the sequence
        for (int i = sequence.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (sequence[randomIndex], sequence[i]) = (sequence[i], sequence[randomIndex]);
        }

        // This slot machine will always start with all 7s
        int symbolIndex = sequence.IndexOf(startingSymbol);
        (sequence[2], sequence[symbolIndex]) = (sequence[symbolIndex], sequence[2]);
    }

    private void AssignNextSymbol(ReelStop stop)
    {
        stop.SetSymbol(sequence[nextSymbolIndex]);
        if (++nextSymbolIndex >= sequence.Count) nextSymbolIndex = 0;
    }

    private void PositionStops(float distance)
    {
        for (int i = 0; i < stops.Count; i++)
            stops[i].transform.localPosition =
                stopPositions[i] + Vector3.down * distance;
    }

    public void Spin()
    {
        if (IsSpinning) return;

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        int targetIndex = Random.Range(0, sequence.Count);

        // Count the no. of forward moves needed to reach that position.
        int movesToTarget = (targetIndex - middleIndex + sequence.Count) % sequence.Count;

        // Do 1 full rotation before stopping at the target symbol.
        int totalMoves = (movesToTarget < 60 ? sequence.Count : 0) + movesToTarget;

        IsSpinning = true;
        StartCoroutine(SpinCoroutine(totalMoves, randomSpeed));
    }

    private IEnumerator SpinCoroutine(int totalMoves, float spinSpeed)
    {
        float spacing = stopPositions[0].y - stopPositions[1].y;
        float distance = 0f;
        while (totalMoves > 0)
        {
            distance += spinSpeed * Time.deltaTime;
            while (distance >= spacing && totalMoves > 0)
            {
                distance -= spacing;
                CompleteMove();
                totalMoves--;
            }
            PositionStops(totalMoves == 0? 0f:distance);
            yield return null;
        }
        IsSpinning = false;
    }

    private void CompleteMove()
    {
        // Recycle the object that has passed below the window.
        ReelStop bottomStop = stops[^1];

        for (int i = stops.Count - 1; i > 0; i--) stops[i] = stops[i - 1];

        stops[0] = bottomStop;

        AssignNextSymbol(bottomStop);

        PositionStops(0f);
        middleIndex = (middleIndex + 1) % sequence.Count;
    }
}
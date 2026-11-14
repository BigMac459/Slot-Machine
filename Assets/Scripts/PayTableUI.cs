using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class PayTableRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI multiplierText;

    public bool permanent;

    public void SetMultiplier(float multiplier)
    {
        multiplierText.text = $"x{multiplier:0.##}";
    }
}

public class PayTableUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Transform threeMatchParent;
    [SerializeField] private GameObject threeMatchPrefab;
    [SerializeField] private Transform miscParent;
    [SerializeField] private GameObject miscPrefab;

    private void Clear(Transform list)
    {
        foreach (Transform child in list)
        {
            PayTableRow row = child.GetComponent<PayTableRow>();
            if (!row.permanent)
                Destroy(row.gameObject);
        }
    }

    public void Refresh(SlotMachineSettings settings)
    {
        Clear(threeMatchParent);
        Clear(miscParent);

        if (settings)
        {
            label.SetText("PAY TABLE: " + settings.presetName);
            foreach (var match in settings.threeMatches)
            {
                ThreeMatchRow row =
                    Instantiate(threeMatchPrefab, threeMatchParent).GetComponent<ThreeMatchRow>();
                row.SetRow(match.symbol, match.multiplier);
            }
            
            List<MiscRow> miscRows = new();
            for (int i = 0; i < 7; i++)
                miscRows.Add(Instantiate(miscPrefab, miscParent).GetComponent<MiscRow>());
            miscRows[0].SetRow("2 Bells + other", settings.twoBells);
            miscRows[1].SetRow("2 Triple BARs + non-BAR", settings.twoTripleBars);
            miscRows[2].SetRow("2 Double BARs + non-BAR", settings.twoDoubleBars);
            miscRows[3].SetRow("2 Single BARs + non-BAR", settings.twoSingleBars);
            miscRows[4].SetRow("3 Mixed BARs", settings.mixedBars);
            miscRows[5].SetRow("2 Identical Fruits + non-Fruit", settings.twoIdenticalFruits);
            miscRows[6].SetRow("3 Mixed Fruits", settings.mixedFruits);
        }
    }
}

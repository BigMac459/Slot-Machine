using TMPro;
using UnityEngine;

public class MiscRow : PayTableRow
{
    [SerializeField] private TextMeshProUGUI ruleText;

    public void SetRow(string rule, float multiplier)
    {
        ruleText.text = rule;
        SetMultiplier(multiplier);
    }
}

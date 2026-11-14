using System;
using UnityEngine;
using UnityEngine.UI;

public class MachineSettingsLoader : MonoBehaviour
{
    public SlotMachineSettings settings;

    public void LoadSettings()
    {
        SlotMachineController.Instance.CurrentSettings = settings;
        SlotMachineController.Instance.SetupPayTable();
        SlotMachineController.Instance.HomePositions();
    }
}

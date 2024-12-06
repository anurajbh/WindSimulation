using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurbineSelectDropdown : MonoSingleton<TurbineSelectDropdown>
{
    public List<PowerGenerator> turbines = new List<PowerGenerator>();
    public TMP_Dropdown turbineDropdown;

    void Start()
    {
        turbineDropdown = GetComponent<TMP_Dropdown>();

        if (turbineDropdown == null)
        {
            Debug.LogError("No TMP_Dropdown component found on this GameObject!");
            return;
        }

        turbineDropdown.onValueChanged.AddListener(OnTurbineSelected); // Add listener
        UpdateDropdown();
    }

    public void RegisterTurbine(PowerGenerator turbine)
    {
        if (!turbines.Contains(turbine))
        {
            turbines.Add(turbine);
            UpdateDropdown();
        }
    }

    public void DeregisterTurbine(PowerGenerator turbine)
    {
        if (turbines.Contains(turbine))
        {
            turbines.Remove(turbine);
            UpdateDropdown();
        }
    }

    private void UpdateDropdown()
    {
        if (turbineDropdown == null) return;

        turbineDropdown.ClearOptions();
        List<string> turbineNames = new List<string>();

        for (int i = 0; i < turbines.Count; i++)
        {
            turbineNames.Add($"Turbine {i + 1}");
        }

        turbineDropdown.AddOptions(turbineNames);
        // Automatically select the first turbine if none is selected
        if (turbines.Count > 0)
        {
            turbineDropdown.value = 0; // Select the first turbine in the dropdown
            OnTurbineSelected(0);     // Trigger the selection logic
        }
    }

    public void OnTurbineSelected(int index)
    {
        if (index >= 0 && index < turbines.Count)
        {
            MetricDisplay.Instance.SetTargetTurbine(turbines[index]);
        }
    }
}

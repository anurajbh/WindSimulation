using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class MetricDisplay : MonoSingleton<MetricDisplay>
{ 
    [Header("Text References")]
    public TextMeshProUGUI windSpeedText;
    public TextMeshProUGUI turbulenceText;
    public TextMeshProUGUI powerOutputText;
    public TextMeshProUGUI totalEnergyText;
    public TextMeshProUGUI airDensityText;
    public TextMeshProUGUI efficiencyText;
    public TextMeshProUGUI timeElapsedText;

    public PowerGenerator currentTurbine;

    private float startTime;
    private void Start()
    {
        if (TurbineSelectDropdown.Instance.turbines.Count > 0)
        {
            SetTargetTurbine(TurbineSelectDropdown.Instance.turbines[0]);
        }
        startTime = Time.time;
    }
    public void SetTargetTurbine(PowerGenerator turbine)
    {
        if(currentTurbine == turbine)
        {
            return;
        }
        currentTurbine = turbine;
        UpdateMetricsDisplay(); // Update display immediately when turbine changes
    }
    private void Update()
    {
        if (currentTurbine != null)
        {
            UpdateMetricsDisplay();
        }
    }
    private void UpdateMetricsDisplay()
    {
        if (null == WindManager.Instance)
        {
            return;
        }
        WindSettings windSettings = WindManager.Instance.currentWindSettings;
        if (windSettings == null || currentTurbine == null) return;

        windSpeedText.text = $"Wind Speed: {windSettings.GetWindSpeed()} m/s";
        turbulenceText.text = $"Turbulence: {windSettings.windZone.windTurbulence}";
        powerOutputText.text = $"Power Output: {currentTurbine.currentPowerOutput/1000f} kW";
        totalEnergyText.text = $"Total Energy: {currentTurbine.totalEnergyProduced} J";
        airDensityText.text = $"Air Density: {windSettings.GetAirDensity(currentTurbine.transform.position)} kg/m³";
        efficiencyText.text = $"Efficiency: {currentTurbine.efficiency * 100f}%";
        // Update time elapsed
        if (timeElapsedText)
        {
            float elapsedTime = Time.time - startTime;
            timeElapsedText.text = $"Time Elapsed: {FormatElapsedTime(elapsedTime)}";
        }
    }

    private string FormatElapsedTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:D2}:{secs:D2}";
    }
}


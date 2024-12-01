using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PowerGenerator : MonoBehaviour
{
    [Header("Turbine Parameters")]
    public float efficiency = 0.4f; //between 0 and 1

    [Header("Values")]
    public float currentPowerOutput = 0f; //Real-time power output
    public float totalEnergyProduced = 0f;
    [SerializeField] float bladeRadius = 0f; //Radius of turbine blades

    private void Start()
    {
        efficiency = Mathf.Clamp(efficiency, Mathf.Epsilon, 1f);
        bladeRadius = CalculateBladeRadius();
        Debug.Log($"Blade radius calculated: {bladeRadius}");
        if (null == WindManager.Instance)
        {
            Debug.Log("No Wind Manager");
            return;
        }
        WindManager.Instance.WindUpdater += CalculateWindPower;
    }
    private void OnDestroy()
    {
        if (null == WindManager.Instance)
        {
            Debug.Log("No Wind Manager");
            return;
        }
        WindManager.Instance.WindUpdater -= CalculateWindPower;
    
    }
    float CalculateBladeRadius()
    {
        if(transform.childCount == 0)
        {
            Debug.LogError("Attach this script to a turbine parent object! No blades found");
            return 0f;
        }
        float maxRadius = 0f; // To track the largest radius among the children

        foreach (Transform child in transform)
        {
            MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
            Collider collider = child.GetComponent<Collider>();

            if (meshRenderer != null)
            {
                float radius = meshRenderer.bounds.extents.z; // Assuming Z-axis is the blade length
                Debug.Log($"Found MeshRenderer. Calculated radius: {radius}");
                maxRadius = Mathf.Max(maxRadius, radius);
            }
            else if (collider != null)
            {
                float radius = collider.bounds.extents.z; // Assuming Z-axis is the blade length
                Debug.Log($"Found Collider. Calculated radius: {radius}");
                maxRadius = Mathf.Max(maxRadius, radius);
            }
            else
            {
                Debug.LogWarning($"Child {child.name} has no MeshRenderer or Collider. Skipping.");
            }
        }

        if (maxRadius == 0f)
        {
            Debug.LogError("Blade radius could not be calculated. Ensure blades have either MeshRenderer or Collider components.");
        }

        return maxRadius;
    }

    public void CalculateWindPower(float deltaTime)
    {
        WindSettings windSettings = WindManager.Instance.currentWindSettings;
        if( null == windSettings )
        {
            Debug.LogError("No wind settings associated with Wind Manager!");
            currentPowerOutput = 0f;
            return;
        }
        float windSpeed = windSettings.GetWindSpeed();

        float sweptArea = Mathf.PI * Mathf.Pow(bladeRadius, 2); // A=pi*r*r

        float airDensity = windSettings.GetAirDensity(transform.position);

        currentPowerOutput = 0.5f * airDensity * sweptArea * Mathf.Pow(windSpeed, 3) * efficiency;

        totalEnergyProduced += currentPowerOutput * deltaTime;
        Debug.Log($"Current air density : {airDensity} Power: {currentPowerOutput} W, Total Energy prduced : {totalEnergyProduced} J");

    }
    public float GetPowerOutput(float power)
    {
        return currentPowerOutput; 
    }
}

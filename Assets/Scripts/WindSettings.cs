using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu (fileName = "Wind Settings", menuName = "Wind/Wind Settings")]
public class WindSettings : ScriptableObject
{
    [Header("Wind Zone reference")]
    public WindZone windZone;

    [Header("Default Wind Settings")]
    public float defaultWindSpeed = 2.0f;
    public float defaultTurbulence = 0.5f;
    public WindZoneMode windMode = WindZoneMode.Directional;
    public float maxWindSpeed = 30.0f;
    public float minWindSpeed = 0.0f;
    public float maxTurbulence = 30.0f;
    public float minTurbulence = 0.0f;

    [Header("Default Noise Weight percentage (Sum is always 1)")]
    public float gaussianWeight = 0.5f;
    public float perlinWeight = 0.3f;
    public float wfcWeight = 0.2f;

    [Header("Range of variation for noise values")]
    public float variationRange = 2.0f;

    [Header("Randomization Settings")]
    public float randomSpeedRange = 1.0f;
    public float randomTurbulenceRange = 0.5f;

    void OnValidate()
    {
        float total = gaussianWeight + perlinWeight + wfcWeight;
        if(Mathf.Abs(total-1f) >Mathf.Epsilon)
        {
            if (total > Mathf.Epsilon)
            {
                // Normalize weights if total is valid
                gaussianWeight /= total;
                perlinWeight /= total;
                wfcWeight /= total;
            }
            else
            {
                // Assign equal weightage if total is 0 to each noise type
                gaussianWeight = 1f / 3f;
                perlinWeight = 1f / 3f;
                wfcWeight = 1f / 3f;
            }
        }
    }
    public void EnsureSubscribed()
    {
        if (windZone == null)
        {
            InitWindZone();
        }
        if (null == WindManager.Instance)
        {
            Debug.LogError("Failed to subscribe! Wind Manager Instance is null");
        }
        WindManager.Instance.WindUpdater -= ApplyWindDynamically;
        WindManager.Instance.WindUpdater += ApplyWindDynamically;
    }

    public void InitWindZone()
    {
        Debug.Log("Searching for WindZone....");
        if (null != windZone)
        {
            Debug.Log("WindZone found! Moving on");
            return;
        }
        Debug.LogWarning("WindZone not assigned in WindSettings. Creating a new one");
        GameObject windZoneObject = new GameObject($"AutoGenWindZone_{name}");
        windZone = windZoneObject.AddComponent<WindZone>();
        windZone.mode = windMode;
        windZone.windMain = defaultWindSpeed;
        windZone.windTurbulence = defaultTurbulence;
        windZone.transform.position = Vector3.zero;
    }
    public float GetWindSpeed()
    {
        return null == windZone ? defaultWindSpeed : windZone.windMain;
    }
    public void SetWindSpeed(float value)
    {
        if (null == windZone)
        {
            Debug.LogError("Cannot set speed - No wind zone assigned to wind settings!");
            return;
        }
        windZone.windMain = Mathf.Clamp(value, minWindSpeed, maxWindSpeed);
        Debug.Log($"New windspeed: {windZone.windMain}");
    }
    public void SetTurbulenceValue(float value)
    {
        if (null == windZone)
        {
            Debug.LogError("Cannot set turbulence - No wind zone assigned to wind settings!");
            return;
        }
        windZone.windTurbulence = Mathf.Clamp(value, minTurbulence, maxTurbulence);
        Debug.Log($"New turbulence: {windZone.windTurbulence}");
    }
    public void ApplyWindDynamically(float deltaTime)
    {
        if (null == windZone)
        {
            Debug.Log("WindZone not initialized! Ensure InitWindZone() is called");
            return;
        }
        float windValue = windZone.windMain;
        windValue += gaussianWeight * WindManager.Instance.GenerateGaussianNoise(0, randomSpeedRange);
        windValue += perlinWeight * WindManager.Instance.GeneratePerlinNoise(0, randomSpeedRange, deltaTime);
        windValue += wfcWeight * WindManager.Instance.WaveFunctionCollapse(0, randomSpeedRange, deltaTime);
        SetWindSpeed(windValue);

        float turbulenceValue = windZone.windTurbulence;
        turbulenceValue += gaussianWeight * WindManager.Instance.GenerateGaussianNoise(0, randomTurbulenceRange);
        turbulenceValue += perlinWeight * WindManager.Instance.GeneratePerlinNoise(0, randomTurbulenceRange, deltaTime);
        turbulenceValue += wfcWeight * WindManager.Instance.WaveFunctionCollapse(0, randomTurbulenceRange, deltaTime);

        SetTurbulenceValue(turbulenceValue);
    }
}

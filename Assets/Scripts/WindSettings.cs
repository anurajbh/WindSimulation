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

    [Header("Default Noise Weight percentage (Sum is always 1)")]
    public float gaussianWeight = 0.5f;
    public float perlinWeight = 0.3f;
    public float wfcWeight = 0.2f;

    [Header("Range of variation for noise values")]
    public float variationRange = 2.0f;

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
            Debug.LogError("No wind zone assigned to wind settings!");
            return;
        }
        windZone.windMain = value;
        Debug.Log($"new windspeed {windZone.windMain}");
    }
    public void ApplyWindDynamically(float deltaTime)
    {
        if (null == windZone)
        {
            Debug.Log("WindZone not initialized! Ensure InitWindZone() is called");
            return;
        }
        float windValue = 0;
        windValue += gaussianWeight * WindManager.Instance.GenerateGaussianNoise(defaultWindSpeed, defaultTurbulence);
        windValue += perlinWeight * WindManager.Instance.GeneratePerlinNoise(defaultWindSpeed, variationRange, deltaTime);
        windValue += wfcWeight * WindManager.Instance.WaveFunctionCollapse(defaultWindSpeed, variationRange, deltaTime);
        SetWindSpeed(windValue);
    }
}

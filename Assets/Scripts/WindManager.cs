using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindManager : MonoSingleton<WindManager>
{
    public event Action<WindSettings> OnWindSettingsChanged;
    public event Action<float> WindUpdater;
    public List<WindSettings> windSettings;
    public WindSettings currentWindSettings;
    [Header("Default Wind Behavior Settings")]
    public float windChangeInterval = 1f;

    private Coroutine windUpdaterCoroutine;
    void Start()
    {
        if(windSettings.Count <= 0)
        {
            Debug.Log("Wind Settings list empty! No options in dropdown");
            return;
        }
        for (int i = 0; i < windSettings.Count; i++)
        {
            windSettings[i].EnsureSubscribed();
        }
        SetWindSettings(windSettings[0]);
        if (null == currentWindSettings)
        {
            Debug.LogError("WindSettings Scriptable Object is not assigned to Wind Manager");
            return;
        }
        windUpdaterCoroutine = StartCoroutine(InvokeWindUpdator());
    }

    private IEnumerator InvokeWindUpdator()
    {
        while (true)
        {
            //Debug.Log($"Invoking WindUpdater at time {Time.time}");
            WindUpdater?.Invoke(Time.time);
            yield return new WaitForSeconds(windChangeInterval);
        }
    }
    public void SetWindChangeInterval(float newInterval)
    {
        windChangeInterval = Mathf.Max(0.01f, newInterval); // Prevent zero or negative intervals
        if (windUpdaterCoroutine != null)
        {
            StopCoroutine(windUpdaterCoroutine); // Stop the currently running coroutine
        }
        windUpdaterCoroutine = StartCoroutine(InvokeWindUpdator()); // Start a new coroutine with the updated interval
    }
    /// <summary>
    /// Updates the wind settings for a specific Wind Zone.
    /// Broadcasts changes to all objects linked to the Wind Zone and optionally executes additional logic.
    /// </summary>
    /// <param name="windSettings">The Wind Settings to update.</param>
    /// <param name="onSettingsChanged">An optional callback to execute additional logic after updating the Wind Zone.</param>
    public void SetWindSettings(WindSettings newWindSettings)
    {
        if(null == newWindSettings)
        {
            Debug.LogError("Provided wind settings are null");
            return;
        }
        newWindSettings.EnsureSubscribed(); // Ensure it's subscribed to event
        currentWindSettings = newWindSettings;
        if(null == currentWindSettings)
        {
            Debug.LogError("Failed to change wind settings.");
            return;
        }
        currentWindSettings.InitWindZone();
        currentWindSettings.windZone.BroadcastMessage("OnWindSettingsChanged", currentWindSettings, SendMessageOptions.DontRequireReceiver);
        OnWindSettingsChanged?.Invoke(currentWindSettings);
        
    }
    public void UpdateWindParams(float windSpeed, float turbulence)
    {
        if(null == currentWindSettings)
        {
            Debug.LogError("WindSettings Scriptable Object is not assigned to Wind Manager");
            return;
        }
        currentWindSettings.windZone.windMain = windSpeed;
        currentWindSettings.windZone.windTurbulence = turbulence;
    }
    /// <summary>
    /// Generate Gaussian noise based on Box-Mueller transform. Yeah I didnt know what that meant before today, either.
    /// </summary>
    /// <param name="baseValue">the base value passed in by the user</param>
    /// <param name="standardDeviation">The spread of the Normal Distribution around the base value</param>
    /// <returns></returns>
    public float GenerateGaussianNoise(float baseValue, float standardDeviation)
    {
        float u1 = UnityEngine.Random.value;

        float u2 = UnityEngine.Random.value;
        if (Mathf.Epsilon > u1)
        {
            u1 = Mathf.Epsilon;
        }
        float z0 = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Cos(2f * Mathf.PI * u2);
        return baseValue+standardDeviation*z0;
    }
    /// <summary>
    /// Generate Perlin noise
    /// </summary>
    /// <param name="baseValue"></param>
    /// <param name="variationRange"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public float GeneratePerlinNoise(float baseValue, float variationRange, float time)
    {
        float noise = Mathf.PerlinNoise(time, 0.0f);
        //Above function returns a value in [0,1] range
        return baseValue + (noise - 0.5f)*2f*variationRange;
    }
    /// <summary>
    /// Generate Wave function Collapse
    /// </summary>
    /// <param name="baseValue">Represents starting wind value (speed or turbulence) </param>
    /// <param name="variationRange"> Represents a range of variation</param>
    /// <param name="time">Temporal input for transition over time</param>
    /// <returns></returns>
    public float WaveFunctionCollapse(float baseValue, float variationRange, float time)
    {
        //local constant for 10% constraint (feel free to change it if you'd like, fellow coder)
        float c = 0.1f;
        float minNeighbour = baseValue - (variationRange * c);
        float maxNeighbour = baseValue + (variationRange * c);
        float constrainedValue = UnityEngine.Random.Range(minNeighbour, maxNeighbour);
        float temporalModifier = Mathf.Sin(2 * Mathf.PI / 10f * time)*variationRange;
        return constrainedValue + temporalModifier;
    }
}

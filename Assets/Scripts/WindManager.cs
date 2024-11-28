using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WindManager : MonoSingleton<WindManager>
{
    public event Action<WindSettings> OnWindSettingsChanged;
    public List<WindSettings> windSettings;
    public WindSettings currentWindSettings;
    void Start()
    {
        if(windSettings.Count <= 0)
        {
            Debug.Log("Wind Settings list empty! No options in dropdown");
            return;
        }
        SetWindSettings(windSettings[0]);
        if (null == currentWindSettings)
        {
            Debug.LogError("WindSettings Scriptable Object is not assigned to Wind Manager");
            return;
        }
    }
    /// <summary>
    /// Updates the wind settings for a specific Wind Zone.
    /// Broadcasts changes to all objects linked to the Wind Zone and optionally executes additional logic.
    /// </summary>
    /// <param name="windSettings">The Wind Settings to update.</param>
    /// <param name="onSettingsChanged">An optional callback to execute additional logic after updating the Wind Zone.</param>
    public void SetWindSettings(WindSettings newWindSettings)
    {
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
}

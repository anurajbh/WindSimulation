using System.Collections;
using System.Collections.Generic;
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

    public void InitWindZone()
    {
        Debug.Log("Searching for WindZone....");
        if (null != windZone)
        {
            Debug.Log("WindZone found! Moving on");
            return;
        }
        Debug.LogWarning("WindZone not assigned in WindSettings. Creating a new one");
        GameObject windZoneObject = new GameObject("AutoGenWindZone");
        windZone = windZoneObject.AddComponent<WindZone>();
        windZone.mode = windMode;
        windZone.windMain = defaultWindSpeed;
        windZone.windTurbulence = defaultTurbulence;
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
    }
}

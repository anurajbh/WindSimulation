using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class to adjust settings of currently active WindSettings scriptable object from WindManager
/// </summary>
public class WindStrengthController : MonoBehaviour
{
    void Start()
    {
        if (null == WindManager.Instance)
        {
            Debug.Log("Please ensure that there is a Wind Manager in the scene");
            return;
        }
    }
    void AdjustSpeed(float speed)
    {
        WindManager.Instance.currentWindSettings.SetWindSpeed(speed);
    }
    void AdjustTurbulence(float turbulence)
    {
        WindManager.Instance.currentWindSettings.SetTurbulenceValue(turbulence);
    }
}

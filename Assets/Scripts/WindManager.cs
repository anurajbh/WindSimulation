using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public WindSettings windSettings;
    void Start()
    {
        if (null == windSettings)
        {
            Debug.LogError("WindSettings Scriptable Object is not assigned to Wind Manager");
            return;
        }
        windSettings.InitWindZone();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbineBlades : MonoBehaviour
{
    [SerializeField] float baseSpeed = 50f;
    public WindSettings windSettings;
    // Update is called once per frame
    private void Start()
    {
        if (null == windSettings)
        {
            Debug.LogError("Wind Settings Scriptable Object not assigned to Turbine!");
        }
    }
    void Update()
    {
        float rotationSpeed = baseSpeed * (null == windSettings ? 1f : windSettings.GetWindSpeed());
        transform.Rotate(Vector3.forward, rotationSpeed*Time.deltaTime);
    }
}

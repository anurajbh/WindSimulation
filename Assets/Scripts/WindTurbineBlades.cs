using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTurbineBlades : MonoBehaviour
{
    [Header("Blade Rotation Speed Multiplier")]
    [SerializeField] float baseSpeed = 50f;
    [Header("Wind Zone Association")]
    public WindSettings associatedWindSettings;
    // Update is called once per frame
    private void Start()
    {
        if (null == associatedWindSettings)
        {
            Debug.LogError("Wind Settings Scriptable Object not assigned to Turbine!");
        }
    }
    public void OnEnable()
    {
        if (null == WindManager.Instance)
        {
            Debug.Log("No Wind Manager");
            return;
        }
        WindManager.Instance.OnWindSettingsChanged += OnWindSettingsChanged;
    }
    public void OnDisable()
    {
        if (null == WindManager.Instance)
        {
            Debug.Log("No Wind Manager");
            return;
        }
        WindManager.Instance.OnWindSettingsChanged -= OnWindSettingsChanged;
    }
    void Update()
    {
        float rotationSpeed = baseSpeed * (null == associatedWindSettings ? 1f : associatedWindSettings.GetWindSpeed());
        transform.Rotate(Vector3.forward, rotationSpeed*Time.deltaTime);
    }
    void OnWindSettingsChanged(WindSettings windSettings)
    {
        associatedWindSettings = windSettings;
        if (windSettings == associatedWindSettings)
        {
            Debug.Log($"Wind settings updated for {gameObject.name}. New wind speed: {windSettings.windZone.windMain}");
            // Optionally, add logic for visual/sound effects, etc.
        }
    }

}

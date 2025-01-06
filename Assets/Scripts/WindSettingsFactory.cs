using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro input fields

public class WindSettingsFactory : MonoSingleton<WindSettingsFactory>
{
    [Header("Wind Settings Template")]
    public WindSettings defaultWindSettingsTemplate;

    [Header("UI References")]
    public TMP_InputField nameField;
    public TMP_InputField speedField;
    public TMP_InputField turbulenceField;
    public TMP_InputField intervalField;

    public void OnCreateWindSettingsButton()
    {
        // Fetch input values
        string name = nameField.text;
        float windSpeed = float.TryParse(speedField.text, out float parsedSpeed) ? parsedSpeed : defaultWindSettingsTemplate.defaultWindSpeed;
        float turbulence = float.TryParse(turbulenceField.text, out float parsedTurbulence) ? parsedTurbulence : defaultWindSettingsTemplate.defaultTurbulence;
        float deltaTime = float.TryParse(intervalField.text, out float parsedInterval) ? parsedInterval : 1f;

        // Create new WindSettings
        WindSettings newSettings = CreateNewWindSettings(name, windSpeed, turbulence, deltaTime);

        Debug.Log($"New WindSettings created: {newSettings.name} with Speed: {newSettings.defaultWindSpeed}, Turbulence: {newSettings.defaultTurbulence}");
    }

    public WindSettings CreateNewWindSettings(string name, float windSpeed, float turbulence, float deltaTime)
    {
        WindSettings newSettings = ScriptableObject.CreateInstance<WindSettings>();
        newSettings.InitWindZone();
        newSettings.name = name;
        if (defaultWindSettingsTemplate != null)
        {
            // Copy default template values
            newSettings.defaultWindSpeed = windSpeed;
            newSettings.defaultTurbulence = turbulence;
            newSettings.minWindSpeed = defaultWindSettingsTemplate.minWindSpeed;
            newSettings.maxWindSpeed = defaultWindSettingsTemplate.maxWindSpeed;
            newSettings.minTurbulence = defaultWindSettingsTemplate.minTurbulence;
            newSettings.maxTurbulence = defaultWindSettingsTemplate.maxTurbulence;
        }

        // Dynamically apply wind changes (optional)
        newSettings.ApplyWindDynamically(deltaTime);
        return newSettings;
    }
}
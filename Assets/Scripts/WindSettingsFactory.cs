using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro input fields
using System.IO;
public class WindSettingsFactory : MonoSingleton<WindSettingsFactory>
{
    [Header("Wind Settings Template")]
    public WindSettings defaultWindSettingsTemplate;

    [Header("UI References")]
    public TMP_InputField nameField;
    public TMP_InputField speedField;
    public TMP_InputField turbulenceField;
    public TMP_InputField intervalField;
    public WindDropdown windDropdown;
    public void OnCreateWindSettingsButton()
    {
        // Fetch input values
        string name = nameField.text;
        float windSpeed = float.TryParse(speedField.text, out float parsedSpeed) ? parsedSpeed : defaultWindSettingsTemplate.defaultWindSpeed;
        float turbulence = float.TryParse(turbulenceField.text, out float parsedTurbulence) ? parsedTurbulence : defaultWindSettingsTemplate.defaultTurbulence;
        float deltaTime = float.TryParse(intervalField.text, out float parsedInterval) ? parsedInterval : 1f;

        // Create new WindSettings
        WindSettings newSettings = CreateNewWindSettings(name, windSpeed, turbulence, deltaTime);
        SaveWindSettings(newSettings);
        Debug.Log($"New WindSettings created: {newSettings.name} with Speed: {newSettings.defaultWindSpeed}, Turbulence: {newSettings.defaultTurbulence}");
        // Add the new settings to the WindManager's list
        WindManager.Instance.windSettings.Add(newSettings);
        windDropdown.PopulateDropdown();
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
    public void SaveWindSettings(WindSettings windSettings)
    {
        SerializableWindSettings data = new SerializableWindSettings
        {
            name = windSettings.name,
            defaultWindSpeed = windSettings.defaultWindSpeed,
            defaultTurbulence = windSettings.defaultTurbulence,
            minWindSpeed = windSettings.minWindSpeed,
            maxWindSpeed = windSettings.maxWindSpeed,
            minTurbulence = windSettings.minTurbulence,
            maxTurbulence = windSettings.maxTurbulence
        };

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, $"{data.name}_WindSettings.json");
        File.WriteAllText(path, json);
        Debug.Log($"Wind settings saved to: {path}");
    }
    public WindSettings LoadWindSettings(string name)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{name}_WindSettings.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"No saved wind settings found at: {path}");
            return null;
        }

        string json = File.ReadAllText(path);
        SerializableWindSettings data = JsonUtility.FromJson<SerializableWindSettings>(json);

        WindSettings newSettings = ScriptableObject.CreateInstance<WindSettings>();
        newSettings.InitWindZone();
        newSettings.name = data.name;
        newSettings.defaultWindSpeed = data.defaultWindSpeed;
        newSettings.defaultTurbulence = data.defaultTurbulence;
        newSettings.minWindSpeed = data.minWindSpeed;
        newSettings.maxWindSpeed = data.maxWindSpeed;
        newSettings.minTurbulence = data.minTurbulence;
        newSettings.maxTurbulence = data.maxTurbulence;

        Debug.Log($"Wind settings loaded from: {path}");

        return newSettings;
    }
    public void OnLoadWindSettingsButton()
    {
        string name = nameField.text; // Use the name from the input field to load settings
        WindSettings loadedSettings = LoadWindSettings(name);

        if (loadedSettings != null)
        {
            WindManager.Instance.windSettings.Add(loadedSettings);
            // Refresh the dropdown
            windDropdown.PopulateDropdown();
            Debug.Log($"Loaded WindSettings: {loadedSettings.name} with Speed: {loadedSettings.defaultWindSpeed}, Turbulence: {loadedSettings.defaultTurbulence}");
        }
    }
}
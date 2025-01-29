using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
public class TurbineFactory : MonoBehaviour
{
    [Header("Wind Turbine Template")]
    public GameObject defaultTurbineTemplate;

    [Header("UI References")]
    public TMP_InputField nameField;
    public TMP_InputField transformField;
    public TMP_InputField speedField;
    public TMP_InputField efficiencyField;
    public TurbineSelectDropdown turbineDropdown;
    public List<TMP_InputField> coordinateFields;
    public Toggle useManualCoordinates;
    // Start is called before the first frame update
    public void OnTurbineCreateButton()
    {
        string name = nameField.text;
        float turbineTransformSize = float.TryParse(transformField.text, out float transformSize) ? transformSize : 1.0f;
        Vector3 turbineScale = new Vector3(transformSize, transformSize, turbineTransformSize);
        float turbineBaseSpeed = float.TryParse(speedField.text, out float baseSpeed) ? baseSpeed : 1.0f;
        float turbineEfficiency = float.TryParse(efficiencyField.text,out float efficiency) ? efficiency : 1.0f;
        if (null == defaultTurbineTemplate)
        {
            Debug.LogWarning($"No turbine template attached to Turbine Factory");
            return;
        }
        GameObject createdTurbine = Instantiate(defaultTurbineTemplate);
        createdTurbine.name = name;
        defaultTurbineTemplate.transform.localScale = turbineScale;
        PowerGenerator turbineGenerator = createdTurbine.GetComponentInChildren<PowerGenerator>();
        if(null == turbineGenerator )
        {
            Debug.LogWarning($"No Power Generator script attached to {createdTurbine.name}");
            return;
        }

        turbineGenerator.efficiency = turbineEfficiency;
        WindTurbineBlades turbineBlades = createdTurbine.GetComponentInChildren<WindTurbineBlades>();
        if (null == turbineBlades)
        {
            Debug.LogWarning($"No Wind Turbine Blades script attached to {createdTurbine.name}");
            return;
        }
        turbineBlades.associatedWindSettings = WindManager.Instance.currentWindSettings;
        turbineBlades.baseSpeed = turbineBaseSpeed;
        turbineDropdown.RegisterTurbine(turbineGenerator);
        createdTurbine.transform.position = PlaceTurbineAtUserLocation();
    }
    public Vector3 PlaceTurbineAtUserLocation()
    {
        if (useManualCoordinates.isOn)
        {
            float xPos = float.TryParse(coordinateFields[0].text, out xPos) ? xPos : 0.0f;
            float yPos = float.TryParse(coordinateFields[1].text, out yPos) ? yPos : 0.0f;
            float zPos = float.TryParse(coordinateFields[2].text, out zPos) ? zPos : 0.0f;
            return new Vector3(xPos, yPos, zPos);
        }
        if (null == LocationServiceController.Instance)
        {
            Debug.LogWarning("No location services initialized. Placing Turbine at world origin");
            return new Vector3(0, 0, 0);
        }
        // Ensure location services fetched valid coordinates
        if (LocationServiceController.Instance.latitude == 0 && LocationServiceController.Instance.longitude == 0)
        {
            Debug.LogWarning("Invalid GPS coordinates. Placing Turbine at world origin.");
            return new Vector3(0,0,0);
        }


        Vector3 turbinePosition = LocationServiceController.Instance.GPSCoordinateToWorldPosition(
            LocationServiceController.Instance.latitude,
            LocationServiceController.Instance.longitude
        );
        Debug.Log($"Turbine placed at world position {turbinePosition}");
        return turbinePosition;
    }
}

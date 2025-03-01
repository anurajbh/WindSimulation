using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

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

    private void Update()
    {
        // Handle AR touch placement when manual placement is disabled
        if (!useManualCoordinates.isOn && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TryPlaceTurbineWithAR(touch.position);
            }
        }
    }

    public void OnTurbineCreateButton()
    {
        if (useManualCoordinates.isOn)
        {
            Vector3 position = PlaceTurbineAtUserLocation();
            InstantiateTurbine(position);
        }
        else
        {
            Debug.Log("AR-based placement is active. Tap on a detected plane to place a turbine.");
        }
    }

    private void TryPlaceTurbineWithAR(Vector2 touchPosition)
    {
        if (TurbinePlacement.Instance.TryGetPlacementPosition(touchPosition, out Vector3 placementPosition))
        {
            InstantiateTurbine(placementPosition);
        }
        else
        {
            Debug.LogWarning("No valid AR surface detected.");
        }
    }

    private void InstantiateTurbine(Vector3 position)
    {
        if (defaultTurbineTemplate == null)
        {
            Debug.LogWarning("No turbine template assigned in Turbine Factory!");
            return;
        }

        GameObject createdTurbine = Instantiate(defaultTurbineTemplate, position, Quaternion.identity);

        // Apply user-defined name
        string turbineName = string.IsNullOrWhiteSpace(nameField.text) ? "Turbine" : nameField.text;
        createdTurbine.name = turbineName;

        // Apply transform size
        float turbineTransformSize = float.TryParse(transformField.text, out float transformSize) ? transformSize : 1.0f;
        createdTurbine.transform.localScale = new Vector3(transformSize, transformSize, transformSize);

        // Apply base speed and efficiency
        PowerGenerator turbineGenerator = createdTurbine.GetComponentInChildren<PowerGenerator>();
        if (turbineGenerator != null)
        {
            turbineGenerator.efficiency = float.TryParse(efficiencyField.text, out float efficiency) ? Mathf.Clamp(efficiency, 0, 1) : 0.4f;
        }

        WindTurbineBlades turbineBlades = createdTurbine.GetComponentInChildren<WindTurbineBlades>();
        if (turbineBlades != null)
        {
            turbineBlades.baseSpeed = float.TryParse(speedField.text, out float baseSpeed) ? baseSpeed : 50f;
            turbineBlades.associatedWindSettings = WindManager.Instance.currentWindSettings;
        }

        // Register turbine in selection dropdown
        turbineDropdown.RegisterTurbine(turbineGenerator);

        Debug.Log($"Turbine '{turbineName}' placed at: {position}");
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
        if (LocationServiceController.Instance == null)
        {
            Debug.LogWarning("No location services initialized. Placing Turbine at world origin");
            return Vector3.zero;
        }

        if (LocationServiceController.Instance.latitude == 0 && LocationServiceController.Instance.longitude == 0)
        {
            Debug.LogWarning("Invalid GPS coordinates. Placing Turbine at world origin.");
            return Vector3.zero;
        }

        Vector3 turbinePosition = LocationServiceController.Instance.GPSCoordinateToWorldPosition(
            LocationServiceController.Instance.latitude,
            LocationServiceController.Instance.longitude
        );
        Debug.Log($"Turbine placed at world position {turbinePosition}");
        return turbinePosition;
    }
}

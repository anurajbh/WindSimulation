using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    }
}

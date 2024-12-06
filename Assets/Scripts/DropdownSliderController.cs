using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropdownSliderController : MonoBehaviour
{
    public TMP_Dropdown dropdown; // The dropdown UI element
    public WindSlider windSlider; // Reference to the WindSlider script

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown == null)
        {
            Debug.LogError("Dropdown is not assigned!");
            return;
        }

        if (windSlider == null)
        {
            Debug.LogError("WindSlider script is not assigned!");
            return;
        }

        // Populate dropdown with property options
        dropdown.ClearOptions();
        dropdown.AddOptions(new System.Collections.Generic.List<string> { "Wind Speed", "Turbulence", "Interval" });

        // Add listener for dropdown value change
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // Set default value (optional)
        dropdown.value = 0;
        OnDropdownValueChanged(0); // Ensure slider is synced to the default value
    }

    void OnDropdownValueChanged(int index)
    {
        switch (index)
        {
            case 0: // Wind Speed
                windSlider.SetPropertyToControl(WindProperty.Speed);
                break;
            case 1: // Turbulence
                windSlider.SetPropertyToControl(WindProperty.Turbulence);
                break;
            case 2: // Interval
                windSlider.SetPropertyToControl(WindProperty.Interval);
                break;
            default:
                Debug.LogError("Unexpected dropdown index");
                break;
        }

        // Sync the slider with the updated property
        //windSlider.SyncSliderWithWindSettings(WindManager.Instance.currentWindSettings);
    }
}

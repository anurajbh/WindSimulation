using System;
using UnityEngine;
using UnityEngine.UI;

public class WindSlider : MonoBehaviour
{
    public WindSettings windSettings; // The ScriptableObject containing wind properties
    public Slider slider;             // The slider UI component

    public enum WindProperty { Speed, Turbulence, Interval }
    public WindProperty propertyToControl = WindProperty.Speed; // Selectable property

    void Start()
    {
        Debug.Log("Initializing WindSlider...");

        if (slider == null)
        {
            slider = GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogWarning("No slider found. Creating one dynamically.");
                EnsureCanvasAndSlider();
            }
        }

        // Subscribe to WindManager updates
        WindManager.Instance.OnWindSettingsChanged += SyncSliderWithWindSettings;
        // Initialize slider value based on the selected property
        SyncSliderWithWindSettings(WindManager.Instance.currentWindSettings);

        // Add listener to handle slider changes
        slider.onValueChanged.AddListener(UpdateWindProperty);
    }


    private void OnDestroy()
    {
        WindManager.Instance.OnWindSettingsChanged -= SyncSliderWithWindSettings;
    }
    public void SyncSliderWithWindSettings(WindSettings newWindSettings)
    {
        if (newWindSettings == null)
        {
            Debug.LogError("No wind settings assigned to slider");
            return;
        }
        windSettings = newWindSettings;
        switch (propertyToControl)
        {
            case WindProperty.Speed:
                slider.value = windSettings.GetWindSpeed();
                slider.minValue = windSettings.minWindSpeed;
                slider.maxValue = windSettings.maxWindSpeed;
                break;

            case WindProperty.Turbulence:
                slider.value = windSettings.defaultTurbulence;
                slider.minValue = windSettings.minTurbulence;
                slider.maxValue = windSettings.maxTurbulence;
                break;

            case WindProperty.Interval:
                slider.value = WindManager.Instance.windChangeInterval;
                break;
        }
    }

    void UpdateWindProperty(float value)
    {
        windSettings = WindManager.Instance.currentWindSettings;
        switch (propertyToControl)
        {
            case WindProperty.Speed:
                windSettings.SetWindSpeed(value);
                break;

            case WindProperty.Turbulence:
                windSettings.SetTurbulenceValue(value);
                //Debug.Log($"Updated turbulence to {value}");
                break;

            case WindProperty.Interval:
                WindManager.Instance.SetWindChangeInterval(value);
                break;
        }
    }

    void EnsureCanvasAndSlider()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.Log("No Canvas found! Creating a new one.");
            canvas = CreateNewCanvas();
        }
        CreateSlider(canvas);
    }

    private void CreateSlider(Canvas canvas)
    {
        GameObject sliderObject = new GameObject("WindSlider");
        sliderObject.transform.SetParent(canvas.transform, false);

        slider = sliderObject.AddComponent<Slider>();
        RectTransform rt = sliderObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 20);
        rt.anchoredPosition = new Vector2(0, -100);
    }

    private Canvas CreateNewCanvas()
    {
        GameObject canvasObject = new GameObject("AutoGeneratedCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }
}

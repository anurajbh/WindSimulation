using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WindDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (null == dropdown)
        {
            Debug.Log("No dropdown component");
            return;
        }
        if (null == WindManager.Instance)
        {
            Debug.Log("No Wind Manager");
            return;
        }
        PopulateDropdown();
        dropdown.onValueChanged.AddListener(SwitchWindSettings);
    }

    public void PopulateDropdown()
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < WindManager.Instance.windSettings.Count; i++)
        {
            options.Add(WindManager.Instance.windSettings[i].name);
        }
        dropdown.AddOptions(options);
    }

    void SwitchWindSettings(int index)
    {
        if (index < 0)
        {
            Debug.Log("Invalid settings omdex");
            return;
        }
        WindManager.Instance.SetWindSettings(WindManager.Instance.windSettings[index]);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARDebugger : MonoBehaviour
{
    private ARPlaneManager arPlaneManager;
    public TextMeshProUGUI debugText;
    private void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
    }

    private void Update()
    {
        if (arPlaneManager != null)
        {
            debugText.text = $"Planes detected: {arPlaneManager.trackables.count}";
            Debug.Log($"Planes detected: {arPlaneManager.trackables.count}");
        }
        else
        {
            Debug.LogError("No ARPlaneManager found! Make sure it's attached to XR Origin.");
        }
    }
}

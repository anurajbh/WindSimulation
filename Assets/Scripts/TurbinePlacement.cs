using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TurbinePlacement : MonoSingleton<TurbinePlacement>
{
    private ARRaycastManager arRaycastManager;
    private List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARRaycastManager>();

        if (arRaycastManager == null)
        {
            Debug.LogError("No ARRaycastManager found in the scene. Ensure AR Session Origin has one.");
        }
    }

    public bool TryGetPlacementPosition(Vector2 touchPosition, out Vector3 placementPosition)
    {
        placementPosition = Vector3.zero;

        if (arRaycastManager.Raycast(touchPosition, hitResults, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hitResults[0].pose;
            placementPosition = hitPose.position;
            return true;
        }

        return false;
    }
}

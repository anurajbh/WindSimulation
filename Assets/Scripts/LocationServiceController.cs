using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationServiceController : MonoSingleton<LocationServiceController>
{
    public float latitude;
    public float longitude;

    private void Start()
    {
        StartCoroutine(GetLocation());
    }

    private IEnumerator GetLocation()
    {
        // Check if the user has enabled location services
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services are not enabled by the user.");
            yield break;
        }

        // Start location service
        Input.location.Start();

        // Wait until the location service initializes
        int maxWait = 20; // Wait up to 20 seconds for a fix
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If location service fails to initialize
        if (maxWait <= 0 || Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Failed to initialize location services.");
            yield break;
        }

        // Retrieve location data
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;

        Debug.Log($"Location fetched: Latitude {latitude}, Longitude {longitude}");

        // Stop the location service
        Input.location.Stop();
    }
    public Vector3 GPSCoordinateToWorldPosition(float latitude, float longitude, float altitude = 0)
    {
        // Define your origin point in GPS (e.g., 0,0 or some reference location)
        float originLatitude = 0f;
        float originLongitude = 0f;

        // Convert latitude and longitude to meters relative to the origin
        float x = (longitude - originLongitude) * 111320f; // 1 degree longitude ~ 111.32 km
        float z = (latitude - originLatitude) * 110540f;   // 1 degree latitude ~ 110.54 km

        return new Vector3(x, altitude, z);
    }
}

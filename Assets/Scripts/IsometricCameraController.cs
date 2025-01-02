using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector3 panBounds = Vector3.zero;
    public float scrollSpeed = 2f;
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if(Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= 0 + panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= 0 + panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -panBounds.x, +panBounds.x);
        pos.y = Mathf.Clamp(pos.y, -panBounds.y, +panBounds.y);
        pos.z = Mathf.Clamp(pos.z, -panBounds.z, +panBounds.z);
        transform.position = pos;
    }
}

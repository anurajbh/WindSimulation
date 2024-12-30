using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public class DraggableTurbine : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset; // Offset between turbine and mouse position

    public LayerMask turbineLayerMask; // Assign in Inspector

    private Camera mainCamera;

    private Transform turbineParent; // Parent of the turbine

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            TrySelectTurbine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            DragTurbine();
        }
    }

    private void TrySelectTurbine()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits any turbine child colliders
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, turbineLayerMask))
        {
            // Get the parent object of the hit child collider
            turbineParent = hit.transform.root;

            if (turbineParent != null)
            {
                isDragging = true;

                // Calculate the offset based on the turbine parent’s position
                offset = turbineParent.position - hit.point;
            }
        }
    }

    private void DragTurbine()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast to get the new position
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~turbineLayerMask)) // Ignore turbine layer
        {
            // Update the parent object's position based on the ray hit point and offset
            Vector3 newPosition = hit.point + offset;

            // Example constraint to only drag on XZ plane (optional, remove if 3D movement is needed)
            newPosition.y = turbineParent.position.y;

            turbineParent.position = newPosition;
        }
    }
}

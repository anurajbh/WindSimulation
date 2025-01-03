using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTurbine : MonoBehaviour
{
    private bool isDragging = false;
    private bool isVerticalDragging = false; // To track vertical dragging mode
    private Vector3 offset; // Offset between turbine and mouse position

    public LayerMask turbineLayerMask; // Assign in Inspector
    public LayerMask collisionLayerMask; // Assign in Inspector for snapping surfaces
    public float snapHeightOffset = 0.5f; // Height offset for snapping (to avoid turbines clipping into the surface)

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
        // Detect dragging modes
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            TrySelectTurbine();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            isVerticalDragging = false;
        }

        // Check if both left and right mouse buttons are pressed
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            isVerticalDragging = true;
        }

        // Drag logic
        if (isDragging)
        {
            if (isVerticalDragging)
            {
                DragTurbineVertically();
            }
            else
            {
                DragTurbineHorizontally();
            }
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

                // Calculate the offset based on the turbine parent's position
                offset = turbineParent.position - hit.point;
            }
        }
    }

    private void DragTurbineHorizontally()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast to get the new position
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~turbineLayerMask)) // Ignore turbine layer
        {
            Vector3 newPosition = hit.point + offset;

            // Check for collisions below the turbine
            if (Physics.Raycast(new Vector3(newPosition.x, turbineParent.position.y + 10f, newPosition.z), Vector3.down, out RaycastHit collisionHit, Mathf.Infinity, collisionLayerMask))
            {
                // Snap turbine to the top of the collider
                newPosition.y = collisionHit.point.y + snapHeightOffset;
            }
            else
            {
                // Keep turbine at its current height if no ground below
                newPosition.y = turbineParent.position.y;
            }

            turbineParent.position = newPosition;
        }
    }

    private void DragTurbineVertically()
    {
        // Get the mouse movement in the Y-axis
        float verticalMovement = Input.GetAxis("Mouse Y");

        // Update the turbine's vertical position
        Vector3 newPosition = turbineParent.position;
        newPosition.y += verticalMovement;

        // Prevent the turbine from going through the ground or below a minimum height
        if (Physics.Raycast(new Vector3(newPosition.x, newPosition.y + 10f, newPosition.z), Vector3.down, out RaycastHit collisionHit, Mathf.Infinity, collisionLayerMask))
        {
            newPosition.y = Mathf.Max(newPosition.y, collisionHit.point.y + snapHeightOffset);
        }

        turbineParent.position = newPosition;
    }
}

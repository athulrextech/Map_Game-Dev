using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamController : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    // Boundary images for the game world
    [SerializeField]
    private Transform leftBoundary;
    [SerializeField]
    private Transform rightBoundary;
    [SerializeField]
    private Transform topBoundary;
    [SerializeField]
    private Transform bottomBoundary;

    public Text MovementText;

    // The starting point for the drag
    private Vector3 dragOrigin;
    private Vector3 initialCameraPosition;
    public Vector3 offset = new Vector3(0f, 27.47f, 0f);

    public static bool canDrag = true;

    private void Start()
    {
        cam.transform.position = cam.transform.position - offset;

    }

    private void Update()
    {
        if (canDrag)
        {
            PanCamera();
        }
        if (MovementText != null)
        {
            MovementText.text = "Camera Position: " + cam.transform.position;
        }
    }


    // Pans the camera based on mouse input and constrains it within the boundaries.
    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Capture the initial position where the drag starts
            dragOrigin = Input.mousePosition;
            initialCameraPosition = cam.transform.position;
        }

        if (Input.GetMouseButton(0))
        {
            // Calculate the difference between the drag origin and the current mouse position
            Vector3 difference = cam.ScreenToWorldPoint(dragOrigin) - cam.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the new camera position
            Vector3 newPosition = initialCameraPosition + difference;

            // Calculate the camera's half height and half width
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = cam.aspect * camHalfHeight;

            // Clamp the new camera position within the boundary limits
            newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary.position.x + camHalfWidth, rightBoundary.position.x - camHalfWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, bottomBoundary.position.y + camHalfHeight, topBoundary.position.y - camHalfHeight);

            // Apply the new constrained position to the camera
            cam.transform.position = newPosition;
        }
    }
}




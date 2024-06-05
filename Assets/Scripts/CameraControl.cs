using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraControl : MonoBehaviour
{
    public float panSpeed = 20f;        // Speed at which the camera pans
    public float panSmoothFactor = 5;
    public float zoomSpeed = 20f;       // Speed at which the camera zooms
    public float minZoomSize = 5f;      // Minimum orthographic size for camera zoom
    public float maxZoomSize = 30f;     // Maximum orthographic size for camera zoom

    public float padding = 2f;

    private Vector3 dragOrigin;         // Origin point of camera drag
    private bool isDragging = false;    // Flag to indicate if camera is being dragged



    private Camera cam;                 // Reference to the orthographic camera component


    public bool CanDrag = true;

    public PlayerInputSystem inputs;

    private float zoomDelta;
    private bool isZooming;

    private Vector3 defaultPos;
    private float defaultOrthoSize;

    private Vector2 minBound;
    private Vector2 maxBound;


    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        cam = GetComponent<Camera>();
        inputs = new PlayerInputSystem();

        defaultPos = transform.position;
        defaultOrthoSize = cam.orthographicSize;

    }

    private void Start()
    {
        inputs.PlayerInput.Hold.performed += Hold_performed;
        inputs.PlayerInput.Hold.canceled += Hold_canceled;

        inputs.PlayerInput.Scroll.performed += Scroll_performed;
        inputs.PlayerInput.SecodaryTouchContact.started += SecodaryTouchContact_started;
        inputs.PlayerInput.SecodaryTouchContact.canceled += SecodaryTouchContact_canceled;
        inputs.PlayerInput.SecodaryTouchContact.performed += SecodaryTouchContact_performed;
    }


    private void SecodaryTouchContact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        HandleCameraZoom();
    }

    private void SecodaryTouchContact_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isZooming = false;
    }

    private void SecodaryTouchContact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isZooming = true;
        StartCoroutine(ZoomDetectuon());
    }

    private void Scroll_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        zoomDelta = inputs.PlayerInput.Scroll.ReadValue<Vector2>().y * zoomSpeed * Time.deltaTime;
        HandleCameraZoom();
    }

    private void Hold_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       isDragging = false;
    }

    private void Hold_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        isDragging = true;
        StartCoroutine(OnDraggingCoroutine());
    }

    private void OnEnable()
    {
        inputs.PlayerInput.Enable();
        gameManager.OnGameStarted += GameManager_OnGameStarted;
    }

    private void GameManager_OnGameStarted(string parentId)
    {
        
    }

    private void OnDisable()
    {
        inputs.PlayerInput.Disable();
        gameManager.OnGameStarted -= GameManager_OnGameStarted;
    }





    private void Update()
    {
        if (!CanDrag)
            return;

 
    }


    private IEnumerator OnDraggingCoroutine()
    {
        Vector3 currentPos = inputs.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>();
        dragOrigin = currentPos;

        Vector3 prePos = cam.transform.position;

        while (isDragging && CanDrag)
        {
            currentPos = inputs.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>();
            Vector3 dragDelta = (currentPos - dragOrigin) * panSpeed * cam.orthographicSize * Time.deltaTime;

            // Move the camera based on the drag direction
            Vector3 newPos = cam.transform.position - new Vector3(dragDelta.x, dragDelta.y, 0);

            float cameraHeight = cam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cam.aspect;

            Vector3 subjectPosition = Vector3.zero;

            float xMin = subjectPosition.x - cameraWidth / 2f - padding;
            float xMax = subjectPosition.x + cameraWidth / 2f + padding;
            float yMin = subjectPosition.y - cameraHeight / 2f - padding;
            float yMax = subjectPosition.y + cameraHeight / 2f + padding;


            newPos.x = Mathf.Clamp(newPos.x, xMin, xMax);
            newPos.y = Mathf.Clamp(newPos.y, yMin, yMax);

            cam.transform.position = Vector3.Lerp(prePos, newPos, Time.deltaTime * panSmoothFactor) ; //newPos ;

            dragOrigin = currentPos;

            prePos = cam.transform.position;

            yield return null;
        }
    }


    IEnumerator ZoomDetectuon()
    {
        float preDistance = 0;
        float currentDistance = 0;

        while(isZooming)         
        {
            Vector2 pos1 = inputs.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>();
            Vector2 pos2 = inputs.PlayerInput.SecondaryFingerPositon.ReadValue<Vector2>();
            currentDistance = Vector2.Distance(pos1, pos2);
            


            if(currentDistance > preDistance)
            {
                zoomDelta = -currentDistance * zoomDelta *Time.deltaTime;
            }   
            else if(currentDistance < preDistance) 
            {
                zoomDelta = currentDistance * zoomDelta * Time.deltaTime;
            }

            preDistance = currentDistance;
            yield return null;
        }

    }



    private void HandleCameraZoom()
    {
        

        // Calculate new orthographic size
        float newOrthoSize = cam.orthographicSize - zoomDelta;

        // Restrict the orthographic size within the defined range
        newOrthoSize = Mathf.Clamp(newOrthoSize, minZoomSize, maxZoomSize);

        // Update camera orthographic size
        cam.orthographicSize = newOrthoSize;
    }

    public void ResetCamera()
    {
        transform.position = defaultPos;
        cam.orthographicSize = defaultOrthoSize;
    }

    public void SetBoundry(Vector2 minBound, Vector2 maxBound)
    {
        this.minBound = minBound;
        this.maxBound = maxBound;
    }

}

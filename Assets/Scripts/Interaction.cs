using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour
{

    #region Serialized Private Field
    [SerializeField] private Vector2 _holdOffset = new Vector2(0,1);
    [SerializeField] private float _smoothFactor = 5f;
    #endregion

    #region Private Field
    Camera _cam;
    CameraControl _camControl;
    private PlayerInputSystem _playerInput ;
    //private Vector2 _currentMousePosition;
    private bool _isDragging = false;
    private Transform _holdObjectTransform;
 
    #endregion

    #region Public Field
    //public Vector2 CurrentMousePosition { get { return _currentMousePosition; } }
    public bool IsDragging { get { return _isDragging; } }
    #endregion


    #region  Events
    public delegate void InteractionEvents();
    public event InteractionEvents HoldCanceledEvent;
    public event InteractionEvents HoldPerformedEvent;
    #endregion

    private void Awake()
    {
        _cam = Camera.main;
        _camControl = _cam.GetComponent<CameraControl>();   
        _playerInput = new PlayerInputSystem();
    }

    private void Start()
    {        
        _playerInput.PlayerInput.Hold.performed += Hold_performed;
        _playerInput.PlayerInput.Hold.canceled += Hold_canceled;
    }

    private void OnEnable()
    {
        _playerInput.PlayerInput.Enable();
        GamePlayUi.OnClickedUiPiece += GamePlayUi_OnClickedUiPiece;
    }

    private void GamePlayUi_OnClickedUiPiece(PieceObject pieceObject)
    {
        _holdObjectTransform = pieceObject.transform;
    }

    private void Hold_canceled(InputAction.CallbackContext obj)
    {
        _isDragging = false;
        if (_holdObjectTransform == null)
            return;
        _holdObjectTransform.GetComponent<IDraggable>().DraggingEnd(()=>
        {
            _holdObjectTransform = null;

            _camControl.CanDrag = true;
            HoldCanceledEvent?.Invoke();
        });
     
    }


    private void Hold_performed(InputAction.CallbackContext obj)
    {
        _isDragging = true;

        // _holdObjectTransform = GetDraggableObject();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results) 
        {
            PieceUIObject pieceUI = result.gameObject.GetComponent<PieceUIObject>();
            if (pieceUI != null)
            {
                _camControl.CanDrag = false;
                _holdObjectTransform = pieceUI.PieceObject.transform;
                pieceUI.gameObject.SetActive(false);
            }

        }

        if (_holdObjectTransform == null)
                return;
       

        StartCoroutine(OnDraggingCoroutine());
        _holdObjectTransform.GetComponent<IDraggable>().DraggingStart();
        HoldPerformedEvent?.Invoke();
    }

    Transform GetDraggableObject()
    {  
        Ray ray = _cam.ScreenPointToRay(_playerInput.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>());
        RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

        for (int i = 0; i < hits.Length; i++)
        {
            IDraggable draggable = hits[i].collider.gameObject.GetComponent<IDraggable>();
            if (!draggable.CanDrag())
                continue;
            if(draggable != null)
            {
                SpriteRenderer spriteRenderer = hits[i].collider.GetComponent<SpriteRenderer>();
                Texture2D tex = spriteRenderer.sprite.texture as Texture2D;
                Vector2 pixelUV = GetUVCoordinates(hits[i].point, hits[i].collider.GetComponent<SpriteRenderer>());
                Color pixelColor = GetPixelColor(tex, pixelUV);
                Debug.Log($"{hits[i].collider.gameObject.name}  Alpha {pixelColor.a} ");

                if(pixelColor.a > .1f)
                {
                    Vector3 objPos = hits[i].collider.transform.position;
                    _holdOffset =  objPos;
                    _holdOffset.x -= hits[i].point.x;
                    _holdOffset.y -= hits[i].point.y;
                    

                    return hits[i].collider.transform;
                }

            }

        }


        return null;

    }

    private Vector2 GetUVCoordinates(Vector2 hitPoint, SpriteRenderer spriteRenderer)
    {
        Rect spriteRect = spriteRenderer.sprite.rect;
        Texture2D texture = spriteRenderer.sprite.texture;

        float uvX = (hitPoint.x - spriteRenderer.bounds.min.x) / spriteRenderer.bounds.size.x;
        float uvY = (hitPoint.y - spriteRenderer.bounds.min.y) / spriteRenderer.bounds.size.y;

        float pixelX = Mathf.Lerp(spriteRect.xMin, spriteRect.xMax, uvX) / texture.width;
        float pixelY = Mathf.Lerp(spriteRect.yMin, spriteRect.yMax, uvY) / texture.height;

        return new Vector2(pixelX, pixelY);
    }


    private Color GetPixelColor(Texture2D texture, Vector2 pixelUV)
    {
        // Convert UV coordinates to pixel coordinates
        int pixelX = Mathf.FloorToInt(pixelUV.x * texture.width);
        int pixelY = Mathf.FloorToInt(pixelUV.y * texture.height);

        // Get the color of the pixel
        Color pixelColor = texture.GetPixel(pixelX, pixelY);

        return pixelColor;
    }


    private void OnDisable()
    {
        _playerInput.PlayerInput.Disable();
        GamePlayUi.OnClickedUiPiece += GamePlayUi_OnClickedUiPiece;
    }


    IEnumerator OnDraggingCoroutine()
    {
        if(_holdObjectTransform == null) 
            yield break;

      
        Vector3 offsetPos = _cam.ScreenPointToRay(_playerInput.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>()).GetPoint(0);
        offsetPos.x += _holdOffset.x;
        offsetPos.y += _holdOffset.y;
        offsetPos.z = 0;

        while (_isDragging) 
        {
            if (_holdObjectTransform == null)
                yield break;
            offsetPos = _cam.ScreenPointToRay(_playerInput.PlayerInput.PrimaryFingerPositon.ReadValue<Vector2>()).GetPoint(0);
            offsetPos.x += _holdOffset.x;
            offsetPos.y += _holdOffset.y;
            offsetPos.z = 0;
            _holdObjectTransform.position = offsetPos;// Vector2.Lerp( preval, offsetPos, Time.deltaTime * _smoothFactor) ;

            yield return null;
        }
    }






}

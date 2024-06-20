using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using System;
using TMPro;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class PieceObject : MonoBehaviour, IClickable, IDraggable
{
    #region Serializer Private Field
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private MapDataScriptableNew _data;
    [SerializeField] private BoxCollider2D _collider2D;
    [SerializeField] private bool _isParent;
    [SerializeField] private Color[] colors;
    [SerializeField] private float _placeTolarence = 10f;
    #endregion

    #region Private Field
    private string _id;
    private bool _isPlacedCorrectly;
    private AudioManger _audioManger;
    private Vector2 _startPosition;
    #endregion

    #region Public Field
    public string Id { get { return _id; } }
    public bool IsPlacedCorrectly { get { return _isPlacedCorrectly; } }
    public MapDataScriptableNew Data { get { return _data; } }
    public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }
    #endregion

    public TextMeshProUGUI placementCountText;
    public static int correctlyPlacedCount = 0;
    private int totalObjects = 5;
    private void Awake()
    {
        correctlyPlacedCount = 0;
        Initialize();
    }


    private void Initialize()
    {
        _audioManger = FindAnyObjectByType<AudioManger>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<BoxCollider2D>();
        _collider2D.size = _spriteRenderer.sprite.textureRect.size / 100;
        

    }
    private void Start()
    {
        Debug.Log(GameObject.FindGameObjectWithTag("piecenumtext").name);/*.GetComponent<TextMeshProUGUI>();*/
        placementCountText = GameObject.FindGameObjectWithTag("piecenumtext").GetComponent<TextMeshProUGUI>();
    }

    public void SetPieceData(MapDataScriptableNew piece, bool isParent)
    {
        _data = piece;
        _id = piece.Id;

        if (_spriteRenderer == null || _collider2D == null)
            Initialize();



        name = piece.Name + "_Object";
        _isParent = isParent;
        if (!isParent)
        {
            _spriteRenderer.sprite = Sprite.Create(piece.ChildTexture, new Rect(0, 0, piece.ChildTexture.width, piece.ChildTexture.height), Vector2.one * 0.5f);
            _collider2D.size = _spriteRenderer.sprite.textureRect.size / 100;
            transform.localPosition = piece.Position;
            _spriteRenderer.color = colors[1];
            transform.localScale = Vector3.one * piece.Scale;
            //transform.localRotation = Quaternion.Euler(piece.Rotation);
            //transform.localScale = piece.Scale; 
        }
        else
        {
            _spriteRenderer.sprite = Sprite.Create(piece.ParentTexture, new Rect(0, 0, piece.ParentTexture.width, piece.ParentTexture.height), Vector2.one * 0.5f);
            _collider2D.size = _spriteRenderer.sprite.textureRect.size / 100;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one * piece.Scale;
        }
        _isPlacedCorrectly = true;
        _spriteRenderer.sortingOrder = isParent ? 0 : 1;
    }



    public void UpdateLocalPosition()
    {
        transform.localPosition = Data.Position;
    }

    private void OnMouseDrag()
    {
        if (_isParent)
            return;
       // Debug.Log("Mouse Drag");
    }

    public void OnClick()
    {

    }

    public void DraggingStart()
    {
        if(_isParent) return;
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, colors[2], .15f); 
        _spriteRenderer.sortingOrder = 2;
        _startPosition = transform.localPosition;
    }

    public void DraggingEnd(Action endCallBack)
    {
        if( _isParent) return;

        Moved(endCallBack);
    }

    public void Moved()
    {
        _isPlacedCorrectly = IsInCorrectPlace();
        _spriteRenderer.color = colors[2];
        if (_isPlacedCorrectly)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, colors[1], .15f);
            transform.localPosition = _data.Position;
            Sequence placeAnim = DOTween.Sequence();

            placeAnim.Append(transform.DOScale(1.1f, .15f).SetEase(Ease.InQuad));
            placeAnim.Append(transform.DOScale(1, .15f).SetEase(Ease.InQuad));

            placeAnim.OnComplete(() => { _spriteRenderer.sortingOrder = 1; });
            _audioManger.PlayTick();
            Debug.Log("worked1");

           

        }
        else
        {
            _spriteRenderer.sortingOrder = 2;
            transform.localPosition+= Vector3.right * 10000;


        }
    }




    public void Moved(Action endCallBack)
    {
        _isPlacedCorrectly = IsInCorrectPlace();
        _spriteRenderer.color = colors[2];
        if (_isPlacedCorrectly)
        {
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, colors[1], .15f);
            transform.localPosition = _data.Position;
            Sequence placeAnim = DOTween.Sequence();
            float scale = transform.localScale.x;
            placeAnim.Append(transform.DOScale(scale * 1.1f, .15f));
            placeAnim.Append(transform.DOScale(scale, .15f));

            placeAnim.OnComplete(() => { _spriteRenderer.sortingOrder = 1; });
            _audioManger.PlayTick();
            Debug.Log("worked2");
            endCallBack();
        }
        else
        { 
            _spriteRenderer.sortingOrder = 2;
            //transform.localPosition+= Vector3.right * 10000;

            Sequence returnAnim = DOTween.Sequence();
            returnAnim.Append(transform.DOScale(1.05f, .1f));
            returnAnim.Append(transform.DOScale(1, .1f));

            returnAnim.Append(transform.DOLocalMove(_startPosition, .75f));
            returnAnim.OnComplete(() =>
            {
                endCallBack();
                transform.localPosition += Vector3.right * 10000;
            });
        }
            
    }


    public bool CanDrag()
    {
        return !_isParent && !_isPlacedCorrectly;
    }


    bool IsInCorrectPlace()
    {
        Vector2 dist = new Vector2(transform.localPosition.x - _data.Position.x, transform.localPosition.y - _data.Position.y);

        if (dist.sqrMagnitude < (_placeTolarence * _placeTolarence))
        {
            Debug.Log("succesfully placed");
            correctlyPlacedCount++;
            Debug.Log("correctlyPlacedCount" + correctlyPlacedCount);
            UpdatePlacementCountText();
        }

        Debug.Log(dist.sqrMagnitude);
        return dist.sqrMagnitude < (_placeTolarence*_placeTolarence);
    }
    private void UpdatePlacementCountText()
    {
        placementCountText.text = $"{correctlyPlacedCount}/{totalObjects}";
    }
}

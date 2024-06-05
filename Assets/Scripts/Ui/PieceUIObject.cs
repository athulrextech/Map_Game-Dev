using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceUIObject : MonoBehaviour
{
    private MapDataScriptableNew _data;
    private PieceObject _pieceObject;
    private Image _image;

    [SerializeField] TextMeshProUGUI _nameText;

    public PieceObject PieceObject { get { return _pieceObject; } }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void SetMapdata(PieceObject pieceObject)
    {
        _pieceObject = pieceObject;
        _data = pieceObject.Data;
        if(_image == null)
            _image = GetComponent<Image>();

        _image.sprite = Sprite.Create(_data.ChildTexture, new Rect(0, 0, _data.ChildTexture.width, _data.ChildTexture.height), Vector2.one * .05f);
        _nameText.text = _data.Name;
    }

    public MapDataScriptableNew GetData()
    {
        return _data;
    }

}

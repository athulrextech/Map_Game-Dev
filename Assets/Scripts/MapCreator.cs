using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapCreator : MonoBehaviour
{

    #region Serialized Private Field
    [SerializeField] GameObject _piecePrefab;
    #endregion


    #region Private Field
    private MapDataScriptableNew _parentPiece;

    private PieceObject _parentObj;
    private PieceObject[] _childObjs;
    private List<PieceObject> _outOfplaceObjects = new List<PieceObject>();
    private Camera _cam;
    #endregion

    #region Public Field
    public PieceObject ParentObj { get { return _parentObj; } }
    public PieceObject[] ChildObjs { get { return _childObjs; } }
    public List<PieceObject> OutOffPlaceObjects { get { return _outOfplaceObjects; } }
    #endregion


    private void Awake()
    {
        _cam = Camera.main;
        //LoadMapObjects(_parentId, 5);
    }



    public void LoadMapObjects(string parentId, int outoffPlaceCnt, MapDataScriptableNew mapData, Transform parentTransform)
    {
        if(_parentObj != null) 
            Destroy(_parentObj.gameObject);
        if (ChildObjs != null)
        {
            for (int chInx = 0; chInx < _childObjs.Length; chInx++)
            {
                if (_childObjs != null)
                    Destroy(_childObjs[chInx].gameObject);
            }
        }

        _parentPiece = MapUtility.GetPiece(parentId, mapData);

        GameObject obj = Instantiate(_piecePrefab, Vector3.zero, quaternion.identity);
        obj.transform.SetParent(parentTransform);
        _parentObj = obj.GetComponent<PieceObject>();
        _parentObj.SetPieceData(_parentPiece, true);
        _childObjs = new PieceObject[_parentPiece.ChildData.Count];
        for (int chInx = 0; chInx < _childObjs.Length; chInx++)
        {
            GameObject childObj = Instantiate(_piecePrefab, Vector3.zero, Quaternion.identity);
            _childObjs[chInx] = childObj.GetComponent<PieceObject>();
            _childObjs[chInx].SetPieceData(_parentPiece.ChildData[chInx], false);
            _childObjs[chInx].transform.parent = _parentObj.transform;
            _childObjs[chInx].UpdateLocalPosition();
        }

        PlaceObjectsOutside(outoffPlaceCnt);

    }

    private void PlaceObjectsOutside(int cnt)
    {
        float radius = _parentObj.GetComponent<BoxCollider2D>().size.magnitude;

        float x = (_cam.pixelWidth / _cam.orthographicSize) / 8; 
        x -= (x / 2);
        float y = (_cam.pixelHeight / _cam.orthographicSize) / 8;
        y -= (y / 2);

        Debug.Log(x + "  " + y);

        if (cnt < 0)
        {
            foreach(PieceObject pieceObj in _childObjs)
            {
                if (pieceObj.Data.notPlceable)
                    continue;
                pieceObj.transform.position = new Vector2(Random.Range(-x, x),Random.Range(-y, y));
                pieceObj.Moved();
            }
        }
        else
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < ChildObjs.Length; i++)
            { 
                indexes.Add(i);
            }

            List<int> randomIndexs = new List<int>();
            for(int i = 0;i < cnt; i++)
            {
                int randomIndex = indexes[Random.Range(0, indexes.Count)];

                while (ChildObjs[randomIndex].Data.notPlceable)
                {
                    randomIndex = indexes[Random.Range(0, indexes.Count)];
                }

                randomIndexs.Add(randomIndex);
                indexes.Remove(randomIndex);
            }
            
            _outOfplaceObjects.Clear();

            for(int i = 0; i < randomIndexs.Count; i++) 
            {
                int inx = randomIndexs[i];
                ChildObjs[inx].transform.position = Random.insideUnitCircle * radius;
                ChildObjs[inx].Moved();
                _outOfplaceObjects.Add(ChildObjs[inx]);
            }

        }


    }





}

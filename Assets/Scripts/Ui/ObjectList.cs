using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectList : MonoBehaviour
{
    [SerializeField] private GameObject _mapObjectPrefab;
    [SerializeField] private Transform _parentObj;
    [SerializeField] private Button _leftButton;
    [SerializeField] private Button _rightButton;

    [SerializeField] private MapDataScriptableNew[] _mapDatas = new MapDataScriptableNew[0];

    [SerializeField] private List<Button> _mapSelectionButtons = new List<Button>();


    private int curruntInx = 0;
    private GameManager _gameManager;


    private void Awake()
    {

        _gameManager = FindObjectOfType<GameManager>();
        _leftButton.onClick.AddListener(() => OnClickDirectionButton(-1));
        _rightButton.onClick.AddListener(() => OnClickDirectionButton(1));

        
    }

    private void Start()
    {
        UpdateMapobjects();
    }


    private void OnClickDirectionButton(int dir)
    {
        curruntInx = curruntInx + dir; //Mathf.Clamp( curruntInx + dir,0, MapSelectionButtons.Count-1);

        if (curruntInx < 0)
        {
            curruntInx = _mapSelectionButtons.Count - 1;
        }
        else if(curruntInx >= _mapSelectionButtons.Count)
        {
            curruntInx = 0;
        }

        for(int i = 0; i < _mapSelectionButtons.Count; i++) 
        {
            _mapSelectionButtons[i].gameObject.SetActive(i==curruntInx);
        }


        _gameManager.UpdateParentData(_mapDatas[curruntInx].Id);

    }


    private void UpdateMapobjects()
    {

        foreach(Button button in _mapSelectionButtons) 
        {
            Destroy(button.gameObject);
        }

        _mapSelectionButtons.Clear();

        for(int i = 0; i < _mapDatas.Length; i++)
        {
            int objInx = i;
            GameObject obj = Instantiate(_mapObjectPrefab, _parentObj);
            _mapSelectionButtons.Add(obj.GetComponent<Button>());
            _mapSelectionButtons[i].onClick.AddListener(() => { OnClickMapObj(objInx); });
            if(i!=0)
            {
                obj.SetActive(false);
            }
            Image img = obj.transform.GetChild(0).GetComponent<Image>();
            Texture2D tex = _mapDatas[i].ParentTexture;
            img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f); 
            
        }

           
        
    }



    void OnClickMapObj(int inx)
    {

    }

}

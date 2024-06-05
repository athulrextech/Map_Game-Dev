using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class GameManager : MonoBehaviour
{
    #region Serialized Private Field
    [SerializeField] private MapDataScriptableNew _mapData;
    [SerializeField] private GameDataScriptable _defaultGameData;
    [SerializeField] private GameDataScriptable _currentGameDataScriptable;
    [SerializeField] private string _parentId = "0";
    [SerializeField] private Interaction _interaction;
    [SerializeField] private Transform _parentTransform;
    #endregion

    #region Private Field
    private MapCreator _mapCreator;
    private PieceObject _parentObj;
    private PieceObject[] _childObjs;
    private List<PieceObject> _outOffPlaceObjects = new List<PieceObject>();
    private GameStates _currentState = GameStates.MenuScreen;
    private CameraControl _cameraControl;
    private TimerSystem _timerSystem;
    #endregion

    #region Public Field
    public PieceObject ParentObj { get { return _parentObj; } }
    public PieceObject[] ChildObjs { get { return _childObjs; } }
    public List<PieceObject> OutOffPlaceObjects { get { return _outOffPlaceObjects; } }
    public MapDataScriptableNew MapData { get { return _mapData; } }
    public enum GameStates { MenuScreen, GamePlay, GameOver }
    public TimerSystem TimerSystem { get { return _timerSystem; } }

    #endregion


    #region Events

    public delegate void GameManagerEvent();
    public event Action<string> OnGameStarted = delegate {};
    public event GameManagerEvent OnMapLoaded;
    public event GameManagerEvent OnCompletedUpdateOutOffPlaceObjects;
    public event Action<GameStates> GameStateChanged = delegate { };


    #endregion


    private void Awake()
    {

        _cameraControl = FindObjectOfType<CameraControl>();
        InitCurrentGameDataScriptable();
        _mapCreator = GetComponent<MapCreator>();
        _interaction = GetComponent<Interaction>();
        _interaction.HoldCanceledEvent += Interaction_HoldCanceledEvent;
        GameStateChanged += GameManager_GameStateChanged;
    }

    private void GameManager_GameStateChanged(GameStates state)
    {
        _currentState = state;

        switch(state)
        {
            case GameStates.MenuScreen: break;
            case GameStates.GamePlay: break;
            case GameStates.GameOver: break;
        }

    }

    private void InitCurrentGameDataScriptable()
    {
        _currentGameDataScriptable.ParentId = _defaultGameData.ParentId;
        _currentGameDataScriptable.OutOfPlacePieceCont = _defaultGameData.OutOfPlacePieceCont;
    }


    private void Interaction_HoldCanceledEvent()
    {
        UpdateOutOffPlaceObjects();
    }

    private void Start()
    {
        GameStateChanged(_currentState);
    }


    public void StartGame()
    {
        _mapCreator.LoadMapObjects(_currentGameDataScriptable.ParentId, _currentGameDataScriptable.OutOfPlacePieceCont, _mapData, _parentTransform);
        _parentObj = _mapCreator.ParentObj;
        _childObjs = _mapCreator.ChildObjs;
        _cameraControl.padding = _parentObj.Data.ParentTexture.width/500;
        _outOffPlaceObjects = _mapCreator.OutOffPlaceObjects;
        OnMapLoaded?.Invoke();

        _currentState = GameStates.GamePlay;
        GameStateChanged(_currentState);

        _cameraControl.ResetCamera();

        Vector2 min = -_parentObj.SpriteRenderer.size / 2;
        Vector2 max = _parentObj.SpriteRenderer.size / 2;

        _cameraControl.SetBoundry(min, max);
        _timerSystem = new TimerSystem(Time.time);
        OnGameStarted(_currentGameDataScriptable.ParentId);

    }


    private void Update()
    {
        if(_timerSystem != null)
            _timerSystem.TimerUpdate();
    }


    public void GameOver()
    {

        float endTime = _timerSystem.StopTimer();

        _currentState = GameStates.GameOver;
        GameStateChanged(_currentState);
    }    


    private void UpdateOutOffPlaceObjects()
    {
        _outOffPlaceObjects.Clear();

        for(int i = 0; i < _childObjs.Length; i++)
        {
            if(!_childObjs[i].IsPlacedCorrectly)
                _outOffPlaceObjects.Add(_childObjs[i]);
        }

        OnCompletedUpdateOutOffPlaceObjects?.Invoke();

        if(_outOffPlaceObjects.Count == 0)
            GameOver();

    }    

    public void UpdateParentData(string parentId)
    {
        _currentGameDataScriptable.ParentId = parentId;
    }

}

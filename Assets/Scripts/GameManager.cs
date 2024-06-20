using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    #region Serialized Private Field
    [SerializeField] private MapDataScriptableNew _mapData;
    [SerializeField] private GameDataScriptable _defaultGameData;
    [SerializeField] private GameDataScriptable _currentGameDataScriptable;
    [SerializeField] private string _parentId = "0";
    [SerializeField] private Interaction _interaction;
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private TextMeshProUGUI timerTextMesh;
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

    [SerializeField] public TextMeshProUGUI highScore;
    public bool ENDtimer = false;
    public float stopTime;
    public int _score;
    private int highScoreValue;
    private string formattedHighScore;
    [SerializeField] public TextMeshProUGUI currentTime;
    private string formattedStopTime;
    [SerializeField] public Ads _ads;

    //Ads ads = GameObject.FindGameObjectWithTag("ads").GetComponent<Ads>();


    

    private void Awake()
    {
        
        _cameraControl = FindObjectOfType<CameraControl>();
        InitCurrentGameDataScriptable();
        _mapCreator = GetComponent<MapCreator>();
        _interaction = GetComponent<Interaction>();
        _interaction.HoldCanceledEvent += Interaction_HoldCanceledEvent;
        GameStateChanged += GameManager_GameStateChanged;

        _ads = GetComponent<Ads>();
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

        highScoreValue = PlayerPrefs.GetInt("highscore",0);
        Debug.Log("highscore :" + highScoreValue);

        formattedHighScore = FormatTime(highScoreValue);
        highScore.text = formattedHighScore;
        Debug.Log("highscore:" + highScore.text);
        Debug.Log("current score:" + _score);
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
        CamController.canDrag = true;

        Vector2 min = -_parentObj.SpriteRenderer.size / 2;
        Vector2 max = _parentObj.SpriteRenderer.size / 2;
        Debug.Log("Value of min:" + min);
        Debug.Log("Value of max:" + max);

        _cameraControl.SetBoundry(min, max);
        _timerSystem = new TimerSystem(Time.time);
        OnGameStarted(_currentGameDataScriptable.ParentId);


        timerTextMesh.gameObject.SetActive(true);


    }


    private void Update()
    {
        if (_timerSystem != null)
        {
            _timerSystem.TimerUpdate();
            UpdateTimerUI();
        }
    }
    private void UpdateTimerUI()
    {
        if (timerTextMesh != null && !ENDtimer)
        {
            float t = _timerSystem._currentTime;
            int minutes = Mathf.FloorToInt(t / 60);
            int seconds = Mathf.FloorToInt(t % 60);
            int milliseconds = Mathf.FloorToInt((t * 100) % 100);

            string timerText = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            timerTextMesh.text = timerText;

        }
    }
    private void ConvertStoppedTimeToScore()
    {
        float t = stopTime;
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);
        int milliseconds = Mathf.FloorToInt((t * 100) % 100);

        formattedStopTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        _score = minutes * 10000 + seconds * 100 + milliseconds;
    }
    public void GameOver()
    {
       
        StopTime();


        float endTime = _timerSystem.StopTimer();


        ConvertStoppedTimeToScore();

        currentTime.text = formattedStopTime;

        Debug.Log("current score:" + _score);

        int storedHighScore = PlayerPrefs.GetInt("highscore", 0);

        if (_score < storedHighScore || storedHighScore == 0)
        {
            SetHighScore();
        }

        CamController.canDrag = false;


        _currentState = GameStates.GameOver;
        GameStateChanged(_currentState);

        _ads.ShowInterstitialAd();
       
    }
    private string FormatTime(int time)
    {
        int minutes = time / 10000;
        int seconds = (time % 10000) / 100;
        int milliseconds = time % 100;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
    public void SetHighScore()
    {
        //jj
        PlayerPrefs.SetInt("highscore", _score);
        highScoreValue = PlayerPrefs.GetInt("highscore", int.MaxValue);
        formattedHighScore = FormatTime(highScoreValue);
        highScore.text = formattedHighScore;
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

    public void StopTime()
    {
        ENDtimer = true;
        stopTime = _timerSystem._currentTime;
        Debug.Log("stopped time :" + stopTime);

    }
    public void BackButton()
    {
        SceneManager.LoadScene(0);
    }

}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayUi : PanelBase<PanelManager> 
{
    #region Serialized Private Field
    [SerializeField] private Transform _scrollContentTransfrom;
    [SerializeField] private GameObject _imagePrefab;
    [SerializeField] private GameObject _scrollObj;
    [SerializeField] private Interaction _playerInetaction;
    #endregion

    #region Private Field
    private GameManager _gameManager;
    private List<PieceUIObject> pieceUIObjects = new List<PieceUIObject>();
    private CanvasGroup _canvasGroup;
    private ScrollRect _scrollRect;
    #endregion

    #region Event
    public delegate void PieceUIObjectDelegate(PieceObject pieceObject);
    public static event PieceUIObjectDelegate OnClickedUiPiece;

    #endregion


    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnMapLoaded += GameManager_OnMapLoaded;
        _gameManager.OnCompletedUpdateOutOffPlaceObjects += _gameManager_OnCompletedUpdateOutOffPlaceObjects;
        _gameManager.GameStateChanged += _gameManager_GameStateChanged;
        _playerInetaction = FindObjectOfType<Interaction>();
        _playerInetaction.HoldPerformedEvent += Interaction_HoldPerformedEvent;
        _playerInetaction.HoldCanceledEvent += Interaction_HoldCanceledEvent;
        _scrollRect = GetComponentInChildren<ScrollRect>();
       

    }



    private void _gameManager_GameStateChanged(GameManager.GameStates gameState)
    {
        if (gameState == GameManager.GameStates.GameOver)
            manager.EnablePanel(typeof(GameOverPanel));
    }

    private void Interaction_HoldPerformedEvent()
    {
        DOVirtual.Float(1, 0,.25f, (val)=> _canvasGroup.alpha = val);
        _canvasGroup.interactable = false;
        _scrollRect.enabled = false;
       // _scrollObj.SetActive(false);
    }
    private void Interaction_HoldCanceledEvent()
    {
        DOVirtual.Float( 0, 1, .25f, (val) => _canvasGroup.alpha = val);
        _canvasGroup.interactable = true;
        _scrollRect.enabled = true;
        // _scrollObj.SetActive(true);
    }



    private void _gameManager_OnCompletedUpdateOutOffPlaceObjects()
    {
        ResetScroll();
    }

    private void GameManager_OnMapLoaded()
    {
        ResetScroll();
    }

    private void OnEnable()
    {
        ResetScroll();
    }


    private void ResetScroll()
    {
        for (int i = 0; i < pieceUIObjects.Count; i++)
        {
            Destroy(pieceUIObjects[i].gameObject);
        }

        pieceUIObjects.Clear();

        for (int i = 0; i < _gameManager.OutOffPlaceObjects.Count; i++)
        {
            int inx = i;
            GameObject pieceUiObjcet = Instantiate(_imagePrefab, _scrollContentTransfrom);
            pieceUiObjcet.name = _gameManager.OutOffPlaceObjects[i].Data.Name;
            PieceUIObject pieceUIObjectScript = pieceUiObjcet.GetComponent<PieceUIObject>();
            pieceUIObjectScript.SetMapdata(_gameManager.OutOffPlaceObjects[i]);
            pieceUIObjects.Add(pieceUIObjectScript);

            //pieceUiObjcet.GetComponent<Button>().OnPointerClick 
        }
    }

    public override void Init(PanelManager uIManager)
    {
        manager = uIManager;
    }
}

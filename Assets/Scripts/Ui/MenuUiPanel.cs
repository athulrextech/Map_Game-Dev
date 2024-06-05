using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUiPanel : PanelBase<PanelManager> 
{
    #region Serialized Private Fieled

    [SerializeField] private Button _startGameButton;

    #endregion




    public override void Init(PanelManager uIManager)
    {
        manager = uIManager;
    }

    private void Start()
    {
        _startGameButton.onClick.AddListener(OnStartButtonClick);


    }

    private void OnStartButtonClick()
    {
        manager.UiManager.GameManager.StartGame();
        manager.EnablePanel(typeof(GamePlayUi));
    }

}

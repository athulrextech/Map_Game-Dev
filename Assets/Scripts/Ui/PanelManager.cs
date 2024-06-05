using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{

    [SerializeField] protected List<PanelBase<PanelManager>> panels = new List<PanelBase<PanelManager>>();
    protected PanelBase<PanelManager> currentActivePanel ;
    protected Type currentType;

    private UiManager uiManager;

    public UiManager UiManager { get { return uiManager; } }

    private void Awake()
    {
        uiManager = GetComponent<UiManager>();
    }


    private void Start()
    {
        InitilizePanels();
        EnablePanel(typeof(MenuUiPanel));
    }

    private void InitilizePanels()
    {
        foreach (var panel in panels) 
        {
            panel.gameObject.SetActive(true);
            panel.Init(this);
            panel.gameObject.SetActive(false);
        }
    }


    public PanelBase<PanelManager> EnablePanel(Type type)
    {
        PanelBase<PanelManager> newPanel = panels.Find(panel => panel.GetType() == type);


        if (newPanel != null)
        {
            currentActivePanel?.OnDeactivation();
            newPanel.OnActivation();
            currentActivePanel = newPanel;
        }
        return currentActivePanel;
    }
}

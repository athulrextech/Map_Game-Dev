using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameOverPanel : PanelBase<PanelManager>
{
    [SerializeField] TextMeshProUGUI timeText;

    public override void Init(PanelManager uIManager)
    {
        manager = uIManager;
    }

    private void OnEnable()
    {
        if (manager == null)
            return;

        TimerSystem timerSystem = manager.UiManager.GameManager.TimerSystem;

        int hours = Mathf.FloorToInt(timerSystem.EndTime / 3600);
        int minutes = Mathf.FloorToInt((timerSystem.EndTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(timerSystem.EndTime);
        int milliseconds = Mathf.FloorToInt((timerSystem.EndTime * 1000) % 1000);

        timeText.text = $"Time - {hours}:{minutes}:{seconds}:{milliseconds} ";
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

}

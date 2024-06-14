using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveData : MonoBehaviour
{
    public TextMeshProUGUI myName;
    public TextMeshProUGUI highScore;
    public int HighScore;
    public static string HighScoreFormatted;

    private void Start()
    {
        HighScore = PlayerPrefs.GetInt("highscore");
        Debug.Log("high score:" + HighScore);

        highScore.text = FormatTime(PlayerPrefs.GetInt("highscore"));
        Debug.Log("best time:" + highScore.text);

        myName.text = PlayerPrefs.GetString("username");
        Debug.Log("username:" + myName.text);

    }
    void Update()
    {

    }
    public void SendScore()
    {

        PlayerPrefs.SetInt("Score", HighScore);
        HighScores.UploadScore(myName.text, 100000 - HighScore);

    }
    private string FormatTime(int time)
    {
        int minutes = time / 10000;
        int seconds = (time % 10000) / 100;
        int milliseconds = time % 100;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}

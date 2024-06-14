using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] rNames;
    public TMPro.TextMeshProUGUI[] rScores;
    HighScores myScores;
    public static string HighScoreFormatted;

    void Start() //Fetches the Data at the beginning
    {
        //for (int i = 0; i < rNames.Length; i++)
        //{
        //    rNames[i].text = i + 1 + ". Fetching...";
        //}
        myScores = GetComponent<HighScores>();
        StartCoroutine("RefreshHighscores");
    }
    public void SetScoresToMenu(PlayerScore[] highscoreList) //Assigns proper name and score for each text value
    {

        for (int i = 0; i < rNames.Length; i++)
        {
            //rNames[i].text = i + 1 + ". ";
            if (highscoreList.Length > i)
            {
                rScores[i].text = FormatTime(highscoreList[i].score);
                rNames[i].text = highscoreList[i].username;
            }
        }
    }
    IEnumerator RefreshHighscores() //Refreshes the scores every 30 seconds
    {
        while (true)
        {
            myScores.DownloadScores();
            yield return new WaitForSeconds(30);
        }
    }
    private string FormatTime(int time)
    {
        int minutes = time / 10000;
        int seconds = (time % 10000) / 100;
        int milliseconds = time % 100;
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
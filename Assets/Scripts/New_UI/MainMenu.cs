using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
public class MainMenu : MonoBehaviour
{
    public TMP_InputField username;
    public GameObject settingsMenuUI;

    private void Start()
    {
        settingsMenuUI.SetActive(false);
       username.text = PlayerPrefs.GetString("username");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || (Input.GetKeyDown(KeyCode.KeypadEnter))) 
        {
            SendUsername();
        }
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void ViewMap()
    {
        SceneManager.LoadScene(1);
    }
    public void Leaderboard()
    {
        SceneManager.LoadScene(2);
    }
    public void SendUsername()
    {
        PlayerPrefs.SetString("username", username.text);
        settingsMenuUI.SetActive(false);
        Debug.Log("name entered successfully");
    }
    public void Settings()
    {
        settingsMenuUI.SetActive(true);       
    }
    public void Cancel()
    {
        settingsMenuUI.SetActive(false);
    }
}

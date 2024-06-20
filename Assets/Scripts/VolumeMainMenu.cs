using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMainMenu : MonoBehaviour
{
    public Sprite mutedSprite;
    public Sprite unmutedSprite;


    private bool isMuted = false;
    private Button volumeButton;
    private Image buttonImage;

    private void Start()
    {
        volumeButton = GetComponent<Button>();
        buttonImage = volumeButton.GetComponent<Image>();
        volumeButton.onClick.AddListener(ToggleMute);

        buttonImage.sprite = unmutedSprite;
    }

    private void ToggleMute()
    {
        if (isMuted)
        {
            Backgroundmusic.Instance.UnmuteAll();
            isMuted = false;
            buttonImage.sprite = unmutedSprite;
        }
        else
        {
            Backgroundmusic.Instance.MuteAll();
            isMuted = true;
            buttonImage.sprite = mutedSprite;
        }
    }
}

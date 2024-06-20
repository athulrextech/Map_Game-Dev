using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgroundmusic : MonoBehaviour
{
    public static Backgroundmusic Instance;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
    }
    public void PlayBackgroundMusic()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.loop = true;
            _audioSource.Play();
        }
    }
    public void StopBackgroundMusic()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
    public void MuteAll()
    {
        AudioListener.volume = 0;
    }

    public void UnmuteAll()
    {
        AudioListener.volume = 1;
    }
}

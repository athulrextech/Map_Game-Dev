using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManger : MonoBehaviour
{
    
    AudioSource _audioSource;

    private void Awake()
    {
   
      _audioSource = GetComponent<AudioSource>();
        
    }

    public void PlayTick()
    {
        _audioSource.Play();
    }


}

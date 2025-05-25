using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ambience_gester : MonoBehaviour
{
    public AudioSource ambienceSource;
    public void play_ambience()
    {
        if (ambienceSource != null && !ambienceSource.isPlaying)
        {
            ambienceSource.Play();
        }
    }
    public void stop_ambience()
    {
        if (ambienceSource != null && ambienceSource.isPlaying)
        {
            ambienceSource.Stop();
        }

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

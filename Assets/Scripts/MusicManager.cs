using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip music;
    
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = music;
        source.loop = true;
        source.Play();
    }
}

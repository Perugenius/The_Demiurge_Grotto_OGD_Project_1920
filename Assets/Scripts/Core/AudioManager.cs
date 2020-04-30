using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager: Singleton<AudioManager>
{
    public AudioSource BackgroundMusic;
    public AudioSource SoundEffect;

    public AudioClip ExampleClip;
    
    public void ExampleSound()
    {
        if (ExampleClip!=null)
            SoundEffect.PlayOneShot(ExampleClip);
    }
    
    //Other functions called for each sound clip to be played
    //...
}

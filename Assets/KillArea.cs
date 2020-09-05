using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Players;
using UnityEngine;

public class KillArea : MonoBehaviour
{
    private bool _triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_triggered) return;
        PlayableCharacter playableCharacter = other.gameObject.GetComponent<PlayableCharacter>();
        if (playableCharacter != null)
        {
            _triggered = true;
            playableCharacter.KillPlayer();
        }
    }
}

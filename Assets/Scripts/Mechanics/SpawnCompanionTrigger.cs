using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Mechanics.Players;
using UnityEngine;

public class SpawnCompanionTrigger : MonoBehaviour
{

    [SerializeField] private Transform spawnPosition;
    private string _currentCharacter;
    private List<GameObject> _characters;
    private bool _triggered = false;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_triggered) return;
        _triggered = true;

        _currentCharacter = GameObject.Find("TutorialManager").GetComponent<TutorialManager>().GetCurrentCharacter().name;
        _characters = GameObject.Find("TutorialManager").GetComponent<TutorialManager>().GetAllCharacters();
        foreach (var character in _characters)
        {
            if (character.name != _currentCharacter)
            {
                SpawnCompanion(character);
                break;
            } 
        }
    }

    private void SpawnCompanion(GameObject character)
    {
        character.transform.position = spawnPosition.position;
        character.SetActive(true);
        character.GetComponent<Rigidbody2D>().mass = 1000;
        //TODO call spawn animation
        //character.GetComponent<PlayableCharacter>().
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

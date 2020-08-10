using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseBubble : MonoBehaviour
{
    private List<GameObject> _players;
    [SerializeField] private GameObject letter;
    private const int _numOfPLayers = 2;

    // Start is called before the first frame update
    void Start()
    {
        _players = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && !_players.Contains(other.gameObject))
        {
            _players.Add(other.gameObject);
        }

        if (_players.Count == _numOfPLayers)
        {
            letter.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && _players.Contains(other.gameObject))
        {
            _players.Remove(other.gameObject);
        }
    }
}

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
            StartCoroutine(DestroyBubble());
        }
    }

    private IEnumerator DestroyBubble()
    {
        while (gameObject.transform.localScale.x > 0.2f)
        {
            gameObject.transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player") && _players.Contains(other.gameObject))
        {
            _players.Remove(other.gameObject);
        }
    }
}

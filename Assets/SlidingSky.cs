using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class SlidingSky : MonoBehaviour
{
    [SerializeField] private GameObject sky;
    [SerializeField] private float speed;
    private Transform _tr;
    // Start is called before the first frame update
    void OnEnable()
    {
        _tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        _tr.position += Vector3.right*speed*Time.deltaTime; 
        if(_tr.position.x>59f)
        {
            _tr.position = new Vector3(-60f,_tr.position.y,0);
        }
    }
}

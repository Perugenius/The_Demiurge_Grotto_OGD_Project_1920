using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class BaloonBackground : Movable
{
    [SerializeField] private float _deltaFloating;
    [SerializeField] private float _speed;
    [SerializeField] private float _accelertion;
    private Transform _tr;

    private bool up;
    // Start is called before the first frame update
    void Start()
    {
        _tr = GetComponent<Transform>();
        SetFloating(_accelertion,_deltaFloating);
    }

    // Update is called once per frame
    void Update()
    {
        /*if(!MoveFixedDistanceAcceleratedDecelerated)
        {
            SetFixedDistanceAcceleratedDecelerated(
                up ? _tr.position + Vector3.up * _deltaFloating / 2f : _tr.position - Vector3.up * _deltaFloating / 2f,
                _speed, _accelertion);
            up = !up;
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class Menu : Movable
{
    [SerializeField] private float speed;
    [SerializeField] private float distance;
    [SerializeField] private Vector2 unfocusedDirection;
    [SerializeField] private bool defaultFocused;
    private bool _isFocused = false;
    // Start is called before the first frame update
    void Start()
    {
        _isFocused = defaultFocused;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Focus(bool isFocused)
    {
        if (_isFocused == isFocused)return;
        _isFocused = isFocused;
        if(isFocused) SetFixedDistance(-unfocusedDirection, speed, distance);
        else SetFixedDistance(unfocusedDirection,speed,distance);
    }

    public bool isFocused()
    {
        return _isFocused;
    }
}

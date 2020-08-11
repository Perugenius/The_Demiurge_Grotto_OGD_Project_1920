using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AirParticle : Movable
{

    public Vector3 normal;
    
    [SerializeField] private float intensity;
    [SerializeField] private float period;
    
    public float Intensity
    {
        get => intensity;
        set => intensity = value;
    }

    public float Period
    {
        get => period;
        set => period = value;
    }

    private int _count;
    private bool _toRight;
    private int _intensityCount;
    private Vector2 _direction;

    // Start is called before the first frame update
    void Start()
    {
        _toRight = (Random.Range(1, 3) == 1);
        if (Math.Abs(normal.x) > 0.01f) _direction = (normal.x >= 0) ? Vector2.right : Vector2.left;
        else _direction = (normal.y >= 0) ? Vector2.up : Vector2.right;
    }

    // Update is called once per frame
    void Update()
    {
        _count++;

        if (_count / 60f >= period)
        {
            _count = 0;
            _intensityCount++;
            AddForce(_direction * (_toRight ? 1f : -1f), Mathf.Pow(_intensityCount,0.75f) * intensity);
            _toRight = !_toRight;
        }

    }
}

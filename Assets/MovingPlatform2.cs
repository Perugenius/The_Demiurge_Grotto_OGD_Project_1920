﻿using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

public class MovingPlatform2 : Movable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float speed = 3;
    [SerializeField] private GameObject destinationObj;
    private Vector3 _destination;
    private Vector3 _startingPoint;
    private Transform _tr;
    private bool _isStartPosition = true;

    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    // Start is called before the first frame update
    void Start()
    {
        _destination = destinationObj.GetComponent<Transform>().position;
        _tr = GetComponent<Transform>();
        _startingPoint = _tr.position;
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (MoveFixedDistance) return;
        if (_isStartPosition) MoveToDestination(_destination-_startingPoint,_isStartPosition);
        else MoveToDestination(_startingPoint-_destination,_isStartPosition);
    }

    private void MoveToDestination(Vector3 directPath, bool isStartPosition)
    {
        Vector3 direction = Vector3.Normalize(directPath);
        float distance = directPath.magnitude;
        SetFixedDistance(direction,speed,distance);
        _animator.SetBool(IsMoving,true);
        StartCoroutine(WaitMovementEnd(isStartPosition));
        _isStartPosition = !isStartPosition;
    }
    
    
    private IEnumerator WaitMovementEnd(bool isStartPosition)
    {
        while (MoveFixedDistance)
        {
            yield return null;
        }
        _animator.SetBool(IsMoving,false);
    }
}
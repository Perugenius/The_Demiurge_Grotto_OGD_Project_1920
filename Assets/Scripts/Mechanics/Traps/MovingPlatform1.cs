﻿using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Traps
{
    public class MovingPlatform1 : Movable
    {
        [SerializeField] private GameObject destinationObj;
        [SerializeField] private float speed = 5;
        [SerializeField] private Animator _animator;
        private Vector3 _destination;
        private Vector3 _startingPoint;
        private Transform _tr;
        private bool _isStartPosition = true;
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        void Start()
        {
            _destination = destinationObj.GetComponent<Transform>().position;
            _tr = GetComponent<Transform>();
            _startingPoint = _tr.position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            if(MoveFixedDistance) return;
            if(!_isStartPosition) return;
            ChangePosition();
        }

        private void ReturnToStartingPosition()
        {
            MoveToDestination(_startingPoint, _isStartPosition);
        }

        private void ChangePosition()
        {
            if (_isStartPosition)
            {
                MoveToDestination(_destination, _isStartPosition);
            }
            else
            {
                MoveToDestination(_startingPoint, _isStartPosition);
            }
        }

        private void MoveToDestination(Vector3 targetPosition, bool isStartPosition)
        {
            SetFixedDistance(targetPosition,speed);
            _animator.SetBool(IsMoving,true);
            StartCoroutine(WaitMovementEnd(isStartPosition));
            if(isStartPosition) _isStartPosition = false;
        }

        private IEnumerator WaitMovementEnd(bool isStartPosition)
        {
            while (MoveFixedDistance)
            {
                yield return null;
            }
            _animator.SetBool(IsMoving,false);
            if (!isStartPosition) _isStartPosition = true;
            if (!_isStartPosition) StartCoroutine(WaitToReturn());
        }

        private IEnumerator WaitToReturn()
        {
            yield return new WaitForSeconds(2);
            ReturnToStartingPosition();
        }
    }
}
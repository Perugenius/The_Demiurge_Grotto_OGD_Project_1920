using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Traps
{
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
            if (!PhotonNetwork.IsMasterClient) return;
            if (MoveFixedDistance) return;
            if (_isStartPosition) MoveToDestination(_destination,_isStartPosition);
            else MoveToDestination(_startingPoint,_isStartPosition);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player") && gameObject.GetPhotonView().IsMine) other.gameObject.transform.SetParent(_tr);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player") && gameObject.GetPhotonView().IsMine) other.gameObject.transform.SetParent(null);
        }

        private void MoveToDestination(Vector3 targetPosition, bool isStartPosition)
        {
            SetFixedDistance(targetPosition,speed);
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
}

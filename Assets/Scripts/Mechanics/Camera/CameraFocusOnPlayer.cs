using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mechanics.Camera
{
    public class CameraFocusOnPlayer : Movable
    {
        public GameObject cameraPlayer; 
        private Vector3 _playerRoomPosition;
        private Vector3 _playerTransitionPosition;
        private bool _isInTransition = false;
        [SerializeField ]private float acceleration = 1;
        [SerializeField ]private float initSpeed = 1;
        private Orientation _playerTransitionOrientation;
        private GameObject _lastTransition;

        public void PlayerInRoom(Vector3 roomPosition, GameObject player)
        {
            if(player != cameraPlayer) return;
            _playerRoomPosition = new Vector3(roomPosition.x, roomPosition.y, -10f);
            UpdateCameraPosition();
        }

        public void PlayerInTransition(Vector3 transitionPosition, Orientation transitionOrientation, GameObject player)
        {
            if(player != cameraPlayer) return;
            _playerTransitionPosition = new Vector3(transitionPosition.x, transitionPosition.y, -10f);
            _playerTransitionOrientation = transitionOrientation;
            _isInTransition = true;
            UpdateCameraPosition(player);
        }

        public void PlayerOutTransition(GameObject player)
        {
            if(player != cameraPlayer) return;
            _isInTransition = false;
            UpdateCameraPosition();
        }
    
        private void UpdateCameraPosition()
        {
            //Debug.Log("CameraPosition = " + Tr.position);
            if(_isInTransition)
            {
                if(Vector2.Distance(_playerTransitionPosition,Tr.position)<0.5) return;
                //if(MoveFixedDistanceAcceleratedDecelerated) return;
                SetFixedDistanceAcceleratedDecelerated(_playerTransitionPosition, initSpeed, acceleration);
            }
            else
            {
                if(Vector2.Distance(_playerRoomPosition,Tr.position)<0.5) return;
                Debug.Log("Target RoomPosition = " + _playerRoomPosition);
                SetFixedDistanceAcceleratedDecelerated(_playerRoomPosition, initSpeed, acceleration);
            }
        }
        
        private void UpdateCameraPosition(GameObject player)
        {
            //Debug.Log("CameraPosition = " + Tr.position);
            if(_isInTransition)
            {
                if(Vector2.Distance(_playerTransitionPosition,Tr.position)<0.5) return;
                if(Vector2.Distance(player.transform.position + new Vector3(0,0,-10),_playerTransitionPosition) > 7) return;
                //if(MoveFixedDistanceAcceleratedDecelerated) return;
                SetFixedDistanceAcceleratedDecelerated(_playerTransitionPosition, initSpeed, acceleration);
            }
            else
            {
                if(Vector2.Distance(_playerRoomPosition,Tr.position)<0.5) return;
                SetFixedDistanceAcceleratedDecelerated(_playerRoomPosition, initSpeed, acceleration);
            }
        }
        
        public Orientation PlayerTransitionOrientation => _playerTransitionOrientation;

        //Deprecated methods
        private IEnumerator CheckIfRightMovement()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.2f);
                Vector3 currentPlayerRoomPosition = FindPlayerRoomAreaPosition();
                if (_playerRoomPosition != currentPlayerRoomPosition)
                {
                    _playerRoomPosition = currentPlayerRoomPosition;
                    UpdateCameraPosition();
                }
            }
        }

        private Vector3 FindPlayerRoomAreaPosition()
        {
            LayerMask mask = LayerMask.GetMask("TriggerAreaPlayer");
            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = mask;
            List<Collider2D> colliders = new List<Collider2D>();
            Physics2D.OverlapCircle(cameraPlayer.transform.position, 0.2f,filter,colliders); //useless, RoomArea collider is flagged as "isTrigger"
            foreach (var collider in colliders)
            {
                Debug.Log(collider.gameObject.name);
                if (collider.gameObject != null && collider.gameObject.name == "RoomArea")
                {
                    Vector3 currentPlayerRoomPosition = collider.gameObject.transform.position;
                    Debug.Log("Player room position = " + currentPlayerRoomPosition);
                    return new Vector3(currentPlayerRoomPosition.x, currentPlayerRoomPosition.y, -10f);
                }
            }
            return _playerRoomPosition;
        }
    }
}

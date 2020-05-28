using UnityEngine;

namespace Mechanics.Camera
{
    public class CameraFocusOnPlayer : Movable
    {
        public GameObject cameraPlayer; 
        private Vector3 _playerRoomPosition;
        private Vector3 _playerVerticalTransitionPosition;
        private bool _isInVerticalTransition = false;
        [SerializeField ]private float acceleration = 1;
        [SerializeField ]private float initSpeed = 1;

        public void PlayerInRoom(Vector3 roomPosition, GameObject player)
        {
            if(player != cameraPlayer) return;
            _playerRoomPosition = new Vector3(roomPosition.x, roomPosition.y, -10f);
            UpdateCameraPosition();
        }

        public void PlayerInVerticalTransition(Vector3 verticalTransitionPosition, GameObject player)
        {
            if(player != cameraPlayer) return;
            _playerVerticalTransitionPosition = new Vector3(verticalTransitionPosition.x, verticalTransitionPosition.y, -10f);
            _isInVerticalTransition = true;
            UpdateCameraPosition();
        }

        public void PlayerOutVerticalTransition(GameObject player)
        {
            if(player != cameraPlayer) return;
            _isInVerticalTransition = false;
            UpdateCameraPosition();
        }
    
        private void UpdateCameraPosition()
        {
            //Debug.Log("CameraPosition = " + Tr.position);
            if(_isInVerticalTransition)
            {
                if(Vector2.Distance(_playerVerticalTransitionPosition,Tr.position)<0.5) return;
                if(MoveFixedDistanceAcceleratedDecelerated) return;
                SetFixedDistanceAcceleratedDecelerated(_playerVerticalTransitionPosition, initSpeed, acceleration);
            }
            else
            {
                if(Vector2.Distance(_playerRoomPosition,Tr.position)<0.5) return;
                SetFixedDistanceAcceleratedDecelerated(_playerRoomPosition, initSpeed, acceleration);
            }
        }
    }
}

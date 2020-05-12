using System.Collections;
using System.Collections.Generic;
using Mechanics;
using UnityEngine;

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
        Debug.Log("CameraPosition = " + Tr.position);
        if(_isInVerticalTransition) SetFixedDistanceAcceleratedDecelerated(_playerVerticalTransitionPosition,initSpeed,acceleration);
        else SetFixedDistanceAcceleratedDecelerated(_playerRoomPosition,initSpeed,acceleration);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Camera;
using Model;
using UnityEngine;

public class CameraArea : MonoBehaviour
{
    private CameraFocusOnPlayer _mainCameraFocusOnPlayer;
    private GameObject _mainCamera;
    private Transform _tr;
    private int _count = 0;
    private float _roomWidth;
    private float _roomHeight;
    private Vector2 _currentCameraMovementDirection;
    private Vector2 _lastCameraPosition;
    void Start()
    {
        _tr = GetComponent<Transform>();
        DungeonRoom dungeonRoom = transform.parent.gameObject.GetComponent<DungeonRoom>();
        _roomWidth = dungeonRoom.width;
        _roomHeight = dungeonRoom.height;
        _currentCameraMovementDirection = Vector2.zero;
        _lastCameraPosition = Vector2.zero;
        StartCoroutine(WaitForCamera());
    }

    private void FixedUpdate()
    {
        _currentCameraMovementDirection = ((Vector2) _tr.position - _lastCameraPosition).normalized;
        _lastCameraPosition = _tr.position;
    }

    private IEnumerator WaitForCamera()
    {
        _mainCamera = GameObject.Find("Main Camera(Clone)");
        while (_mainCamera == null)
        {
            yield return new WaitForSeconds(1);
            _mainCamera = GameObject.Find("Main Camera(Clone)");
        }
        _mainCameraFocusOnPlayer = _mainCamera.GetComponent<CameraFocusOnPlayer>();
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        bool isHorizontalCameraMovement = _currentCameraMovementDirection == Vector2.right ||
                                          _currentCameraMovementDirection == Vector2.left;
        if(_mainCameraFocusOnPlayer!=null && Vector2.Distance(_mainCamera.transform.position,_tr.position)> (isHorizontalCameraMovement ?_roomWidth/2:_roomHeight/2))
            _mainCameraFocusOnPlayer.PlayerInRoom(_tr.position,other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_mainCameraFocusOnPlayer!=null)_mainCameraFocusOnPlayer.PlayerInRoom(_tr.position, other.gameObject);
    }
}

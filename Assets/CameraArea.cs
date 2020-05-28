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
    void Start()
    {
        _tr = GetComponent<Transform>();
        DungeonRoom dungeonRoom = transform.parent.gameObject.GetComponent<DungeonRoom>();
        _roomWidth = dungeonRoom.width;
        _roomHeight = dungeonRoom.height;
        StartCoroutine(WaitForCamera());
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
        if(_mainCameraFocusOnPlayer!=null && Vector2.Distance(_mainCamera.transform.position,_tr.position)> _roomWidth/2)
            _mainCameraFocusOnPlayer.PlayerInRoom(_tr.position,other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_mainCameraFocusOnPlayer!=null)_mainCameraFocusOnPlayer.PlayerInRoom(_tr.position, other.gameObject);
    }
}

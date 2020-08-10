using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Camera;
using Model;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mechanics
{
    public class RoomArea : MonoBehaviour
    {
        private int _numOfPlayersInRoom = 0;
        private bool _roomEnable = false;
        private CameraFocusOnPlayer _mainCameraFocusOnPlayer;
        private GameObject _mainCamera;
        private Transform _tr;
        private int _count = 0;
        private float _roomWidth;
        private float _roomHeight;
        private DungeonMap _dungeonMap;

        private void Start()
        {
            _tr = GetComponent<Transform>();
            if(_numOfPlayersInRoom == 0) SetActiveRoom(false);
            _dungeonMap = GameObject.Find("Main Camera").transform.Find("Canvas").transform.Find("Map")
                .GetComponent<DungeonMap>();
            StartCoroutine(WaitForCamera());
        }
        
        private IEnumerator WaitForCamera()
        {
            _mainCamera = GameObject.Find("Main Camera");
            while (_mainCamera == null)
            {
                yield return new WaitForSeconds(1);
                _mainCamera = GameObject.Find("Main Camera");
            }
            _mainCameraFocusOnPlayer = _mainCamera.GetComponent<CameraFocusOnPlayer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _numOfPlayersInRoom++;
            if(_numOfPlayersInRoom > 0 && !_roomEnable) SetActiveRoom(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_numOfPlayersInRoom>0) _numOfPlayersInRoom--;
            if (_numOfPlayersInRoom == 0 && _roomEnable) StartCoroutine(WaitBeforeDisabling());
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            GameObject parentRoom = gameObject.transform.parent.gameObject;

            //get player photonView 
            PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
            photonView = photonView == null ? other.transform.parent.gameObject.GetComponent<PhotonView>() : photonView;
            
            if(photonView.IsMine && _dungeonMap.ActiveRoom != parentRoom)
            {
                _dungeonMap.AddRoom(parentRoom);
                _dungeonMap.ActiveRoomIcon(parentRoom);
            }
        }

        private IEnumerator WaitBeforeDisabling()
        {
            if (_mainCamera != null)
            {
                float checkDistance = _mainCameraFocusOnPlayer.PlayerTransitionOrientation == Orientation.Vertical
                    ? _roomHeight / 2 + 1f
                    : _roomWidth / 2 + 1f;
                while (Vector2.Distance(_mainCamera.transform.position, _tr.position) < checkDistance)
                {
                    yield return new WaitForSeconds(5);
                }
                yield return new WaitForSeconds(5);
                if (_numOfPlayersInRoom == 0 && _roomEnable) SetActiveRoom(false);
            }
        }

        private void SetActiveRoom(bool activeValue)
        {
            if (!PhotonNetwork.IsMasterClient) return;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(activeValue);
            }

            if (activeValue)
            {
                foreach (var enemySpawner in GetComponentsInChildren<EnemySpawner>())
                {
                    if (enemySpawner.IsEnemySpawned()) ;
                    else enemySpawner.InstantiateEnemy();
                }
            
                foreach (var collectibleSpawner in GetComponentsInChildren<CollectibleSpawner>())
                {
                    if (collectibleSpawner.IsCollectibleSpawned()) ;
                    else collectibleSpawner.InstantiateCollectible();
                }
            }

            _roomEnable = activeValue;
        }
    }
}

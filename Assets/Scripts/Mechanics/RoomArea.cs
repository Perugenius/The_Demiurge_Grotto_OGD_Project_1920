﻿using System;
using System.Collections;
using System.Collections.Generic;
using Mechanics.Camera;
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

        private void Start()
        {
            _tr = GetComponent<Transform>();
            StartCoroutine(WaitForCamera());
            if(_numOfPlayersInRoom == 0) SetActiveRoom(false);
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
            _count++;
            if(_count < 200) return;
            _count = 0;
            if(_mainCameraFocusOnPlayer!=null)_mainCameraFocusOnPlayer.PlayerInRoom(_tr.position,other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _numOfPlayersInRoom++;
            if(_mainCameraFocusOnPlayer!=null)_mainCameraFocusOnPlayer.PlayerInRoom(_tr.position, other.gameObject);
            if(_numOfPlayersInRoom > 0 && !_roomEnable) SetActiveRoom(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_numOfPlayersInRoom>0) _numOfPlayersInRoom--;
            if (_numOfPlayersInRoom == 0 && _roomEnable) StartCoroutine(WaitBeforeDisabling());
        }

        private IEnumerator WaitBeforeDisabling()
        {
            if (_mainCamera != null)
            {
                while (Vector2.Distance(_mainCamera.transform.position, _tr.position) < 16)
                {
                    yield return new WaitForSeconds(5);
                }

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

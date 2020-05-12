using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class RoomArea : MonoBehaviour
    {
        private int _numOfPlayersInRoom = 0;
        private bool _roomEnable = false;
        private CameraFocusOnPlayer _mainCamera;
        private Transform _tr;

        private void Start()
        {
            _tr = GetComponent<Transform>();
            _mainCamera = GameObject.Find("Main Camera").GetComponent<CameraFocusOnPlayer>();
            if(_numOfPlayersInRoom == 0) SetActiveRoom(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _numOfPlayersInRoom++;
            _mainCamera.PlayerInRoom(_tr.position, other.gameObject);
            if(_numOfPlayersInRoom > 0 && !_roomEnable) SetActiveRoom(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_numOfPlayersInRoom>0) _numOfPlayersInRoom--;
            if(_numOfPlayersInRoom == 0 && _roomEnable) SetActiveRoom(false);
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

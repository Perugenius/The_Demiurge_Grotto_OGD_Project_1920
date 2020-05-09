using System;
using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class RoomArea : MonoBehaviour
    {
        private int _numOfPlayersInRoom = 0;
        private bool _roomEnable = false;

        private void Start()
        {
            if(_numOfPlayersInRoom == 0) SetActiveRoom(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            _numOfPlayersInRoom++;
            if(_numOfPlayersInRoom > 0 && !_roomEnable) SetActiveRoom(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(_numOfPlayersInRoom>0) _numOfPlayersInRoom--;
            if(_numOfPlayersInRoom == 0 && _roomEnable) SetActiveRoom(false);
        }

        private void SetActiveRoom(bool activeValue)
        {
            /*if (PhotonNetwork.IsMasterClient) ;*/
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

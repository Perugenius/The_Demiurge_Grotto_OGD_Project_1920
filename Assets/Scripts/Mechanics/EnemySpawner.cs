using System.Collections.Generic;
using System.IO;
using Model;
using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<GameObject> enemiesThatCanBeSpawned;
    
        private int _difficulty = 0;
        private Transform _tr;
        private bool _enemySpawned = false;
    
        void Start()
        {
            
        }

        public bool IsEnemySpawned()
        {
            return _enemySpawned;
        }

        public void InstantiateEnemy()
        {
            _tr = GetComponent<Transform>();
            List<GameObject> suitableEnemies = new List<GameObject>();
            _difficulty = GetComponentInParent<DungeonRoom>().GetDifficulty();
        
            //TODO filter enemies by difficulty
            suitableEnemies = enemiesThatCanBeSpawned;
            //...

            if (suitableEnemies.Count == 0) return;
            string enemyName = suitableEnemies[Random.Range(0, suitableEnemies.Count)].name;
            GameObject enemy = PhotonNetwork.Instantiate(Path.Combine("Enemies", enemyName), _tr.position, _tr.rotation);
            enemy.transform.parent = gameObject.transform;
            _enemySpawned = true;
        }
    }
}

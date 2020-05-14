using System.Collections.Generic;
using System.IO;
using Model;
using Photon.Pun;
using UnityEngine;

namespace Mechanics
{
    public class CollectibleSpawner : MonoBehaviour
    {
        public List<GameObject> collectiblesThatCanBeSpawned;
    
        private int _difficulty = 0;
        private Transform _tr;
        private bool _collectibleSpawned = false;
    
        void Start()
        {
            
        }

        public bool IsCollectibleSpawned()
        {
            return _collectibleSpawned;
        }

        public void InstantiateCollectible()
        {
            _tr = GetComponent<Transform>();
            List<GameObject> suitableCollectibles = new List<GameObject>();
            _difficulty = GetComponentInParent<DungeonRoom>().GetDifficulty();
        
            //TODO filter collectibles by difficulty
            suitableCollectibles = collectiblesThatCanBeSpawned;
            //...

            if (suitableCollectibles.Count == 0) return;
            GameObject collectibleReference = suitableCollectibles[Random.Range(0, suitableCollectibles.Count)];
            GameObject collectible = Instantiate(collectibleReference, _tr.position, _tr.rotation);
            collectible.transform.parent = gameObject.transform;
            _collectibleSpawned = true;
        }
    }
}

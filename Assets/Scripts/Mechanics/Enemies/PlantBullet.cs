using System;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class PlantBullet : Movable
    {
        [SerializeField] private float speed;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            MoveDynamic(-Tr.right, speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}

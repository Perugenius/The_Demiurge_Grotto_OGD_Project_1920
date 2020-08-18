using System;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players.PlayerAttacks
{
    public class Laser : MonoBehaviour, IDamageInflictor
    {
        private Vector2 _direction;
        private float _speed;
        private float _range;
        private float _traveledDistance;
        private float _damage;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void FixedUpdate()
        {
            if (gameObject.GetPhotonView().IsMine)
            {
                if (_traveledDistance < _range)
                {
                    Vector2 pos = transform.position;
                    Vector2 oldPos = new Vector2(pos.x, pos.y);
                    transform.position = pos + Time.deltaTime * _speed * _direction;
                    _traveledDistance += Vector2.Distance(transform.position, oldPos);
                }
                else
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if ((LayerMask.GetMask("Obstacle") & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        public Vector2 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public float Range
        {
            get => _range;
            set => _range = value;
        }

        public float Damage
        {
            get => _damage;
            set => _damage = value;
        }

        public float GetDamage()
        {
            return _damage;
        }
    }
}

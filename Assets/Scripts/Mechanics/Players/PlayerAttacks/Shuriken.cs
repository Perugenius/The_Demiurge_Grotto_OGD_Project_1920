using System;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players.PlayerAttacks
{
    public class Shuriken : MonoBehaviour, IDamageInflictor
    {
        private Rigidbody2D _rb;
        private float _damage;
        private Vector2 _speed;
    
    
    
    
        // Start is called before the first frame update
        void Start()
        {
            _rb = gameObject.GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            transform.position += (Vector3) (Time.deltaTime * _speed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        { 
            PhotonNetwork.Destroy(gameObject);
        }

        public void SetDamage(float damage)
        {
            _damage = damage;
        }

        public void SetSpeed(Vector2 speed)
        {
            _speed = speed;
        }

        public float GetDamage()
        {
            return _damage;
        }
    }
}

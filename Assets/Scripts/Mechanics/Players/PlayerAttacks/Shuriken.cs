using System;
using UnityEngine;

namespace Mechanics.Players.PlayerAttacks
{
    public class Shuriken : MonoBehaviour
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
            Debug.Log(other.gameObject.name);
            Destroy(gameObject);
        }

        public void SetDamage(float damage)
        {
            _damage = damage;
        }

        public void SetSpeed(Vector2 speed)
        {
            _speed = speed;
        }
    
    }
}

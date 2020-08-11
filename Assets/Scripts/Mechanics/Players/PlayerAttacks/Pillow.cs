using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Players.PlayerAttacks
{
    public class Pillow : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private float _duration;
        private Transform _playerPosition;

        // Start is called before the first frame update
        void Start()
        {
            _playerPosition = GetComponentInParent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            transform.Rotate(Time.deltaTime * 2 * Vector3.back);
            transform.RotateAround(_playerPosition.position, Vector3.back,_speed*Time.deltaTime);
        }

        private IEnumerator StartTimer()
        {
            yield return null;
        }

        public void SetDamage(float damage)
        {
            _damage = damage;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Players.PlayerAttacks
{
    public class Pillow : MonoBehaviour, IDamageInflictor
    {
        private float _damage;
        private float _speed;
        private float _duration;
        private Transform _playerPosition;
        private Pinkie _pinkie;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(nameof(StartTimer));
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            transform.Rotate(Time.deltaTime * 180* Vector3.forward);
            transform.RotateAround(_playerPosition.position, Vector3.back,_speed*Time.deltaTime);
        }

        private IEnumerator StartTimer()
        {
            
            yield return new WaitForSeconds(_duration);
            _pinkie.SetCanSummonPillow(true);
            Destroy(gameObject);
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

        public void SetPlayerPosition(Transform playerPosition)
        {
            _playerPosition = playerPosition;
        }

        public void SetPinkie(Pinkie pinkie)
        {
            _pinkie = pinkie;
        }


        public float GetDamage()
        {
            return _damage;
        }
    }
}

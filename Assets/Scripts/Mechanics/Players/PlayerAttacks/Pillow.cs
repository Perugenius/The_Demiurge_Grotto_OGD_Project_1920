using System;
using System.Collections;
using Photon.Pun;
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

        private bool _isMine;

        // Start is called before the first frame update
        void Start()
        {
            if (gameObject.GetPhotonView().IsMine)
            {
                _isMine = true;
                StartCoroutine(nameof(StartTimer));
            }
            else
            {
                _isMine = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void FixedUpdate()
        {
            if (_isMine)
            {
                transform.Rotate(Time.deltaTime * _speed * Vector3.forward);
                transform.RotateAround(_playerPosition.position, Vector3.back, _speed * Time.deltaTime);
            }
        }

        private IEnumerator StartTimer()
        {
            
            yield return new WaitForSeconds(_duration);
            _pinkie.SetCanSummonPillow(true);
            PhotonNetwork.Destroy(gameObject);
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

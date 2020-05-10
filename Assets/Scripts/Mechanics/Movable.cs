using System;
using UnityEngine;

namespace Mechanics
{
    public class Movable : MonoBehaviour
    {
        public float Thrust;
        protected Transform Tr;
        protected Rigidbody2D Rb;
        protected Vector2 TargetPosition;
        protected bool MoveFixedDistance;
        protected bool MoveFixedDistanceAccelerated;
        private float _fixedSpeed;
        private float _fixedDistance;
        private Vector2 _fixedDirection;
        private float _fixedAcceleration;

        protected Vector2 m_velocity;

        protected void Awake()
        {
            Tr = GetComponent<Transform>();
            Rb = GetComponent<Rigidbody2D>();
        }

        public void MoveDynamic(Vector2 direction, float maxSpeed)
        {
            Vector2 targetVelocity = new Vector2(maxSpeed * direction.x, Rb.velocity.y);
            Rb.velocity = Vector2.SmoothDamp(Rb.velocity, targetVelocity, ref m_velocity, 0.2f);
        }
        
        public void MoveKinematic(Vector2 direction, float speed)
        {
            Tr.position = (Vector2)Tr.position + Time.deltaTime * speed * direction;
        }

        public void SetFixedDistance(Vector2 direction, float speed, float distance)
        {
            MoveFixedDistance = true;
            _fixedDirection = direction;
            _fixedDistance = distance;
            _fixedSpeed = speed;
        }
        
        public void SetFixedDistanceAccelerated(Vector2 direction, float initSpeed, float distance, float acceleration)
        {
            MoveFixedDistanceAccelerated = true;
            _fixedDirection = direction;
            _fixedDistance = distance;
            _fixedSpeed = initSpeed;
            _fixedAcceleration = acceleration;
        }

        public void Jump(float jumpHeight)
        {
            Rb.AddForce(new Vector2(0,jumpHeight),ForceMode2D.Impulse);
        }

        // Update is called once per frame
        protected virtual void FixedUpdate()
        {
            if (MoveFixedDistance)
            {
                Tr.position = (Vector2)Tr.position + Time.deltaTime * _fixedSpeed * _fixedDirection;
                _fixedDistance = _fixedDistance - Time.deltaTime * _fixedSpeed;
                if(_fixedDistance <= 0) MoveFixedDistance = false;
            }

            if (MoveFixedDistanceAccelerated)
            {
                Tr.position = (Vector2)Tr.position + Time.deltaTime * _fixedSpeed * _fixedDirection + .5f * Time.deltaTime * Time.deltaTime *  _fixedAcceleration * _fixedDirection;
                _fixedDistance = _fixedDistance - Time.deltaTime * _fixedSpeed - .5f * Time.deltaTime * Time.deltaTime * _fixedAcceleration;
                if(_fixedDistance <= 0) MoveFixedDistanceAccelerated = false;
            }
        }
    }
}

﻿using System;
using TMPro;
using UnityEngine;

namespace Mechanics
{
    public class Movable : MonoBehaviour
    {
        protected Transform Tr;
        protected Rigidbody2D Rb;
        protected Vector2 TargetPosition;
        protected bool MoveFixedDistance;
        protected bool MoveFixedDistanceAccelerated;
        protected bool MoveFixedDistanceAcceleratedDecelerated;
        protected bool FloatingUp;
        protected bool FloatingDown;
        private float _fixedSpeed;
        private float _fixedDistance;
        private float _totalDistance;
        private Vector2 _fixedDirection;
        private Vector3 _fixedDirectionVector3;
        private float _fixedAcceleration;
        private Vector3 _lastPosition = Vector3.zero;
        private Vector3 _currentMovingDirection = Vector3.zero;

        private float _floatingAmplitude;
        private float _floatingAcceleration;

        protected Vector2 m_velocity;

        protected void Awake()
        {
            Tr = GetComponent<Transform>();
            Rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// It returns the current moving direction (normalized) of the object computed from the difference between its
        /// two last positions.
        /// </summary>
        /// <returns>Moving direction</returns>
        public Vector3 GetMovingDirection()
        {
            return _currentMovingDirection;
        }

        public void MoveDynamic(Vector2 direction, float maxSpeed)
        {
            Vector2 targetVelocity = new Vector2(maxSpeed * direction.x, Rb.velocity.y);
            Rb.velocity = Vector2.SmoothDamp(Rb.velocity, targetVelocity, ref m_velocity, 0.2f);
        }

        public void MoveDynamic(Vector2 direction, float maxSpeed, float smoothTime)
        {
            Vector2 targetVelocity = new Vector2(maxSpeed * direction.x, Rb.velocity.y);
            Rb.velocity = Vector2.SmoothDamp(Rb.velocity, targetVelocity, ref m_velocity, smoothTime);
        }

        public void MoveDynamicUp(Vector2 direction, float maxSpeed, float smoothTime)
        {
            Vector2 targetVelocity = new Vector2(Rb.velocity.x, maxSpeed*direction.y);
            Rb.velocity = Vector2.SmoothDamp(Rb.velocity, targetVelocity, ref m_velocity, smoothTime);
        }

        public void HorizontalDeceleration()
        {
            Rb.velocity = Vector2.SmoothDamp(Rb.velocity, new Vector2(0,Rb.velocity.y), ref m_velocity, 0.1f);
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
        
        public void SetFixedDistance(Vector3 targetPosition, float speed)
        {
            MoveFixedDistance = true;
            Vector3 movementVector = targetPosition - Tr.position;
            _fixedDirection = Vector3.Normalize(movementVector);
            _fixedDistance = movementVector.magnitude;
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
        
        public void SetFixedDistanceAccelerated(Vector3 targetPosition, float initSpeed, float acceleration)
        {
            MoveFixedDistanceAccelerated = true;
            Vector3 movementVector = targetPosition - Tr.position;
            _fixedDirection = Vector3.Normalize(movementVector);;
            _fixedDistance = movementVector.magnitude;
            _fixedSpeed = initSpeed;
            _fixedAcceleration = acceleration;
        }
        
        /// <summary>
        /// It reaches the max acceleration when the remaining distance is half of the total distance,
        /// then it slows down decelerating at each fixedUpdate call.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="initSpeed"></param>
        /// <param name="acceleration"></param>
        public void SetFixedDistanceAcceleratedDecelerated(Vector3 targetPosition, float initSpeed, float acceleration)
        {
            MoveFixedDistanceAcceleratedDecelerated = true;
            Vector3 movementVector = targetPosition - Tr.position;
            _fixedDirectionVector3 = Vector3.Normalize(movementVector);;
            _fixedDistance = movementVector.magnitude;
            _totalDistance = _fixedDistance;
            _fixedSpeed = initSpeed;
            _fixedAcceleration = acceleration;
        }

        public void Jump(float jumpHeight)
        {
            Rb.velocity = new Vector2(Rb.velocity.x,0);
            Rb.AddForce(new Vector2(0,jumpHeight),ForceMode2D.Impulse);
        }

        public void AddForce(Vector2 direction, float intensity)
        {
            Rb.AddForce(direction*intensity, ForceMode2D.Impulse);
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
                _fixedSpeed = _fixedSpeed + Time.deltaTime * _fixedAcceleration;
                if(_fixedDistance <= 0) MoveFixedDistanceAccelerated = false;
            }
            
            if (MoveFixedDistanceAcceleratedDecelerated)
            {
                Tr.position = Tr.position + Time.deltaTime * _fixedSpeed * _fixedDirectionVector3;
                _fixedDistance = _fixedDistance - Time.deltaTime * _fixedSpeed;
                if(_fixedDistance > _totalDistance/2f) _fixedSpeed = _fixedSpeed + Time.deltaTime * _fixedAcceleration;
                else _fixedSpeed = _fixedSpeed - Time.deltaTime * _fixedAcceleration;
                if(_fixedDistance <= 0) MoveFixedDistanceAcceleratedDecelerated = false;
            }

            if (FloatingUp && !MoveFixedDistanceAcceleratedDecelerated)
            {
                FloatingUp = false;
                FloatingDown = true;
                SetFixedDistanceAcceleratedDecelerated(Tr.position + Vector3.down*_floatingAmplitude, 0, _floatingAcceleration);
            }
            
            if (FloatingDown && !MoveFixedDistanceAcceleratedDecelerated)
            {
                FloatingUp = true;
                FloatingDown = false;
                SetFixedDistanceAcceleratedDecelerated(Tr.position + Vector3.up*_floatingAmplitude, 0, _floatingAcceleration);
            }

            _currentMovingDirection = (Tr.position - _lastPosition).normalized;
            _lastPosition = Tr.position;
        }

        public void JumpLateral(float jumpHeight, Vector2 direction)
        {
            Rb.AddForce(new Vector2(direction.x, jumpHeight),ForceMode2D.Impulse);
        }

        public void SetFloating(float acceleration, float amplitude)
        {
            FloatingDown = true;
            _floatingAmplitude = amplitude;
            _floatingAcceleration = acceleration;
            SetFixedDistanceAcceleratedDecelerated(Tr.position + Vector3.down*amplitude, 0, acceleration);
        }

        public void StopFloating()
        {
            FloatingUp = false;
            FloatingDown = false;
            MoveFixedDistanceAcceleratedDecelerated = false;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    public class Mushroom : Movable
    {
        private Vector2 _direction;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _direction = Random.Range(0, 1) == 0
                ? Vector2.left
                : Vector2.right;
        }

        private void Update()
        {
            
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            MoveDynamic(_direction * Thrust);
            Debug.Log(Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x * 2, -1, 0), LayerMask.GetMask("Obstacle")));
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x * 2, -1, 0), LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.up);
                Debug.Log(_direction);
            }
        }
    }
}

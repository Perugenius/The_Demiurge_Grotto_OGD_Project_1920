using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mechanics.Enemies
{
    public class Mushroom : Movable
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private bool initialDirection;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _direction = initialDirection
                ? Vector2.left
                : Vector2.right;
            
            Vector3 scale = Tr.localScale;
            scale.x = -_direction.x;
            Tr.localScale = scale;
        }

        private void Update()
        {
            
        }
        
        protected override void FixedUpdate()
        {
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            MoveDynamic(_direction, speed);
            if (!Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, -1, 0), LayerMask.GetMask("Obstacle")) || Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, 0, 0), LayerMask.GetMask("Obstacle")))
            {
                _direction = Vector2.Reflect(_direction, Vector2.right);
                Vector3 newScale = Tr.localScale;
                newScale.x = -_direction.x;
                Tr.localScale = newScale;
            }
        }
    }
}

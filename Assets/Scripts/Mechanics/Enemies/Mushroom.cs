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
            if (Physics2D.OverlapPoint(Tr.position + new Vector3(_direction.x, -1, 0), 18))
            {
                
            }
        }
    }
}

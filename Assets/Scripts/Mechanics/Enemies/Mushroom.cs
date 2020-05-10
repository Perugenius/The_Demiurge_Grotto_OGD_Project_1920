using System;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Mushroom : Movable
    {
        private float _horizontal;
        
        
        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void Update()
        {
            _horizontal = Input.GetAxisRaw("Horizontal");
        }
        
        void FixedUpdate()
        {
            MoveDynamic(new Vector2(_horizontal*Thrust,0));
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
        }
    }
}

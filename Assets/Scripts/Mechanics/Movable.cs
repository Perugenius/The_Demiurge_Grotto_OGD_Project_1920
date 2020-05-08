using System;
using UnityEngine;

namespace Mechanics
{
    public class Movable : MonoBehaviour
    {
        protected Transform Tr;
        protected Rigidbody2D Rb;
        protected Vector2 TargetPosition;
        protected bool IsMoving;
        protected bool MoveFixedDistance;

        private void Awake()
        {
            Tr = GetComponent<Transform>();
            Rb = GetComponent<Rigidbody2D>();
        }

        public void MoveDynamic(Vector2 direction)
        {
            Rb.AddRelativeForce(direction);
        }
        
        public void MoveKinematic(Vector2 direction, float speed)
        {
            Tr.position = (Vector2)Tr.position + Time.deltaTime * speed * direction;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

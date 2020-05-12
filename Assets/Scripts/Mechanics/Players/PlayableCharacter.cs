using System;
using Photon.Pun;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Mechanics.Players
{
    public abstract class PlayableCharacter : Character
    {

        public bool localTesting;
        
        protected float Speed;
        protected int MaxConsecutiveJump = 1;
        protected int CurrentConsecutiveJump = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            CurrentSpeed = statistics.movSpeed;
            CurrentHealth = statistics.maxHealth;
            CurrentAttack = statistics.attack;
            Debug.Log(gameObject.GetPhotonView().Owner);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                Speed = Input.GetAxisRaw("Horizontal");
            
            
                if (Input.GetKeyDown(KeyCode.Space) && CurrentConsecutiveJump<MaxConsecutiveJump)
                {
                    IsJumping = true;
                    Jump(45);
                    CurrentConsecutiveJump++;
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector2 direction;
            if (Speed > 0)
            {
                direction = Vector2.right;
                MoveDynamic(direction,CurrentSpeed*Speed);
            }
            else if (Speed < 0)
            {
                direction = Vector2.left;
                MoveDynamic(direction,CurrentSpeed*Speed*-1);
            }
            else
            {
                HorizontalDeceleration();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Terrain"))
            {
                CurrentConsecutiveJump = 0;
            }
        }
    }
}

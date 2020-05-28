﻿using System;
using Photon.Pun;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

namespace Mechanics.Players
{
    public abstract class PlayableCharacter : Character
    {

        public bool localTesting;

        protected Animator Animator;
        protected static readonly int Horizontal = Animator.StringToHash("Horizontal");
        protected static readonly int IsMoving = Animator.StringToHash("IsMoving");
        protected static readonly int FaceDirection = Animator.StringToHash("FaceDirection");
        protected static readonly int IsJumpingAnim = Animator.StringToHash("IsJumping");
        protected static readonly int IsDescending = Animator.StringToHash("IsDescending");
        protected float Speed;
        protected int MaxConsecutiveJump = 1;
        protected int CurrentConsecutiveJump = 0;
        
        protected bool IsAnchored;
        protected PhotonView AnchoredPlayer;

        
        
        // Start is called before the first frame update
        void Start()
        {
            CurrentSpeed = statistics.movSpeed;
            CurrentHealth = statistics.maxHealth;
            CurrentAttack = statistics.attack;

            Animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                Speed = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump"))
                {
                    RaycastHit2D raycast = Physics2D.Raycast(Tr.position,Vector2.down, 1.1f,LayerMask.GetMask("Obstacle"));
                    RaycastHit2D playerRaycastHit2D = Physics2D.Raycast(new Vector2(Tr.position.x,Tr.position.y-1.1f), Vector2.down, 0.5f,LayerMask.GetMask("PlayerPhysic"));
                    if (raycast)
                    {
                        IsJumping = true;
                        Jump(62);
                        Animator.SetBool(IsJumpingAnim, true);
                    }
                    else if (playerRaycastHit2D)
                    {
                        Transform otherplayer = playerRaycastHit2D.transform;
                        raycast = Physics2D.Raycast(Tr.position,Vector2.down, 1.1f,LayerMask.GetMask("Obstacle"));
                        if (raycast)
                        {
                            IsJumping = true;
                            Jump(62);
                            Animator.SetBool(IsJumpingAnim, true);
                        }
                    }
                }
                
                Animate();
            }
        }

        private void Animate()
        {
            if (Speed != 0)
            {
                if (Speed > 0)
                {
                    Animator.SetFloat(FaceDirection,1);
                }
                else
                {
                    Animator.SetFloat(FaceDirection,-1);
                }
                Animator.SetBool(IsMoving,true);
            }
            else
            {
                Animator.SetBool(IsMoving,false);
            }
            Animator.SetFloat(Horizontal,Speed);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector2 direction;
            if (Speed > 0)
            {
                direction = Vector2.right;
                MoveDynamic(direction,CurrentSpeed*Speed);
                if (AnchoredPlayer)
                {
                    AnchoredPlayer.RPC("MoveWithFriend",RpcTarget.All, direction,CurrentSpeed*Speed);
                }
            }
            else if (Speed < 0)
            {
                direction = Vector2.left;
                MoveDynamic(direction,CurrentSpeed*Speed*-1);
                if (AnchoredPlayer)
                {
                    AnchoredPlayer.RPC("MoveWithFriend",RpcTarget.All, direction,CurrentSpeed*Speed*-1);
                }
            }
            else
            {
                HorizontalDeceleration();
            }
            CheckJumpPhase();
        }

        protected void CheckJumpPhase()
        {
            if (IsJumping)
            {
                RaycastHit2D raycast = Physics2D.Raycast(Tr.position,Vector2.down, 1.1f,LayerMask.GetMask("Obstacle"));
                if (raycast)
                {
                    IsJumping = false;
                    Animator.SetBool(IsJumpingAnim,false);
                    Animator.SetBool(IsDescending,false);
                }
                else
                {
                    if (Rb.velocity.y < 0)
                    {
                        Animator.SetBool(IsDescending, true);
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                RaycastHit2D hit2D = Physics2D.BoxCast(new Vector2(Tr.position.x, Tr.position.y + 1.3f),
                    new Vector2(0.5f, 0.5f), 0, Vector2.up, LayerMask.GetMask("PlayerPhysic"));
                if (hit2D)
                {
                    AnchoredPlayer = hit2D.collider.gameObject.GetPhotonView();
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                if (AnchoredPlayer != null)
                {
                    AnchoredPlayer = null;
                }
            }
        }


        [PunRPC]
        public void MoveWithFriend(Vector2 direction, float speed)
        {
            MoveDynamic(direction,speed);
        }
    }
}

using System;
using System.Collections;
using Core.SaveLoadData;
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
        protected static readonly int FaceDirectionAnimator = Animator.StringToHash("FaceDirection");
        protected static readonly int IsJumpingAnim = Animator.StringToHash("IsJumping");
        protected static readonly int IsDescending = Animator.StringToHash("IsDescending");
        protected float Speed;
        protected float JumpHeight;
        protected int MaxConsecutiveJump = 1;
        protected int CurrentConsecutiveJump = 0;
        protected Vector2 FaceDirection;
        protected int CollectedGems;
        protected string characterName;

        protected GameObject Hitbox;
        protected bool IsTakingDamage;
        protected SpriteRenderer SpriteRenderer;

        protected bool IsAnchored;
        protected PhotonView AnchoredPlayer;

        protected IEnumerator FadingOut = null;

        protected bool _isMine;
        protected PlayerData PlayerData;

        
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                PlayerData = SaveSystem.LoadPlayerData();
                _isMine = true;
                characterName = statistics.characterName;
                CurrentSpeed = PlayerData.speed[characterName];
                CurrentHealth = PlayerData.maxHealth[characterName];
                CurrentAttack = PlayerData.attack[characterName];
                JumpHeight = PlayerData.jumpHeight[characterName];

                SpriteRenderer = GetComponent<SpriteRenderer>();
                Hitbox = transform.Find("PlayerHitbox").gameObject;
                Animator = GetComponent<Animator>();
            }
            else
            {
                _isMine = false;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (_isMine || localTesting)
            {
                Speed = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump") && !IsJumping)
                {
                    Jump(JumpHeight);
                    Animator.SetBool(IsJumpingAnim, true);
                }
                
                Animate();
            }
        }

        protected virtual void Animate()
        {
            if (Speed != 0)
            {
                if (Speed > 0)
                {
                    FaceDirection = Vector2.right;
                    Animator.SetFloat(FaceDirectionAnimator,1);
                }
                else
                {
                    FaceDirection = Vector2.left;
                    Animator.SetFloat(FaceDirectionAnimator,-1);
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
            if (_isMine || localTesting)
            {
                base.FixedUpdate();
                Vector2 direction;
                if (Speed != 0)
                {
                    direction = Vector2.right;
                    MoveDynamic(direction, CurrentSpeed * Speed);
                    if (AnchoredPlayer)
                    {
                        /*AnchoredPlayer.RPC("MoveWithFriend",RpcTarget.All, direction,CurrentSpeed*Speed);*/
                    }
                } /*
            else if (Speed < 0)
            {
                direction = Vector2.left;
                MoveDynamic(direction,CurrentSpeed*Speed*-1);
                if (AnchoredPlayer)
                {
                    AnchoredPlayer.RPC("MoveWithFriend",RpcTarget.All, direction,CurrentSpeed*Speed*-1);
                }
            }*/
                else
                {
                    HorizontalDeceleration();
                }

                CheckJumpPhase();
            }
        }

        protected void CheckJumpPhase()
        {
            if (IsJumping)
            {
                if (Rb.velocity.y > 0)
                {
                    Animator.SetBool(IsJumpingAnim,true);
                    Animator.SetBool(IsDescending,false);
                }
                else
                {
                    Animator.SetBool(IsJumpingAnim,false);
                    Animator.SetBool(IsDescending,true);
                }
            }
            else
            {
                Animator.SetBool(IsJumpingAnim,false);
                Animator.SetBool(IsDescending,false);
            }
        }

        public bool IsTakingDamage1
        {
            get => IsTakingDamage;
            set => IsTakingDamage = value;
        }

        public void TakeDamage(Collider2D other)
        {
            CurrentHealth -= 1;
            if (CurrentHealth <= 0)
            {
                Die();
            }
            else
            {
                IsTakingDamage = true;
                
                if ((LayerMask.GetMask("Obstacle") & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
                {

                }
                else
                {
                    StartCoroutine(nameof(DamageEffect));
                }
            }
        }

        public IEnumerator DamageEffect()
        {
            Color red = new Color(255,0,0,0.6f);
            Color white = new Color(255,255,255,0.6f);
            for (int i = 0; i < 4; i++)
            {
                SpriteRenderer.color = red;
                yield return  new WaitForSeconds(0.2f);
                SpriteRenderer.color = white;
                yield return  new WaitForSeconds(0.2f);
            }
            SpriteRenderer.color = Color.white;
            IsTakingDamage = false;
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                /*RaycastHit2D hit2D = Physics2D.BoxCast(new Vector2(Tr.position.x, Tr.position.y + 1.3f),
                    new Vector2(0.5f, 0.5f), 0, Vector2.up, LayerMask.GetMask("PlayerPhysic"));
                if (hit2D)
                {
                    AnchoredPlayer = hit2D.collider.gameObject.GetPhotonView();
                }*/
                if (FadingOut != null)
                {
                    StopCoroutine(FadingOut);
                    SpriteRenderer.color = Color.white;
                    CurrentHealth = 2;
                    FadingOut = null;
                    Hitbox.SetActive(true);
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

        public void CollectGems(int gems)
        {
            CollectedGems += gems;
        }

        protected override void Die()
        {
            Hitbox.SetActive(false);
            FadingOut = BecomingGhost();
            StartCoroutine(FadingOut);
        }

        protected IEnumerator BecomingGhost()
        {
            SpriteRenderer.color = Color.black;
            float i = 0.9f;
            while (i > 0.1f)
            {
                SpriteRenderer.color = new Color(0,0,0,i);
                yield return  new WaitForSeconds(0.75f);
                i -= 0.05f;
            }
        }

        [PunRPC]
        public void MoveWithFriend(Vector2 direction, float speed)
        {
            MoveDynamic(direction,speed);
        }

        public Vector2 FaceDirection1
        {
            get => FaceDirection;
            set => FaceDirection = value;
        }
    }
}

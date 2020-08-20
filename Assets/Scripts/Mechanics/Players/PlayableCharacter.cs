using System;
using System.Collections;
using Core.SaveLoadData;
using Photon.Pun;
using Scriptable_Objects;
using UI;
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
        protected float AttackRate;
        protected float Speed;
        protected float JumpHeight;
        protected int MaxConsecutiveJump = 1;
        protected int CurrentConsecutiveJump = 0;
        protected Vector2 FaceDirection;
        protected int CollectedGems;
        protected string CharacterName;
        protected int MaxHealth;
        protected bool CanAttack;
        protected int ReanimationHealth;

        protected HealthBar HealthBar;

        protected GameObject Hitbox;
        protected bool IsTakingDamage;
        protected SpriteRenderer SpriteRenderer;
        protected PhotonView PhotonView;

        protected IEnumerator FadingOut = null;
        protected bool Poisoned;

        protected bool IsDying;
        protected bool IsMine;
        protected PlayerData PlayerData;
        protected IEnumerator PoisoningCoroutine;

        protected Vector2 CheckPoint;

        
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Hitbox = transform.Find("PlayerHitbox").gameObject;
            Animator = GetComponent<Animator>();
            this.PhotonView = PhotonView.Get(this);
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                PlayerData = SaveSystem.LoadPlayerData();
                IsMine = true;
                CharacterName = statistics.characterName;
                ReanimationHealth = PlayerData.reanimationLife[CharacterName];
                CurrentSpeed = PlayerData.speed[CharacterName];
                MaxHealth = PlayerData.maxHealth[CharacterName];
                AttackRate = PlayerData.attackRate[CharacterName];
                CurrentHealth = MaxHealth;
                CurrentAttack = PlayerData.attack[CharacterName];
                JumpHeight = PlayerData.jumpHeight[CharacterName];
                HealthBar.InitializeHealthBar();
            }
            else
            {
                IsMine = false;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (IsMine || localTesting)
            {
                Speed = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump") && !IsJumping)
                {
                    Jump(JumpHeight);
                    Animator.SetBool(IsJumpingAnim, true);
                }
                
                Animate();
                if (Poisoned)
                {
                    StartPoisoningDamage();
                }
                else if (PoisoningCoroutine != null)
                {
                    StopCoroutine(PoisoningCoroutine);
                }
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
            if (IsMine || localTesting)
            {
                base.FixedUpdate();
                Vector2 direction;
                if (Speed != 0)
                {
                    direction = Vector2.right;
                    MoveDynamic(direction, CurrentSpeed * Speed);
                }
                else
                {
                    HorizontalDeceleration();
                }

                CheckJumpPhase();
            }
        }

        protected void StartPoisoningDamage()
        {
            if (PoisoningCoroutine != null)
            {
                PoisoningCoroutine = PoisoningDamage();
                StartCoroutine(PoisoningCoroutine);
            }
        }

        protected IEnumerator PoisoningDamage()
        {
            while (Poisoned)
            {
                CurrentHealth -= 1;
                HealthBar.LoseHearth();
                if (CurrentHealth <= 0)
                {
                    break;
                }
                this.SpriteRenderer.color = Color.green;
                yield return  new WaitForSeconds(0.2f);
                this.SpriteRenderer.color = Color.white;
                yield return new WaitForSeconds(1.8f);
            }
            if (CurrentHealth <= 0)
            {
                Die();
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
            if (IsMine && !IsDying)
            {
                CurrentHealth -= 1;
                HealthBar.LoseHearth();
                if (CurrentHealth <= 0)
                {
                    Die();
                }
                else
                {
                    IsTakingDamage = true;

                    if ((LayerMask.GetMask("DamageTrap") & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
                    {
                        //TODO
                    }
                    else
                    {
                        StartCoroutine(nameof(DamageEffect));
                        this.PhotonView.RPC(nameof(TakeRemoteDamage),RpcTarget.Others);
                    }
                }
            }
        }

        [PunRPC]
        public void TakeRemoteDamage()
        {
            IsTakingDamage = true;
            StartCoroutine(nameof(DamageEffect));
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
                if (FadingOut != null)
                {
                    StopCoroutine(FadingOut);
                    IsDying = false;
                    SpriteRenderer.color = Color.white;
                    CurrentHealth = ReanimationHealth < MaxHealth ? ReanimationHealth : MaxHealth;
                    HealthBar.RefillHearth((int)CurrentHealth);
                    FadingOut = null;
                    Hitbox.SetActive(true);
                    CanAttack = true;
                }
            }
        }

        public void CollectGems(int gems)
        {
            CollectedGems += gems;
        }

        protected override void Die()
        {
            CanAttack = false;
            IsDying = true;
            Hitbox.SetActive(false);
            FadingOut = BecomingGhost();
            StartCoroutine(FadingOut);
            this.PhotonView.RPC(nameof(RemoteDie),RpcTarget.Others);
        }

        [PunRPC]
        public void RemoteDie()
        {
            IsDying = true;
            FadingOut = BecomingGhost();
            StartCoroutine(FadingOut);
        }

        protected IEnumerator BecomingGhost()
        {
            SpriteRenderer.color = Color.black;
            float i = 1f;
            while (i > 0.05f)
            {
                SpriteRenderer.color = new Color(0,0,0,i);
                yield return  new WaitForSeconds(0.75f);
                i -= 0.05f;
            }
        }

        public void RefillHealth(int life = 1)
        {
            if (CurrentHealth + life > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            else
            {
                CurrentHealth += life;
            }
            HealthBar.RefillHearth(life);
        }

        protected IEnumerator AttackTimeLapse()
        {
            yield return new WaitForSeconds(AttackRate);
            CanAttack = true;
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

        public Vector2 CheckPoint1
        {
            get => CheckPoint;
            set => CheckPoint = value;
        }

        public bool Poisoned1
        {
            get => Poisoned;
            set => Poisoned = value;
        }

        public HealthBar HealthBar1
        {
            get => HealthBar;
            set => HealthBar = value;
        }
    }
}

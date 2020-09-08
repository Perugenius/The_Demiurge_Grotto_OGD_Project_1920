using System;
using System.Collections;
using Core;
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
        protected static readonly int Appear = UnityEngine.Animator.StringToHash("Appear");
        protected static readonly int Disappear = UnityEngine.Animator.StringToHash("Disappear");
        protected float AttackRate;
        protected float Speed;
        protected float JumpHeight;
        protected int MaxConsecutiveJump = 1;
        protected int CurrentConsecutiveJump = 0;
        protected Vector2 FaceDirection;
        protected int CollectedGems;
        protected string CharacterName;
        protected int MaxHealth;
        protected bool CanAttack = true;
        protected int ReanimationHealth;
        protected bool CanMove = true;
        protected bool JumpingController;
        protected float MaxJumpTime;
        protected float CurrentJumpTime;
        protected GameObject Shadow;

        protected HealthBar HealthBar;
        protected Bar AttackBar;

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
        public bool immortalMode;
        protected PlayableCharacter OtherPlayer;



        // Start is called before the first frame update
        protected virtual void Start()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Hitbox = transform.Find("PlayerHitbox").gameObject;
            Animator = GetComponent<Animator>();
            this.PhotonView = PhotonView.Get(this);
            Shadow = transform.Find("Shadow").gameObject;
            Shadow.SetActive(false);
            if (!localTesting)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (var player in players)
                {
                    if (!player.GetPhotonView().IsMine)
                    {
                        OtherPlayer = player.GetComponent<PlayableCharacter>();
                    }
                }
            }
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
                AttackBar.SetDuration(AttackRate);
                MaxJumpTime = 0.35f;
                CurrentJumpTime = 0;
            }
            else
            {
                IsMine = false;
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if ((IsMine || localTesting))
            {
                if (CanMove)
                {
                    Speed = Input.GetAxisRaw("Horizontal");
                    if (Input.GetButtonDown("Jump") && !IsJumping)
                    {
                        JumpingController = true;
                        Rb.velocity = new Vector2(Rb.velocity.x,15);
                        AudioManager.Instance.PlaySound("JumpSFX");
                        Animator.SetBool(IsJumpingAnim, true);
                    }

                    if (Input.GetButton("Jump") && JumpingController)
                    {
                        ModulateJump();
                    }
                    if (Input.GetButtonUp("Jump"))
                    {
                        JumpingController = false;
                        CurrentJumpTime = 0;
                    }

                    Animate();
                    if (Poisoned && !IsDying && !IsTakingDamage)
                    {
                        StartPoisoningDamage();
                    }
                }
                else
                {
                    Speed = 0;
                }
            }
        }

        protected void ModulateJump()
        {
            float horizontalRbVelocity = Rb.velocity.x;
            if (CurrentJumpTime < MaxJumpTime/4)
            {
                Rb.velocity = new Vector2(horizontalRbVelocity,12.5f);
                CurrentJumpTime += Time.deltaTime;
            }
            else if (CurrentJumpTime < MaxJumpTime / 2)
            {
                Rb.velocity = new Vector2(horizontalRbVelocity,10.5f);
                CurrentJumpTime += Time.deltaTime;
            }
            else if (CurrentJumpTime < MaxJumpTime *3/ 4)
            {
                Rb.velocity = new Vector2(horizontalRbVelocity,8);
                CurrentJumpTime += Time.deltaTime;
            }
            else if (CurrentJumpTime < MaxJumpTime )
            {
                Rb.velocity = new Vector2(horizontalRbVelocity,6);
                CurrentJumpTime += Time.deltaTime;
            }
            else
            {
                JumpingController = false;
                CurrentJumpTime = 0;
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
                //Rb.velocity = new Vector2(Speed * CurrentSpeed, Rb.velocity.y);
                if (Speed != 0)
                {
                    MoveDynamic(Vector2.right, CurrentSpeed * Speed);
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
            if (PoisoningCoroutine == null)
            {
                PoisoningCoroutine = PoisoningDamage();
                StartCoroutine(PoisoningCoroutine);
            }
        }

        protected IEnumerator PoisoningDamage()
        {
            /*yield return new WaitForSeconds(1f);*/
            while (Poisoned && Hitbox.activeSelf)
            {
                CurrentHealth -= 1;
                HealthBar.LoseHearth();
                PhotonView.RPC(nameof(RemotePoisoningEffect),RpcTarget.Others);
                this.SpriteRenderer.color = Color.green;
                if (CurrentHealth <= 0)
                {
                    break;
                }
                yield return  new WaitForSeconds(0.2f);
                this.SpriteRenderer.color = Color.white;
                yield return new WaitForSeconds(4.3f);
            }
            PoisoningCoroutine = null;
            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        [PunRPC]
        protected void RemotePoisoningEffect()
        {
            IEnumerator enumerator = ChangeSpriteColor(Color.green, 0.2f);
            StartCoroutine(enumerator);
        }

        protected IEnumerator ChangeSpriteColor(Color color, float time)
        {
            SpriteRenderer.color = color;
            yield return new WaitForSeconds(time);
            SpriteRenderer.color = Color.white;
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
            if (IsMine && !IsDying )
            {
                if (!immortalMode || (immortalMode && CurrentHealth > 1))
                {
                    CurrentHealth -= 1;
                    HealthBar.LoseHearth();
                    AudioManager.Instance.PlaySound("PlayerHitSFX");
                }

                if (other.gameObject.layer == LayerMask.NameToLayer("DamageTrap"))
                {
                    IsTakingDamage = true;
                    PhotonView.RPC(nameof(RemoteDisappearAnimation), RpcTarget.Others);
                    StartCoroutine(nameof(DisappearAnimation));
                }
                if (CurrentHealth <= 0)
                {
                    Die();
                    Debug.Log(IsDying);
                }
                else
                {
                    if(!immortalMode || (immortalMode && CurrentHealth > 1))
                    {
                        IsTakingDamage = true;
                        StartCoroutine(nameof(DamageEffect));
                        this.PhotonView.RPC(nameof(TakeRemoteDamage), RpcTarget.Others);
                    }
                }
            }
            else if (IsMine && IsDying && other.gameObject.layer == LayerMask.NameToLayer("DamageTrap"))
            {
                PhotonView.RPC(nameof(RemoteDisappearAnimation), RpcTarget.Others);
                StartCoroutine(nameof(DisappearAnimation));
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

        protected IEnumerator DisappearAnimation()
        {
            CanMove = false;
            Animator.SetTrigger(Disappear);
            yield return new WaitForSeconds(1);
            if (IsMine)
            {
                transform.position = CheckPoint;
            }
            Animator.SetTrigger(Appear);
            yield return  new WaitForSeconds(1);
            CanMove = true;
            IsTakingDamage = false;
        }

        [PunRPC]
        protected void RemoteDisappearAnimation()
        {
            StartCoroutine(nameof(DisappearAnimation));
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
                    if (IsMine)
                    {
                        CurrentHealth = ReanimationHealth < MaxHealth ? ReanimationHealth : MaxHealth;
                        HealthBar.RefillHearth((int) CurrentHealth);
                    }
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
            if (OtherPlayer && OtherPlayer.IsDying1)
            {
                PhotonView.RPC(nameof(CallRemoteGameOver),RpcTarget.All);
            }
            else
            {
                this.PhotonView.RPC(nameof(RemoteDie), RpcTarget.Others);
                FadingOut = BecomingGhost();
                StartCoroutine(FadingOut);
            }

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
            while (i > 0.06f)
            {
                SpriteRenderer.color = new Color(0,0,0,i);
                yield return  new WaitForSeconds(0.5f);
                i -= 0.02f;
            }

            if (!immortalMode)
            {
                PhotonView.RPC(nameof(CallRemoteGameOver),RpcTarget.All);
            }
        }

        [PunRPC]
        protected void CallRemoteGameOver()
        {
            GameManager.Instance.ShowGameOverScreen();
        }

        public void RefillHealth(int life = 1)
        {
            if (!IsDying)
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
        }

        public void SetShadowActive(bool condition)
        {
            Shadow.SetActive(condition);
        }
        

        public void KillPlayer()
        {
            CurrentHealth = 0;
            Die();
        }

        protected IEnumerator AttackTimeLapse()
        {
            AttackBar.ActualSize = 0;
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

        public bool IsDying1
        {
            get => IsDying;
            set => IsDying = value;
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

        public string CharacterName1
        {
            get => CharacterName;
            set => CharacterName = value;
        }

        public Bar AttackBar1
        {
            get => AttackBar;
            set => AttackBar = value;
        }
        
        public PlayableCharacter OtherPlayer1
        {
            get => OtherPlayer;
            set => OtherPlayer = value;
        }
    }
}

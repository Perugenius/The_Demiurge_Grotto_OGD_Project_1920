using System.Collections;
using System.IO;
using Core;
using Mechanics.Players.PlayerAttacks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mechanics.Players
{
    public class Steve : PlayableCharacter
    {
        private Transform _spawnPosition;

        private static readonly int IsOnWallAnimator = Animator.StringToHash("IsOnWall");
        private static readonly int WallSideAnimator = Animator.StringToHash("WallSide");
        private bool _isOnWall = false;
        private WallSide _wallSide;
        private PhysicsMaterial2D _colliderMaterial;

        private float _fireRate = 2;
        private float _range = 3;
        private float _projectileSpeed = 10;

        public GameObject laserPrefab;
        
        
        
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (IsMine)
            {
                _range = PlayerData.attackRange[CharacterName];
                _fireRate = PlayerData.attackRate[CharacterName];
                _projectileSpeed = PlayerData.projectileSpeed[CharacterName];
                _spawnPosition = transform.Find("LaserSpawner");
                _colliderMaterial = gameObject.GetComponent<EdgeCollider2D>().sharedMaterial;
                _colliderMaterial.friction = (PlayerData.secondarySkillLevel[CharacterName] - 1) * 0.2f + 0.7f;
            }
            
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                if (CanMove)
                {
                    Speed = Input.GetAxisRaw("Horizontal");
                    if (_isOnWall && (_wallSide == WallSide.RightWall && Speed > 0) ||
                        (_wallSide == WallSide.LeftWall && Speed < 0))
                    {
                        if (Input.GetButtonDown("Jump"))
                        {
                            Rb.velocity = new Vector2(Rb.velocity.x, 0);
                            if (_wallSide == WallSide.RightWall)
                            {
                                Rb.AddForce(new Vector2(-62, 62), ForceMode2D.Impulse);
                            }
                            else
                            {
                                Rb.AddForce(new Vector2(62, 62), ForceMode2D.Impulse);
                            }

                            Animator.SetBool(IsJumpingAnim, true);
                        }
                        else if (Input.GetButton("Jump") && JumpingController)
                        {
                            ModulateJump();
                        }
                    }
                    else
                    {
                        /*if (Input.GetButtonDown("Jump") && !IsJumping)
                        {
                            Jump(62);
                            Animator.SetBool(IsJumpingAnim, true);
                        }*/
                        if (Input.GetButtonDown("Jump") && !IsJumping)
                        {
                            JumpingController = true;
                            AudioManager.Instance.PlaySound("JumpSFX");
                            Rb.velocity = new Vector2(Rb.velocity.x,15);
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
                    }
                    Animate();
                    if (Input.GetButtonDown("Attack") && CanAttack)
                    {
                        Attack();
                    }

                    if (Poisoned && !IsDying && !IsTakingDamage)
                    {
                        StartPoisoningDamage();
                    }
                }
            }
        }
        

        protected override void Attack()
        {
            CanAttack = false;
            GameObject laser;
            if (!localTesting)
            {
                laser =
                    PhotonNetwork.Instantiate(Path.Combine("Players", "Laser"), _spawnPosition.position,
                        Quaternion.identity);
            }
            else
            {
                laser = GameObject.Instantiate(laserPrefab, _spawnPosition.position, Quaternion.identity);
            }
            Laser laserScript = laser.GetComponent<Laser>();
            if (localTesting)
                laserScript.Offline = true;
            laserScript.Damage = CurrentAttack;
            laserScript.Direction = FaceDirection;
            laserScript.Range = _range;
            laserScript.Speed = _projectileSpeed;
            AudioManager.Instance.PlaySound("LaserSFX");
            StartCoroutine(nameof(AttackTimeLapse));
        }

        protected override void Animate()
        {
            base.Animate();
            if (_isOnWall)
            {
                if ((_wallSide == WallSide.RightWall && Speed > 0) || (_wallSide == WallSide.LeftWall && Speed < 0))
                {
                    Animator.SetBool(IsOnWallAnimator, true);
                    if (_wallSide == WallSide.RightWall)
                    {
                        FaceDirection = Vector2.left;
                        Animator.SetFloat(WallSideAnimator, 1);
                    }
                    else
                    {
                        FaceDirection = Vector2.right;
                        Animator.SetFloat(WallSideAnimator, -1);
                    }
                }
                else
                {
                    Animator.SetBool(IsOnWallAnimator, false);
                }
            }
            else
            {
                Animator.SetBool(IsOnWallAnimator, false);
            }
        }
        

        public bool IsOnWall
        {
            get => _isOnWall;
            set => _isOnWall = value;
        }

        public WallSide WallSide
        {
            get => _wallSide;
            set => _wallSide = value;
        }
        
        
    }
}

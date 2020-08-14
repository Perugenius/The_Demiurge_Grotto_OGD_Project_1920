using System.Collections;
using System.IO;
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

        private float _fireRate = 1.5f;
        private bool _canAttack = true;
        private float _range = 5;
        private float _projectileSpeed = 10;
        
        
        
        
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _spawnPosition = transform.Find("LaserSpawner");
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                Speed = Input.GetAxisRaw("Horizontal"); 
                if (_isOnWall)
                {
                    if (Input.GetButtonDown("Jump"))
                    {
                        Rb.velocity = new Vector2(Rb.velocity.x,0);
                        if (_wallSide == WallSide.RightWall)
                        {
                            Rb.AddForce(new Vector2(-62,62),ForceMode2D.Impulse);
                        }
                        else
                        {
                            Rb.AddForce(new Vector2(62,62),ForceMode2D.Impulse);
                        }
                        Animator.SetBool(IsJumpingAnim, true);
                    }
                }
                else
                { 
                    if (Input.GetButtonDown("Jump") && !IsJumping)
                    {
                        Jump(62);
                        Animator.SetBool(IsJumpingAnim, true);
                    }
                }

                Animate();
                if (Input.GetButtonDown("Attack") && _canAttack)
                {
                    Attack();
                }
                
            }
        }
        

        protected override void Attack()
        {
            _canAttack = false;
            GameObject laser =
                PhotonNetwork.Instantiate(Path.Combine("Players", "Laser"), _spawnPosition.position, Quaternion.identity);
            Laser laserScript = laser.GetComponent<Laser>();
            laserScript.Damage = CurrentAttack;
            laserScript.Direction = FaceDirection;
            laserScript.Range = _range;
            laserScript.Speed = _projectileSpeed;
            StartCoroutine(nameof(AttackRecharge));
        }

        protected override void Animate()
        {
            base.Animate();
            if (_isOnWall)
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

        private IEnumerator AttackRecharge()
        {
            yield return new WaitForSeconds(_fireRate);
            _canAttack = true;
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

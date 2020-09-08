using System.Collections;
using Core;
using Photon.Pun;
using UnityEngine;

namespace Mechanics.Players
{
    public class Voodoo : PlayableCharacter
    {

        private bool _isDashing = false;
        private Vector2 _dashDirection;

        private GameObject _damageCollider;
        private GameObject _playerHitbox;

        private float _initialMass;
        private float _horizontalDashSpeed;
        private float _verticalDashSpeed;

        private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            base.Start();
            if (IsMine)
            {
                
                _damageCollider = transform.Find("DamageCollider").gameObject;
                _playerHitbox = transform.Find("PlayerHitbox").gameObject;
                _damageCollider.SetActive(false);
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _initialMass = Rb.mass;
            }
        }
        
        // Update is called once per frame
        protected override void Update()
        {
            if (IsMine || localTesting)
            {
                base.Update();
                IEnumerator coroutine;
                if (Input.GetAxis("Vertical") > 0 && !_isDashing)
                {
                    _dashDirection = Vector2.up;
                    coroutine = DashTimer(0.15f);
                }
                else if (!_isDashing)
                {
                    _dashDirection = FaceDirection;
                    coroutine = DashTimer(0.25f);
                }
                else
                {
                    coroutine = null;
                }

                if (Input.GetButtonDown("Attack") && !_isDashing && !IsJumping && CanAttack && !IsDying)
                {
                    CanAttack = false;
                    Attack();
                    _isDashing = true;
                    StartCoroutine(coroutine);
                }
            }
        }

        protected override void FixedUpdate()
        {
            if (IsMine || localTesting)
            {
                if (!_isDashing)
                    base.FixedUpdate();
                else
                {
                    Dash();
                }
            }
        }

        protected override void Attack()
        {
            AudioManager.Instance.PlaySound("DashSFX");
            _spriteRenderer.color = new Color(255,255,255,0.6f);
            _damageCollider.SetActive(true);
            _playerHitbox.SetActive(false);
        }

        private void Dash()
        {
            if (_dashDirection == Vector2.up)
            {
                MoveDynamicUp(_dashDirection,30,0.02f);
            }
            else
            {
                MoveDynamic(_dashDirection,30,0.05f);
            }
            /*if (_dashDirection == Vector2.up)
            {
                
                Rb.velocity = new Vector2(Rb.velocity.x,0);
                Rb.mass = 0.1f;
                Rb.AddForce(new Vector2(0,3),ForceMode2D.Impulse);
            }
            else
            {
                if(_dashDirection == Vector2.right) Rb.AddForce(new Vector2(JumpHeight+8,0),ForceMode2D.Impulse);
                else
                {
                    Rb.AddForce(new Vector2(-JumpHeight-8,0),ForceMode2D.Impulse);
                }
            }*/
            
        }
        

        private IEnumerator DashTimer(float toWait)
        {
            yield return new WaitForSeconds(toWait);
            _damageCollider.SetActive(false);
            _playerHitbox.SetActive(true);
            _spriteRenderer.color = new Color(255,255,255,1);
            _isDashing = false;
            Rb.velocity = new Vector2(Rb.velocity.x,0);
            StartCoroutine(AttackTimeLapse());
        }

        public bool IsDashing
        {
            get => _isDashing;
            set => _isDashing = value;
        }
    }
}

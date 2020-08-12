using System.Collections;
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

        protected override void Start()
        {
            base.Start();
            _damageCollider = transform.Find("DamageCollider").gameObject;
            _playerHitbox = transform.Find("PlayerHitbox").gameObject;
            _damageCollider.SetActive(false);
        }
        
        // Update is called once per frame
        protected override void Update()
        {
            if (gameObject.GetPhotonView().IsMine || localTesting)
            {
                base.Update();
                IEnumerator coroutine;
                if (Input.GetAxis("Vertical") > 0 && !_isDashing)
                {
                    _dashDirection = Vector2.up;
                    coroutine = DashTimer(0.04f);
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

                if (Input.GetButtonDown("Attack") && !_isDashing)
                {
                    Attack();
                    _isDashing = true;
                    StartCoroutine(coroutine);
                }
            }
        }

        protected override void FixedUpdate()
        {
            if(!_isDashing)
                base.FixedUpdate();
            else
            {
                Dash();
            }
        }

        protected override void Attack()
        {
            _damageCollider.SetActive(true);
            _playerHitbox.SetActive(false);
        }

        private void Dash()
        {
            if (_dashDirection == Vector2.up)
            {
                MoveDynamicUp(_dashDirection,CurrentSpeed*6,0.005f);
            }
            else
            {
                MoveDynamic(_dashDirection,CurrentSpeed*3,0.05f);
            }
            
        }

        protected override void Die()
        {
            throw new System.NotImplementedException();
        }

        private IEnumerator DashTimer(float toWait)
        {
            yield return new WaitForSeconds(toWait);
            _damageCollider.SetActive(false);
            _playerHitbox.SetActive(true);
            _isDashing = false;
            Rb.velocity = new Vector2(Rb.velocity.x,0);
        }
        
    }
}

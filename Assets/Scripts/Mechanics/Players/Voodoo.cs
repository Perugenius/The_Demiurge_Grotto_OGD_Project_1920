using System.Collections;
using UnityEngine;

namespace Mechanics.Players
{
    public class Voodoo : PlayableCharacter
    {

        private bool _isDashing = false;
        private Vector2 _dashDirection;
        
        
        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            IEnumerator coroutine;
            if (Input.GetAxis("Vertical") >0)
            {
                _dashDirection = Vector2.up;
                coroutine = DashTimer(0.03f);
            }
            else
            {
                _dashDirection = FaceDir;
                coroutine = DashTimer(0.3f);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing)
            {
                _isDashing = true;
                
                StartCoroutine(coroutine);
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
            throw new System.NotImplementedException();
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
            _isDashing = false;
            Rb.velocity = new Vector2(Rb.velocity.x,0);
        }
        
    }
}

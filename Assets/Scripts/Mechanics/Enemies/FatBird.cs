using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class FatBird : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float acceleration;
        [SerializeField] private float amplitude;
        private float _altitude;
        private bool _falling;
        private bool _returning;
        
        
        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponent<Animator>();
            _direction = Vector2.up;
            _altitude = Tr.position.y;
            SetFloating(acceleration, amplitude);
        }

        private void Update()
        {
            
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (hit.collider && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic") && !_falling)
            {
                _falling = true;
                Animator.SetBool("Falling", true);
                StopFloating();
                RaycastHit2D ground = Physics2D.Raycast(transform.position, Vector2.down, 100, LayerMask.GetMask("Obstacle"));
                SetFixedDistanceAccelerated(Vector3.down, 0, ground.distance - 1, acceleration*5);
            }

            if (_returning && _falling && MoveFixedDistanceAcceleratedDecelerated == false)
            {
                _returning = false;
                _falling = false;
                SetFloating(acceleration, amplitude);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                Animator.SetBool("Falling", false);
                Animator.SetBool("Ground", true);
                StartCoroutine (nameof(Cooldown));
            }
        }
        
        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (Animator.GetCurrentAnimatorStateInfo(0).length);
            SetFixedDistanceAcceleratedDecelerated(Tr.position + Vector3.up*(_altitude - Tr.position.y), 0, acceleration/2);
            _returning = true;
            Animator.SetBool("Ground", false);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                if(!Hit) Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }
    }
}

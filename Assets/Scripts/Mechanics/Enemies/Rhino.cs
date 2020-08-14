using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Rhino : Enemy
    {
        private Vector2 _direction;
        [SerializeField] private float speed;
        [SerializeField] private float knockbackHeight;
        [SerializeField] private float knockbackThrust;
        private bool _run;
        
        // Start is called before the first frame update
        void Start()
        {
            Animator = GetComponent<Animator>();
            RaycastHit2D left = Physics2D.Raycast(Tr.position, Vector2.left,100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            RaycastHit2D right = Physics2D.Raycast(Tr.position, Vector2.right, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (left.distance > right.distance)
            {
                _direction = Vector2.left;
            }
            else
            {
                _direction = Vector2.right;
                Tr.Rotate(0f,180,0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        protected override void FixedUpdate()
        {
            if(!Physics2D.OverlapPoint(Tr.position + new Vector3(0, -1.1f, 0), LayerMask.GetMask("Obstacle"))) return;    //if falling, it does nothing
            base.FixedUpdate();
            if (_run)
            {
                MoveDynamic(_direction, speed, .5f);
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position, _direction, 100, LayerMask.GetMask("PlayerPhysic","Obstacle"));
            if (hit.collider && !_run && hit.collider.gameObject.layer == LayerMask.NameToLayer("PlayerPhysic"))
            {
                _run = true;
                Animator.SetBool("Run",true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                _run = false;
                Animator.SetBool("Run",false);
                Animator.SetBool("Bump",true);
                JumpLateral(knockbackHeight, Vector2.Reflect(_direction, Vector2.right)*knockbackThrust);
                StartCoroutine (nameof(Cooldown));
            }

            if (other.gameObject.layer == LayerMask.NameToLayer("DamagePlayer"))
            {
                Damage(other.GetComponent<IDamageInflictor>().GetDamage());
            }
        }
        
        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (Animator.GetCurrentAnimatorStateInfo(0).length);
            Animator.SetBool("Bump", false);
            Tr.Rotate(0f,180,0f);
            _direction = Vector2.Reflect(_direction, Vector2.right);
        }
    }
}

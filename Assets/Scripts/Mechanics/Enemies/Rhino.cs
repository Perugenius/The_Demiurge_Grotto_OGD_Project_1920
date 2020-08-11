using System;
using System.Collections;
using UnityEngine;

namespace Mechanics.Enemies
{
    public class Rhino : Movable
    {
        private Vector2 _direction;
        private Animator _animator;
        [SerializeField] private float speed;
        [SerializeField] private float knockbackHeight;
        [SerializeField] private float knockbackThrust;
        [SerializeField] private float lifePoints;
        private bool _run;
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
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
                _animator.SetBool("Run",true);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                _run = false;
                _animator.SetBool("Run",false);
                _animator.SetBool("Bump",true);
                JumpLateral(knockbackHeight, Vector2.Reflect(_direction, Vector2.right)*knockbackThrust);
                StartCoroutine (nameof(Cooldown));
            }
        }
        
        private IEnumerator Cooldown(){
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            _animator.SetBool("Bump", false);
            Tr.Rotate(0f,180,0f);
            _direction = Vector2.Reflect(_direction, Vector2.right);
        }

        private void Damage(float damage)
        {
            _animator.SetTrigger("Hit");
            if (damage < lifePoints)
            {
                lifePoints = lifePoints - damage;
            }
            else StartCoroutine (nameof(Die));
        }

        private IEnumerator Die(){
            yield return new WaitForSeconds (_animator.GetCurrentAnimatorStateInfo(0).length);
            Destroy(gameObject);
        }
    }
}
